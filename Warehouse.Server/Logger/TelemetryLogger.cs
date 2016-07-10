using System.Net.Http;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Warehouse.Server.Logger
{
    public class TelemetryLogger : ILogger
    {
        private readonly TelemetryClient telemetry;

        public TelemetryLogger(string instrumentationKey)
        {
            telemetry = new TelemetryClient { InstrumentationKey = instrumentationKey };
        }

        public void Error(string message)
        {
            telemetry.TrackTrace(message, SeverityLevel.Error);
        }

        public void Info(string message)
        {
            telemetry.TrackTrace(message, SeverityLevel.Information);
        }

        public void TrackRequest(HttpRequestMessage r, bool ok)
        {
            var tr = new RequestTelemetry
            {
                Name = r.RequestUri.AbsolutePath,
                HttpMethod = r.Method.Method,
                Success = ok,
            };

            telemetry.TrackRequest(tr);
        }
    }
}