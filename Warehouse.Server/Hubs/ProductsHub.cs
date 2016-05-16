using System.Collections.Generic;
using Microsoft.AspNet.SignalR;

namespace Warehouse.Server.Hubs
{
    public class ProductsHub : Hub<IClient>
    {
        public void RaiseProductUpdated(string id)
        {
            Clients.Others.OnProductUpdated(id);
        }

        public void RaiseProductUpdatedBatch(List<string> ids)
        {
            Clients.Others.OnProductUpdatedBatch(ids);
        }

        public void RaiseProductDeletedBatch(List<string> ids)
        {
            Clients.Others.OnProductDeletedBatch(ids);
        }
    }

    public interface IClient
    {
        void OnProductUpdated(string id);
        void OnProductUpdatedBatch(List<string> ids);
        void OnProductDeletedBatch(List<string> ids);
    }
}