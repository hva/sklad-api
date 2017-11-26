using System;
using System.Linq;
using System.Net;
using Serilog;
using Sklad.Api.Configuration;
using Topshelf;

namespace Sklad.Api
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = ConfigLoader.Load();
            var ub = new UriBuilder
            {
                Host = "localhost",
                Port = config.Port,
            };

            SetupLog(config.Log.Host, config.Log.Port, config.ServiceName);

            HostFactory.Run(x =>
            {
                x.Service<NancySelfHost>(s =>
                {
                    s.ConstructUsing(name => new NancySelfHost());
                    s.WhenStarted(tc => tc.Start(ub.Uri));
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription(config.ServiceName);
                x.SetDisplayName(config.ServiceName);
                x.SetServiceName(config.ServiceName);
            });
        }

        private static void SetupLog(string host, int port, string displayName)
        {
            IPAddress ip;
            if (TryResolveIPAddress(host, out ip))
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .Enrich.WithProperty("Program", displayName)
                    .WriteTo.Udp(ip, port,
                        outputTemplate:
                            "<22>{Timestamp:MMM d H:mm:ss} {Program}: [{Level}] {Message}{NewLine}{Exception}")
                    .CreateLogger();
                return;
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

        }

        private static bool TryResolveIPAddress(string host, out IPAddress ipAddress)
        {
            ipAddress = IPAddress.None;

            if (string.IsNullOrEmpty(host))
            {
                return false;
            }

            try
            {
                ipAddress = Dns.GetHostAddresses(host).First();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
