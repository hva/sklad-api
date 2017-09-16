using System;
using Nancy.Hosting.Self;

namespace SkladApi
{
    public class NancySelfHost
    {
        private NancyHost host;

        public void Start()
        {
            var ub = new UriBuilder {
                Host = "localhost",
                Port = Config.Load().Port,
            };
            host = new NancyHost(ub.Uri);
            host.Start();
            Console.WriteLine("Running on {0}.", ub.Uri);
        }

        public void Stop()
        {
            host.Stop();
            Console.WriteLine("Stopped. Good bye!");
        }
    }
}
