using System.Runtime.Serialization;

namespace Sklad.Api.Configuration
{
    [DataContract]
    public class ApiConfig
    {
        [DataMember(Name = "port")]
        public int Port { get; set; }

        public string Db { get; set; }
        public string Version { get; set; }
        public string Commit { get; set; }
    }
}
