using System.IO;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace SkladApi.Nancy.Infrastructure
{
    public class Config
    {
        public Db Db { get; set; }

        public string ConnectionString { get; private set; }

        public static Config Load()
        {
            using (var file = File.OpenText("config.json"))
            {
                var serializer = new JsonSerializer();
                var config = (Config)serializer.Deserialize(file, typeof(Config));

                var sb = new MySqlConnectionStringBuilder
                {
                    Server = config.Db.Server,
                    Port = config.Db.Port,
                    Database = config.Db.Database,
                    UserID = config.Db.User,
                    Password = config.Db.Password,
                };

                config.ConnectionString = sb.GetConnectionString(true);

                return config;
            }
        }
    }

    public class Db
    {
        public string Server { get; set; }
        public uint Port { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
