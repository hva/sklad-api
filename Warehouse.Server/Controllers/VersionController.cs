using System.Configuration;
using System.Web.Http;
using Warehouse.Server.Logger;

namespace Warehouse.Server.Controllers
{
    [RoutePrefix("api/version")]
    public class VersionController : ApiController
    {
        private readonly ILogger logger;

        public VersionController(ILogger logger)
        {
            this.logger = logger;
        }

        [Route]
        public IHttpActionResult Get()
        {
            var version = ConfigurationManager.AppSettings["Version"];
            var commit = ConfigurationManager.AppSettings["Commit"];
            var data = new { version, commit };
            logger.TrackRequest(Request, true);
            return Ok(data);
        }
    }
}
