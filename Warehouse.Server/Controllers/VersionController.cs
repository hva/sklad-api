using System.Diagnostics;
using System.Reflection;
using System.Web.Http;

namespace Warehouse.Server.Controllers
{
    [RoutePrefix("api/version")]
    public class VersionController : ApiController
    {
        [Route]
        public IHttpActionResult Get()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var info = FileVersionInfo.GetVersionInfo(assembly.Location);
            var data = new
            {
                version = info.FileVersion,
                commit = info.ProductVersion,
            };

            return Ok(data);
        }
    }
}
