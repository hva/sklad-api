using System;
using Nancy;
using Nancy.Hosting.Self;
using Serilog;

namespace Sklad.Api
{
    public class NancySelfHost
    {
        private NancyHost host;

        public void Start(Uri uri)
        {
            StaticConfiguration.DisableErrorTraces = false;

            host = new NancyHost(uri);
            host.Start();

            Log.Information("Running on {0}.", uri);
        }

        public void Stop()
        {
            host.Stop();

            Log.Information("Stopped. Good bye!");
            Log.CloseAndFlush();
        }
    }
}
