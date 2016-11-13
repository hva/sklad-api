using System.Configuration;
using System.Web.Http;

namespace Warehouse.Server.Controllers
{
    [RoutePrefix("api/version")]
    public class VersionController : ApiController
    {
        [Route]
        public IHttpActionResult Get()
        {
            var version = ConfigurationManager.AppSettings["Version"];
            var commit = ConfigurationManager.AppSettings["Commit"];
            var data = new { version, commit };
            return Ok(data);
        }
    }
}
