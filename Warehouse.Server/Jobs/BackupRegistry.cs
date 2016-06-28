using FluentScheduler;

namespace Warehouse.Server.Jobs
{
    public class BackupRegistry : Registry
    {
        public BackupRegistry()
        {
            //Schedule<BackupJob>().ToRunEvery(1).Days().At(18, 47).WeekdaysOnly();
            Schedule<BackupJob>().ToRunEvery(30).Minutes();
        }
    }
}