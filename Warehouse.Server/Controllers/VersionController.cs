using System;
using System.IO;
using System.Web.Hosting;
using System.Web.Http;

namespace Warehouse.Server.Controllers
{
    public class VersionController : ApiController
    {
        public IHttpActionResult Get()
        {
            var path = HostingEnvironment.MapPath("~/COMMIT");
            if (string.IsNullOrEmpty(path))
            {
                return InternalServerError();
            }

            try
            {
                using (var sr = new StreamReader(path))
                {
                    var version = sr.ReadLine();
                    var commit = sr.ReadLine();
                    var data = new { version, commit };
                    return Ok(data);
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}
