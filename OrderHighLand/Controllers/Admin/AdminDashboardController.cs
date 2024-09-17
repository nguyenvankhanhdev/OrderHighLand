using Microsoft.AspNetCore.Mvc;

namespace OrderHighLand.Controllers.Admin
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
