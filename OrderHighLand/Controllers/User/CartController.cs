using Microsoft.AspNetCore.Mvc;

namespace OrderHighLand.Controllers.User
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
