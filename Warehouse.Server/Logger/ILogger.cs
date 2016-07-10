using System.Net.Http;

namespace Warehouse.Server.Logger
{
    public interface ILogger
    {
        void Error(string message);
        void Info(string message);
        void TrackRequest(HttpRequestMessage request, bool success);
    }
}