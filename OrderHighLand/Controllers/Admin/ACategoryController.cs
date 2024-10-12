using Microsoft.AspNetCore.Mvc;
using OrderHighLand.Service;

namespace OrderHighLand.Controllers.Admin
{
	public class ACategoryController : Controller
	{
		private readonly CategoryService _categoryService;
		public ACategoryController(CategoryService categoryService)
        {
			_categoryService = categoryService;
		}
        public async Task<IActionResult> Index()
		{
			var categories = await _categoryService.GetAllAsync();
			return View(categories);
		}

		public async Task<IActionResult> Create()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(Models.Category category)
		{
			var categories = await _categoryService.CreateAsync(category);
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Edit(int id)
		{
			var category = await _categoryService.GetByIdAsync(id);
			return View(category);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(int id, Models.Category category)
		{
			var categories = await _categoryService.UpdateAsync(id,category);
			return Json(new { status = "success", message = "Category updated successfully!" });
		}

		public async Task<IActionResult> Delete(int id)
		{
			await _categoryService.DeleteAsync(id);
			return Json(new { status = "success", message = "Category deleted successfully!" });
		}
	}
}
