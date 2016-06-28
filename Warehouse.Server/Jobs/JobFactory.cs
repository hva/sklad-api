using FluentScheduler;
using Microsoft.Practices.Unity;

namespace Warehouse.Server.Jobs
{
    public class JobFactory : IJobFactory
    {
        private readonly IUnityContainer container;

        public JobFactory(IUnityContainer container)
        {
            this.container = container;
        }

        public IJob GetJobInstance<T>() where T : IJob
        {
            return container.Resolve<T>();
        }
    }
}