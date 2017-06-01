using Nancy;

namespace SkladApi.Nancy.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = parameters => "home module";
        }
    }
}
