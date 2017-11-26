using System.IO;
using System.Runtime.Serialization.Json;

namespace Sklad.Api.Configuration
{
    public class ConfigLoader
    {
        public static ApiConfig Load()
        {
            using (var stream = File.OpenRead("config.json"))
            {
                var serializer = new DataContractJsonSerializer(typeof(ApiConfig));
                return (ApiConfig)serializer.ReadObject(stream);
            }
        }
    }
}

