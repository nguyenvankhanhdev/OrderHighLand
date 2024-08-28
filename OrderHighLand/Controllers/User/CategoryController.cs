using Microsoft.AspNetCore.Mvc;

namespace OrderHighLand.Controllers.User
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
