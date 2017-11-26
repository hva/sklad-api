using System.Runtime.Serialization;

namespace Sklad.Api.Configuration
{
    [DataContract]
    public class ApiConfig
    {
        [DataMember(Name = "port")]
        public int Port { get; set; }

        [DataMember(Name = "service_name")]
        public string ServiceName { get; set; }

        [DataMember(Name = "log")]
        public Logging Log { get; set; }

        [DataContract]
        public class Logging
        {
            [DataMember(Name = "host")]
            public string Host { get; set; }

            [DataMember(Name = "port")]
            public int Port { get; set; }
        }
    }
}
