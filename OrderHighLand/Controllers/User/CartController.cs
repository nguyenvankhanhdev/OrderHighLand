using Microsoft.AspNetCore.Mvc;
using OrderHighLand.Models;
using OrderHighLand.Service;

namespace OrderHighLand.Controllers.User
{
    public class ProductOrder
    {
        public int Provar_Id { get; set; }
        public List<int> Topping_Ids { get; set; }
        public int Quantity { get; set; }
        public float TotalPrice { get; set; }
    }
    public class CartController : Controller
    {
        private readonly ProductVariantService productVariantService;
        private readonly CartService cartService;
        private readonly ToppingService toppingService;
        private readonly SizeService sizeService;
        private readonly ProductService productService;
        private readonly AddressService addressService;
        private readonly OrderService orderService;
        public CartController(ProductVariantService _productVariantService,OrderService _orderService, AddressService _addressService, ProductService _productService ,CartService _cartService,ToppingService _toppingService,SizeService _sizeService) 
        { 
            this.productVariantService = _productVariantService;
            this.cartService = _cartService;
            this.toppingService = _toppingService;
            this.sizeService = _sizeService;
            this.productService = _productService;
            this.addressService = _addressService;
            this.orderService = _orderService;
        }
        public async  Task<IActionResult> Index()
        {
            ViewBag.CheckLogin = 1;
            var userId = HttpContext.Session.GetString("UserId");
            int countCart =await cartService.countCartByAId(int.Parse(userId));
            ViewBag.CountCart=countCart;
            // Kiểm tra nếu userId không tồn tại (người dùng chưa đăng nhập)
            if (string.IsNullOrEmpty(userId))
            {
                // Trả về thông báo yêu cầu đăng nhập
                ViewBag.CheckLogin = 0;
                return View();
            }
            var products=await productService.GetAllAsync();
            var toppings = await toppingService.getAllTopping();
            var productVariants=await productVariantService.getAllProductVariants();
            var sizes =await sizeService.GetAllAsync();
            int A_Id = int.Parse(userId);
            var carts =await cartService.getAllCartById(A_Id);
            
            var cartDetails = from cart in carts
                              join productVariant in productVariants on cart.Provar_Id equals productVariant.Id
                              join size in sizes on productVariant.Size_Id equals size.Id
                              join product in products on productVariant.Pro_Id equals product.Id
                              select new
                              {
                                  Cart = cart,
                                  ProductVariant = productVariant,
                                  Size = size,
                                  Product= product,
                              };
            ViewBag.Carts = carts;
            ViewBag.productVariants=productVariants;
            ViewBag.sizes = sizes;
            ViewBag.toppings=toppings;
            ViewBag.CartDetails = cartDetails;

            return View();
        }
        public IActionResult CheckOut()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> addItemToCart(int productId,float price ,int sizeId, List<int> toppings, int quantity)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var cartId = await cartService.getCartIdMax() + 1;

            // Kiểm tra nếu userId không tồn tại (người dùng chưa đăng nhập)
            if (string.IsNullOrEmpty(userId))
            {
                // Trả về chỉ thị chuyển hướng
                return Json(new { success = false, redirect = true, redirectUrl = Url.Action("Index", "Account"), message = "Bạn cần đăng nhập để thực hiện mua hàng." });
            }


            var Provar_Id = await productVariantService.getProvarByProIdAndSizeId(productId, sizeId);
            if(Provar_Id==-1)
            {
                return Json(new { success = false, message = "Mã sản phẩm không có, đang bị lỗi." });
            }    
            var cartItem = new Cart
            {
                Id = cartId,
                Provar_Id = Provar_Id,
                A_Id = int.Parse(userId),
                Quantity = quantity,
                TotalPrice = price*quantity,
                Topping_Id = toppings
            };

            await cartService.addToCart(cartItem); // Đảm bảo rằng bạn đang gọi phương thức bất đồng bộ
            return Json(new { success = true, message = "Sản phẩm đã được thêm vào giỏ hàng." });
        }

        public async Task<IActionResult> removeCart(int Cart_Id)
        {
            int checkRemove = await cartService.removeFromCartById(Cart_Id);

            if (checkRemove == 1)
            {
                return Json(new { success = true, message = "Xóa thành công sản phẩm khỏi giỏ hàng." });
            }
            else
            {
                return Json(new { success = false, message = "Xóa sản phẩm không thành công." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = HttpContext.Session.GetString("UserId");
            int noLoginCount = 0;
            if(string.IsNullOrEmpty(userId))
            {
                return Json(new { success = true, noLoginCount });
            }    
            int count = await cartService.countCartByAId(int.Parse(userId));

            return Json(new { success = true, count });
        }

        [HttpPost]
        public async Task<IActionResult> placeOrder(string address, string district,string ward,string phone, float cartPrice, List<ProductOrder> products)
        {
            var userId = HttpContext.Session.GetString("UserId");
            int A_Id = int.Parse(userId);
            int Address_Id= await addressService.getAddressIdMax() + 1;
            var addresses = new Addresses
            {
                Id= Address_Id,
                Address= address,
                A_ID= int.Parse(userId),
                District= district,
                Ward=ward,
                Phone=phone
            };
            await addressService.addToAddress(addresses);
            int Order_Id = await orderService.getMaxIdOrder()+1;
            var order = new Orders
            {
                Id = Order_Id,
                Date = DateTime.Now,
                Status = "Pending",
                A_Id = A_Id,
                TotalPrice = cartPrice,
                Address_Id = Address_Id,
            };
            await orderService.addToOrder(order);

            foreach (var product in products)
            {
                // Truy cập thông tin từng sản phẩm
                int provarId = product.Provar_Id;
                List<int> toppingIds = product.Topping_Ids;
                int quantity = product.Quantity;
                float totalPrice = product.TotalPrice;

                int orderDetail_Id = await orderService.getMaxIdOrderDetail() + 1;
                var orderDetail = new OrderDetails
                {
                    Id = orderDetail_Id,
                    Price = totalPrice,
                    Topping_Id = toppingIds,
                    Quantity = quantity,
                    Provar_Id = provarId,
                    Order_Id=Order_Id
                };
                await orderService.addToOrderDetail(orderDetail);
            }
            await cartService.removeAllCartByAId(A_Id);
            return Json(new { success = true, message = "Đặt đơn hàng thành công" });

        }
    }
}
