using Microsoft.AspNetCore.Mvc;

namespace TechYardHub.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
