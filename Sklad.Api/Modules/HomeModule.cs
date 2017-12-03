using System;
using System.Diagnostics;
using System.Reflection;
using Nancy;
using Serilog;

namespace Sklad.Api.Modules
{
    public class HomeModule : NancyModule
    {
        private static readonly Lazy<FileVersionInfo> infoLazy = new Lazy<FileVersionInfo>(LoadInfo);

        public HomeModule()
        {
            Get["/"] = parameters =>
            {
                var info = infoLazy.Value;
                return Response.AsJson(new
                {
                    Version = info.FileVersion,
                    Commit = info.ProductVersion.Substring(0, 7)
                });
            };
        }

        private static FileVersionInfo LoadInfo()
        {
            Log.Information("Loading assembly info.");
            var assembly = Assembly.GetExecutingAssembly();
            return FileVersionInfo.GetVersionInfo(assembly.Location);
        }
    }
}
