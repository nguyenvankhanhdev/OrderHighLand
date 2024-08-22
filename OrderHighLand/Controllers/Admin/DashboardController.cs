using Microsoft.AspNetCore.Mvc;

namespace OrderHighLand.Controllers.Admin
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        { 
            

            return View();
           
        }
    }
}
