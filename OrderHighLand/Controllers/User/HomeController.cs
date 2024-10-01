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
        private readonly ProductService service;

        public HomeController(ILogger<HomeController> logger, ProductService _service)
        {
            _logger = logger;
            service=_service;
        }

        public async Task<IActionResult> Index()
        {
            var hotProducts= await service.getFourHotProduct();
            var newProducts =await service.getTwoNewProduct();
            ViewBag.NewProducts = newProducts;    
            ViewBag.HotProducts = hotProducts;
            return View();
        }
       
        public IActionResult Privacy()
        {
            return View();
        }

       
    }
}
