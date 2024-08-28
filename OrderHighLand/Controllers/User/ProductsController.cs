using Microsoft.AspNetCore.Mvc;

namespace OrderHighLand.Controllers.User
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}
