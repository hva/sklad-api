using System;
using System.Diagnostics;
using System.Reflection;
using Nancy;
using Serilog;
using Resp = System.Tuple<string,string>;

namespace Sklad.Api.Modules
{
    public class HomeModule : NancyModule
    {
        private static readonly Lazy<Resp> resp = new Lazy<Resp>(Load);

        public HomeModule()
        {
            Get["/"] = parameters => Response.AsJson(new
            {
                Version = resp.Value.Item1,
                Commit = resp.Value.Item2,
            });
        }

        private static Resp Load()
        {
            Log.Information("Loading assembly info.");
            var assembly = Assembly.GetExecutingAssembly();
            var info = FileVersionInfo.GetVersionInfo(assembly.Location);
            var commit = info.ProductVersion;
            if (commit.Length > 7)
            {
                commit = commit.Substring(0, 7);
            }
            return Tuple.Create(info.FileVersion, commit);
        }
    }
}
