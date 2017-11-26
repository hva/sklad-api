using Nancy;

namespace Sklad.Api.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = parameters => Response.AsJson(new
            {
                Version = "1.0",
                Commit = "f5bae0a"
            });
        }
    }
}
