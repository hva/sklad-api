using System.Diagnostics;
using System.Reflection;
using Nancy;

namespace Sklad.Api.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = parameters =>
            {
                var assembly = Assembly.GetExecutingAssembly();
                var info = FileVersionInfo.GetVersionInfo(assembly.Location);
                return Response.AsJson(new
                {
                    Version = info.FileVersion,
                    Commit = info.ProductVersion
                });
            };
        }
    }
}
