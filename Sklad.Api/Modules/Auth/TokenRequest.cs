using Newtonsoft.Json;

namespace Sklad.Api.Modules.Auth
{
    public class TokenRequest
    {
        [JsonProperty(PropertyName = "grant_type")]
        public string GrantType;
    }
}
