using System.Diagnostics;
using MySql.Data.MySqlClient;
using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace SkladApi
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            var config = Config.Load();
            container.Register(config);

            if (config.Db.Logging)
            {
                MySqlTrace.Listeners.Add(new MySqlTraceListener());
                MySqlTrace.Switch.Level = SourceLevels.All;
            }

            
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            //var statelessAuthConfiguration =
            //    new StatelessAuthenticationConfiguration(ctx =>
            //    {
            //        if (!ctx.Request.Query.apikey.HasValue)
            //        {
            //            return null;
            //        }

            //var userValidator =
            //                container.Resolve<IUserApiMapper>();

            //            return userValidator.GetUserFromAccessToken(ctx.Request.Query.apikey);
            //    });
            //StatelessAuthentication.Enable(pipelines, statelessAuthConfiguration);
        }
    }
}