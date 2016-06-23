using System.Configuration;
using MongoDB.Driver;
using Warehouse.Server.Models;

namespace Warehouse.Server.Data
{
    public class MongoContext : IMongoContext
    {
        private readonly MongoDatabase database;

        public MongoContext()
        {
            var connectionString = ConfigurationManager.AppSettings["MongoDB"];
            var url = new MongoUrl(connectionString);
            var client = new MongoClient(url);
            var server = client.GetServer();
            database = server.GetDatabase(url.DatabaseName);
        }

        public MongoDatabase Database { get { return database; } }

        public MongoCollection<Product> Products
        {
            get { return database.GetCollection<Product>("products"); }
        }

        public MongoCollection<User> Users
        {
            get { return database.GetCollection<User>("users"); }
        }
    }
}
