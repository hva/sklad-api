using System.Configuration;
using Microsoft.ApplicationInsights;

namespace Warehouse.Server.Logger
{
    public class TelemetryLogger : ILogger
    {
        private readonly TelemetryClient telemetry;

        public TelemetryLogger()
        {
            var instrumentationKey = ConfigurationManager.AppSettings["InstrumentationKey"];
            telemetry = new TelemetryClient { InstrumentationKey = instrumentationKey };
        }

        public void TrackTrace(string message)
        {
            telemetry.TrackTrace(message);
        }
    }
}