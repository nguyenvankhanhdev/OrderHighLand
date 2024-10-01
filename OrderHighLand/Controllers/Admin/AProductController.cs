using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrderHighLand.Models;
using OrderHighLand.Models.VM;
using OrderHighLand.Service;
namespace OrderHighLand.Controllers.Admin
{
	public class AProductController : Controller
	{
		private readonly CategoryService _categoryService;
		private readonly ProductService _productService;
		public AProductController(ProductService productService, CategoryService categoryService)
		{
			_productService = productService;
			_categoryService = categoryService;
		}

		public async Task<IActionResult> Index()
		{
			var products = await _productService.GetAllAsync();
			var categories = await _categoryService.GetAllAsync();
			var model = new ProductCategoryVM
			{
				Products = products,
				Categories = categories
			};
			return View(model);
		}

		public async Task<IActionResult> Create()
		{
			ProductCreateVM viewModel = new();

			var cate = await _categoryService.GetAllAsync();
			if (cate != null)
			{
				viewModel.categories = cate.Select(c => new SelectListItem
				{
					Value = c.Id.ToString(),
					Text = c.Name
				}).ToList();
			}
			return View(viewModel);

		}
		[HttpPost]
		public async Task<IActionResult> Create(ProductCreateVM viewModel, IFormFile image)
		{
			if (image != null && image.Length > 0)
			{
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imgs", image.FileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await image.CopyToAsync(stream);
				}
				viewModel.products.Image = "/imgs/" + image.FileName;
			}
			await _productService.CreateAsync(viewModel.products);
			return RedirectToAction("Index");
		}
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            var cate = await _categoryService.GetAllAsync();
            ProductCreateVM viewModel = new()
            {
                products = product,
                categories = cate.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ProductCreateVM viewModel, IFormFile image, int id)
        {
            if (image != null && image.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imgs", image.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                viewModel.products.Image = "/imgs/" + image.FileName;
            }
			else
			{
				viewModel.products.Image = viewModel.products.Image;
			}
            await _productService.UpdateAsync(id, viewModel.products);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
		{
			await _productService.DeleteAsync(id);
			return Json(new
			{
				status = "success",
				message = "Deleted Successfully"
			});
		}
	}
}
