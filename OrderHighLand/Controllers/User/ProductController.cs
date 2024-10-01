using Microsoft.AspNetCore.Mvc;
using OrderHighLand.Service;

namespace OrderHighLand.Controllers.User
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Route("Product/Detail/{slug}")]
        public async Task<IActionResult> Detail(string slug)
        {
            

            var product = await _productService.GetProductBySlug(slug);
            if (product == null)
            {
                return NotFound(); 
            }

            return View(product);
        }
        public IActionResult categoryProduct()
        {
            return View();
        }
        public IActionResult categoryProduct2()
        {
            return View();
        }
    }
}
