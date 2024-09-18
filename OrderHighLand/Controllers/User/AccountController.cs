using Microsoft.AspNetCore.Mvc;

namespace OrderHighLand.Controllers.User
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
