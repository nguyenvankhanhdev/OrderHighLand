using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrderHighLand.Models;
using OrderHighLand.Models.VM;
using OrderHighLand.Service;

namespace OrderHighLand.Controllers.Admin
{
	public class AProductVariantController : Controller
	{
		private readonly ProductService _productService;
		private readonly ProductVartiantService _vartiantService;
		private readonly SizeService _sizeService;
		public AProductVariantController(ProductVartiantService vartiantService, SizeService sizeService, ProductService productService)
		{
			_vartiantService = vartiantService;
			_sizeService = sizeService;
			_productService = productService;

		}
		public async Task<IActionResult> Index(int proId)
		{
			var product = await _productService.GetByIdAsync(proId);
			var size = await _sizeService.GetAllAsync();
			var variant = await _vartiantService.GetAllAsync(proId);
			var model = new VariantSizeVM
			{
				products = new List<Products> { product },
				sizes = size,
				productVariants = variant,
			};
			return View(model);
		}

		public async Task<IActionResult> Create(int proID)
		{
			VariantCreateVM variantCreateVM = new();
			var size = await _sizeService.GetAllAsync();
			var product = await _productService.GetByIdAsync(proID);
			variantCreateVM.sizes = size.Select(s => new SelectListItem
			{
				Value = s.Id.ToString(),
				Text = s.Size
			}).ToList();
			variantCreateVM.products = product;
			return View(variantCreateVM);
		}
		[HttpPost]
		public async Task<IActionResult> Create(ProductVariant productVariant)
		{
			await _vartiantService.CreateAsync(productVariant);
			return Json(new { status = "success", message = "Variant created successfully!" });
		}
		public async Task<IActionResult> Edit(int id)
		{
			var size = await _sizeService.GetAllAsync();
			var variant = await _vartiantService.GetByIdAsync(id);
			VariantCreateVM variantCreateVM = new VariantCreateVM
			{
				productVariant = variant,
				sizes = size.Select(s => new SelectListItem
				{
					Value = s.Id.ToString(),
					Text = s.Size
				}).ToList(),
				products = await _productService.GetByIdAsync(variant.Pro_Id)
			};

			return View(variantCreateVM);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(int id, ProductVariant productVariant)
		{
			await _vartiantService.UpdateAsync(productVariant, id);
			return Json(new { status = "success", message = "Variant updated successfully!" });
		}


		public async Task<IActionResult> Delete(int id)
		{
			await _vartiantService.DeleteAsync(id);
			return Json(new { status = "success", message = "Variant deleted successfully!" });
		}




	}
}
