using Microsoft.AspNetCore.Mvc;
using OrderHighLand.Controllers.User;
using OrderHighLand.Service;

namespace OrderHighLand.Controllers.Admin
{
    public class AdminDashboardController : Controller
    {
<<<<<<< Updated upstream
        public IActionResult Index()
        {
            return View();
        }
=======
		private readonly ILogger<HomeController> _logger;
		private readonly ConnecNeo4J _connecNeo4J;

		public AdminDashboardController(ILogger<HomeController> logger, ConnecNeo4J neo4J)
		{
			_logger = logger;
			_connecNeo4J = neo4J;
		}

		public async Task<IActionResult> Index()
		{
			var customers = await _connecNeo4J.GetAllCustomersAsync();
			ViewBag.Customers = customers;
			return View();
		}
>>>>>>> Stashed changes
    }
}
