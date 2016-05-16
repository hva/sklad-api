using MongoDB.Driver;
using Warehouse.Server.Models;

namespace Warehouse.Server.Data
{
    public interface IMongoContext
    {
        MongoDatabase Database { get; }
        MongoCollection<Product> Products { get; }
        MongoCollection<User> Users { get; }
    }
}
