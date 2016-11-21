using System.Configuration;
using FluentScheduler;
using Warehouse.Server.Logger;

namespace Warehouse.Server.Jobs
{
    public class BackupRegistry : Registry
    {
        public BackupRegistry(ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IsBackupEnabled"]))
            {
                logger.Info("backup is disabled");
                return;
            }

            UseUtcTime();
            Schedule<BackupJob>().ToRunEvery(1).Days().At(21, 0).WeekdaysOnly();

            logger.Info("backup job has been scheduled");
        }
    }
}