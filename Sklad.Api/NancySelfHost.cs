using System;
using Nancy.Hosting.Self;
using Sklad.Api.Configuration;

namespace Sklad.Api
{
    public class NancySelfHost
    {
        private NancyHost host;

        public void Start()
        {
            var config = ConfigLoader.Load();
            var ub = new UriBuilder
            {
                Host = "localhost",
                Port = config.Port,
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
