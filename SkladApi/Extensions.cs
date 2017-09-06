using MySql.Data.MySqlClient;

namespace SkladApi
{
    public static class Extensions
    {
        public static string GetConnectionString(this Config config)
        {
            var sb = new MySqlConnectionStringBuilder
            {
                Server = config.Db.Server,
                Port = config.Db.Port,
                Database = config.Db.Database,
                UserID = config.Db.User,
                Password = config.Db.Password,
                Logging = config.Db.Logging,
            };

            return sb.GetConnectionString(true);
        }
    }
}
