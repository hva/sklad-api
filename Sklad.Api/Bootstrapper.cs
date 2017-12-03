using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Serilog;

namespace Sklad.Api
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            var req = context.Request;
            Log.Information("request: {method} {uri} {userHostAddress}", req.Method, req.Url, req.UserHostAddress);
            base.RequestStartup(container, pipelines, context);
        }
    }
}
