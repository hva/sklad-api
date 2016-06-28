using System.Configuration;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Warehouse.Server.Logger
{
    public class TelemetryLogger : ILogger
    {
        private readonly TelemetryClient telemetry;

        public TelemetryLogger()
        {
            var instrumentationKey = ConfigurationManager.AppSettings["InstrumentationKey"];
            telemetry = new TelemetryClient
            {
                InstrumentationKey = instrumentationKey
            };
        }

        public void Error(string message)
        {
            telemetry.TrackTrace(message, SeverityLevel.Error);
        }

        public void Info(string message)
        {
            telemetry.TrackTrace(message, SeverityLevel.Information);
        }
    }
}