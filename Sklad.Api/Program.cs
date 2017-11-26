using Topshelf;

namespace Sklad.Api
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<NancySelfHost>(s =>
                {
                    s.ConstructUsing(name => new NancySelfHost());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription("Sklad API Service");
                x.SetDisplayName("Sklad API");
                x.SetServiceName("Sklad-API");
            });
        }
    }
}
