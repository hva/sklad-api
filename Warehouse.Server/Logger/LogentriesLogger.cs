using System.IO;
using System.Web.Hosting;
using Serilog;

namespace Warehouse.Server.Logger
{
    public class LogentriesLogger : ILogger
    {
        private readonly Serilog.Core.Logger logger;

        public LogentriesLogger(string token)
        {
            var configuration = new LoggerConfiguration();
            if (string.IsNullOrEmpty(token))
            {
                var pathFormat = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "log.txt");
                configuration.WriteTo.File(pathFormat);
            }
            else
            {
                configuration.WriteTo.Logentries(token);
            }
            logger = configuration.CreateLogger();
        }

        public void Info(string message)
        {
            logger.Information(message);
        }

        public void Error(string message)
        {
            logger.Error(message);
        }
    }
}