using System.IO;
using Nancy;
using Nancy.TinyIoc;
using Newtonsoft.Json;

namespace SkladApi.Nancy
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            var json = File.ReadAllText("config.json");
            var config = JsonConvert.DeserializeObject<Config>(json);
            container.Register(config);
        }
    }
}
