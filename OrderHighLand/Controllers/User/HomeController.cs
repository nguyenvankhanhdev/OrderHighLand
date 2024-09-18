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


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
       
        public IActionResult Privacy()
        {
            return View();
        }

       
    }
}
