using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using OrderHighLand.Models;
using OrderHighLand.Service;

namespace OrderHighLand.Controllers.Admin
{
	public class AProductController : Controller
	{


		private readonly ProductService _productService;
		public AProductController(ProductService productService)
		{
			_productService = productService;
		}

		public async Task<IActionResult> GetAllProducts()
		{
			List<Products> products = new();
			products = await _productService.GetAllAsync();
			return View(products);
		}

		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Create(Products product)
		{

			return RedirectToAction("Index");
		}


	}
}
