using System;
using Nancy.Hosting.Self;

namespace SkladApi
{
    public class NancySelfHost
    {
        private NancyHost host;
        private readonly Uri uri = new Uri("http://localhost:8000");

        public void Start()
        {
            host = new NancyHost(uri);
            host.Start();
            Console.WriteLine("Running on {0}.", uri);
        }

        public void Stop()
        {
            host.Stop();
            Console.WriteLine("Stopped. Good bye!");
        }
    }
}
