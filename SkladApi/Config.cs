using System.IO;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace SkladApi
{
    public class Config
    {
        public int Port { get; set; }
        public Db Db { get; set; }
        public string Version { get; set; }
        public string Commit { get; set; }

        public static Config Load()
        {
            var json = File.ReadAllText("config.json");
            return JsonConvert.DeserializeObject<Config>(json);
        }

        public string GetConnectionString()
        {
            var sb = new MySqlConnectionStringBuilder
            {
                Server = Db.Server,
                Port = Db.Port,
                Database = Db.Database,
                UserID = Db.User,
                Password = Db.Password,
                Logging = Db.Logging,
            };

            return sb.GetConnectionString(true);
        }
    }

    public class Db
    {
        public string Server { get; set; }
        public uint Port { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public bool Logging{ get; set; }
    }
}
