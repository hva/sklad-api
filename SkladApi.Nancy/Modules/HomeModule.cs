using Nancy;
using SkladApi.Nancy.Infrastructure;

namespace SkladApi.Nancy.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = parameters => Response.AsJson(Config.Load());
        }
    }
}
