using Microsoft.AspNetCore.Mvc;

namespace OrderHighLand.Controllers.Admin
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            // tôi muốn return layout adminlayout
           
            return View();

        }
    }
}
