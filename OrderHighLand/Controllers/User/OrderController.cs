using Microsoft.AspNetCore.Mvc;
using OrderHighLand.Service;

namespace OrderHighLand.Controllers.User
{
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }
        public async Task<IActionResult> OrderDetails(int orderId)
        {
            var orderDetails = await _orderService.GetOrderDetailsByOrderIdAsync(orderId);

            if (orderDetails == null)
            {
                return RedirectToAction("Index", "Account");
            }

            return View(orderDetails);
        }
    }
}
