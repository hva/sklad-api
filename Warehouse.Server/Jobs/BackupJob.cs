using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Web.Hosting;
using FluentScheduler;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using MongoDB.Driver;
using Warehouse.Server.Logger;

namespace Warehouse.Server.Jobs
{
    public class BackupJob : IJob, IRegisteredObject
    {
        private readonly object _lock = new object();
        private bool _shuttingDown;
        private readonly ILogger logger;

        public BackupJob(ILogger logger)
        {
            this.logger = logger;

            // Register this job with the hosting environment.
            // Allows for a more graceful stop of the job, in the case of IIS shutting down.
            HostingEnvironment.RegisterObject(this);
        }

        public void Execute()
        {
            lock (_lock)
            {
                if (_shuttingDown)
                    return;

                // Do work, son!
                ExecuteInternal();
            }
        }

        public void Stop(bool immediate)
        {
            // Locking here will wait for the lock in Execute to be released until this code can continue.
            lock (_lock)
            {
                _shuttingDown = true;
            }

            HostingEnvironment.UnregisterObject(this);
        }

        private void ExecuteInternal()
        {
            try
            {
                var now = DateTime.UtcNow;
                var dbName = GetDatabaseName();
                logger.Info("backup database " + dbName);

                var workingDir = Path.Combine(Path.GetTempPath(), "skill-backup_" + dbName);
                logger.Info("backup working dir " + workingDir);

                Dump(workingDir, dbName);
                logger.Info("backup execute dump command");

                var zipFile = string.Format("{0}_{1:yyyyMMdd_HHmm}.zip", dbName, now);
                Zip(workingDir, zipFile);
                logger.Info("backup zip file " + zipFile);

                const string containerName = "skill-backup";
                UploadBlob(workingDir, zipFile, containerName);
                logger.Info("backup upload blob to container " + containerName);

                Cleanup(workingDir, zipFile);
                logger.Info("backup cleanup");

                logger.Info("backup finished");
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        private static string GetDatabaseName()
        {
            var connectionString = ConfigurationManager.AppSettings["MongoDB"];
            var url = new MongoUrl(connectionString);
            return url.DatabaseName;
        }

        private static void Dump(string workingDir, string dbName)
        {
            var info = new ProcessStartInfo
            {
                FileName = "mongodump",
                Arguments = string.Format("--db {0} --out {1}", dbName, Path.Combine(workingDir, "dump")),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
            };

            var process = new Process { StartInfo = info };
            try
            {
                process.Start();
                process.WaitForExit();
            }
            catch
            {
                var message = string.Format("process start failed: {0} {1}", info.FileName, info.Arguments);
                throw new Exception(message);
            }

            var error = process.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
        }

        private static void Zip(string workingDir, string zipFile)
        {
            var dumpPath = Path.Combine(workingDir, "dump");
            var zipFullPath = Path.Combine(workingDir, zipFile);
            ZipFile.CreateFromDirectory(dumpPath, zipFullPath);
        }

        private static void UploadBlob(string workingDir, string zipFile, string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExists();
            var blockBlob = container.GetBlockBlobReference(zipFile);
            var zipFullPath = Path.Combine(workingDir, zipFile);
            using (var fileStream = File.OpenRead(zipFullPath))
            {
                blockBlob.UploadFromStream(fileStream);
            }
        }

        private void Cleanup(string workingDir, string zipFile)
        {
            var dumpPath = Path.Combine(workingDir, "dump");
            if (Directory.Exists(dumpPath))
            {
                Directory.Delete(dumpPath, true);
            }

            var zipFullPath = Path.Combine(workingDir, zipFile);
            if (File.Exists(zipFullPath))
            {
                File.Delete(zipFullPath);
            }
        }
    }
}