using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using OrderHighLand.Models;
using OrderHighLand.Service;
using System.Diagnostics;

namespace OrderHighLand.Controllers.User
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ConnecNeo4J _connecNeo4J;

        public HomeController(ILogger<HomeController> logger, ConnecNeo4J neo4J)
        {
            _logger = logger;
            _connecNeo4J = neo4J;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
