using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Hosting;
using FluentScheduler;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
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
                var task = ExecuteAsync();
                task.Wait();
            }
            catch (AggregateException e)
            {
                foreach (var inner in e.Flatten().InnerExceptions)
                {
                    logger.Error(inner.Message);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        public async Task ExecuteAsync()
        {
            var now = DateTime.UtcNow;
            var dbName = GetDatabaseName();
            var workingDir = Path.Combine(Path.GetTempPath(), "skill-backup_" + dbName);

            Dump(workingDir, dbName);

            var zipFile = string.Format("{0}_{1:yyyyMMdd_HHmm}.zip", dbName, now);
            Zip(workingDir, zipFile);

            var uploadPath = string.Format("skill-backup/{0:yyyy_MM}", now);
            var token = GetToken();
            await CreateUploadPathAsync(uploadPath, token);

            var uploadLink = await GetUploadLinkAsync(uploadPath, token, zipFile);
            await UploadFileAsync(workingDir, zipFile, uploadLink);

            Cleanup(workingDir, zipFile);

            logger.Info("backup created: " + zipFile);
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

        private static string GetToken()
        {
            var token = ConfigurationManager.AppSettings["BackupToken"];
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("token is empty");
            }
            return token;
        }

        private static async Task CreateUploadPathAsync(string uploadPath, string token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                client.BaseAddress = new Uri("https://cloud-api.yandex.net:443");

                var uriString = string.Format("/v1/disk/resources/?path={0}", WebUtility.UrlEncode(uploadPath));

                var resp = await client.GetAsync(uriString);
                if (resp.StatusCode == HttpStatusCode.NotFound)
                {
                    var message = new HttpRequestMessage(HttpMethod.Put, uriString);
                    var resp2 = await client.SendAsync(message);
                    if (resp2.StatusCode != HttpStatusCode.Created)
                    {
                        throw new Exception("can't create upload path");
                    }
                }
            }
        }

        private static async Task<string> GetUploadLinkAsync(string uploadPath, string token, string zipFile)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                client.BaseAddress = new Uri("https://cloud-api.yandex.net:443");

                var path = string.Concat(uploadPath, "/", zipFile);
                var linkUriString = string.Format("/v1/disk/resources/upload?path={0}", WebUtility.UrlEncode(path));

                var resp = await client.GetAsync(linkUriString);
                string link = null;
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    var content = await resp.Content.ReadAsStringAsync();
                    var json = JObject.Parse(content);
                    link = json["href"].ToString();
                }
                if (string.IsNullOrEmpty(link))
                {
                    throw new Exception("empty upload link");
                }
                return link;
            }
        }

        private static async Task UploadFileAsync(string workingDir, string zipFile, string uploadLink)
        {
            var timeoutString = ConfigurationManager.AppSettings["BackupUploadTimeout"];
            var timeout = TimeSpan.Parse(timeoutString);

            var zipFullPath = Path.Combine(workingDir, zipFile);
            using (var client = new HttpClient { Timeout = timeout })
            using (var stream = File.OpenRead(zipFullPath))
            using (var content = new StreamContent(stream))
            {
                var resp = await client.PutAsync(uploadLink, content);
                if (resp.StatusCode != HttpStatusCode.Created)
                {
                    var errorContent = await resp.Content.ReadAsStringAsync();
                    throw new Exception(errorContent);
                }
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