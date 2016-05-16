using System.Web.Mvc;

namespace Warehouse.Server.Controllers
{
    [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}