using System.Diagnostics;
using System.IO;
using MySql.Data.MySqlClient;
using Nancy;
using Nancy.TinyIoc;
using Newtonsoft.Json;

namespace SkladApi
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            var json = File.ReadAllText("config.json");
            var config = JsonConvert.DeserializeObject<Config>(json);
            container.Register(config);

            if (config.Db.Logging)
            {
                MySqlTrace.Listeners.Add(new MySqlTraceListener());
                MySqlTrace.Switch.Level = SourceLevels.All;
            }
        }
    }
}