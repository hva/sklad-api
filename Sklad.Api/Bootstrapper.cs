using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Sklad.Api
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register(new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            var req = context.Request;
            Log.Information("request: {method} {uri} {userHostAddress}", req.Method, req.Url, req.UserHostAddress);
            base.RequestStartup(container, pipelines, context);
        }
    }
}
