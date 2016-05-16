using MongoDB.Bson.Serialization.Attributes;

namespace Warehouse.Server.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonElement("UserName")]
        public string UserName { get; set; }

        [BsonElement("Roles")]
        public string[] Roles { get; set; }

        public string Password { get; set; }
    }
}
