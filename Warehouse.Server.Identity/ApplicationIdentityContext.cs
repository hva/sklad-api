using AspNet.Identity.MongoDB;
using Warehouse.Server.Data;

namespace Warehouse.Server.Identity
{
    public class ApplicationIdentityContext : IdentityContext
    {
        public ApplicationIdentityContext(IMongoContext context)
        {
            Users = context.Database.GetCollection("users");
        }
    }
}