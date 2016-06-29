using FluentScheduler;

namespace Warehouse.Server.Jobs
{
    public class BackupRegistry : Registry
    {
        public BackupRegistry()
        {
            Schedule<BackupJob>().ToRunEvery(1).Days().At(21, 0).WeekdaysOnly();
        }
    }
}