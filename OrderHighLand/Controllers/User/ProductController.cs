using Microsoft.AspNetCore.Mvc;
using OrderHighLand.Models;
using OrderHighLand.Service;

namespace OrderHighLand.Controllers.User
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly ConnecNeo4J connecNeo4J;
        private readonly ProductService proService;
        private readonly CategoryService categoryService;
        private readonly ProductVariantService productVariantService;
        private readonly ToppingService toppingService;
        public ProductController(ProductService productService, ToppingService _topping, ConnecNeo4J _Connect, ProductService _proService, CategoryService _categoryService, ProductVariantService _productVariantService)
        {
            _productService = productService;
            connecNeo4J = _Connect;
            proService = _proService;
            categoryService = _categoryService;
            productVariantService = _productVariantService;
            toppingService = _topping;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Route("Product/Detail/{slug}")]
        public async Task<IActionResult> Detail(string slug)
        {
           
            var product = await _productService.getProductBySlug(slug);
            var allTopping = await toppingService.getAllTopping();
            var allSize = await connecNeo4J.getAllSize();
            var cateCount = await categoryService.countAllCate();
            var productVariants = await productVariantService.getAllProductVariants();
            

          
            var provarSize = from pv in productVariants
                             join s in allSize on pv.Size_Id equals s.Id
                             select new
                             {
                                 ProductVariant = pv,
                                 Size = s
                             };
            var combined = from pv in productVariants
                           join p in product on pv.Pro_Id equals p.Id
                           where pv.Size_Id == 1 // Lọc các ProductVariant có Size_Id = 1
                           select new { Product = p, ProductVariant = pv };
            ViewBag.Toppings = allTopping;
            ViewBag.ProSize = provarSize;
            ViewBag.ProductVariant = productVariants;
            ViewBag.CateCount = cateCount;
            ViewBag.ProductVariantsWithProducts = combined.ToList();

            return View();
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
