using Nancy;

namespace SkladApi.Nancy.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule(Config config)
        {
            Get["/"] = parameters => Response.AsJson(new
            {
                config.Version,
                Build = config.Commit
            });
        }
    }
}
