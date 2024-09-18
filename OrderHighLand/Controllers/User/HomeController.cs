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
        private ConnecNeo4J _connecNeo4J;

        public HomeController(ILogger<HomeController> logger, ConnecNeo4J connecNeo4J)
        {
            _logger = logger;
            _connecNeo4J = connecNeo4J;
        }

        public async Task<IActionResult> Index()
        {
            var products =await _connecNeo4J.getAllProducts();
            ViewBag.Products = products;    
            return View();
        }
       
        public IActionResult Privacy()
        {
            return View();
        }

       
    }
}
