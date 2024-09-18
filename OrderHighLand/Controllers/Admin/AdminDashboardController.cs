using Microsoft.AspNetCore.Mvc;
using OrderHighLand.Controllers.User;
using OrderHighLand.Service;

namespace OrderHighLand.Controllers.Admin
{
	public class AdminDashboardController : Controller
    {

		public IActionResult Index()
		{
			return View();
		}
		private readonly ILogger<HomeController> _logger;

		public AdminDashboardController(ILogger<HomeController> logger)
		{
			_logger = logger;
			
		}

		
    }
}
