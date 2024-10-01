using Microsoft.AspNetCore.Mvc;
using OrderHighLand.Service;
using OrderHighLand.Models;
using Neo4j.Driver;

namespace OrderHighLand.Controllers.Admin
{
	public class ASizeController : Controller
	{
		private readonly IDriver _driver;
		private readonly SizeService _sizeService;
		public ASizeController(IDriver driver, SizeService sizeService)
		{
			_driver = driver;
			_sizeService = sizeService;
		}
		public async Task<IActionResult> Index()
		{
			var sizes = await _sizeService.GetAllAsync();
			return View(sizes);
		}

		public async Task<IActionResult> Edit(int id)
		{
			var size = await _sizeService.GetByIdAsync(id);
			return View(size);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(Sizes size, int id)
		{
			await _sizeService.UpdateAsync(size, id);
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Create()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(Sizes size)
		{
			await _sizeService.CreateAsync(size);
			return Json(new { status = "success", message = "Size created successfully!" });

		}

		public async Task<IActionResult> Delete(int id)
		{
			await _sizeService.DeleteAsync(id);
			return Json(new { status = "success", message = "Size deleted successfully!" });
		}

	}
}
