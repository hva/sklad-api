namespace SkladApi
{
    public class Config
    {
        public Db Db { get; set; }
        public string Version { get; set; }
        public string Commit { get; set; }
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
