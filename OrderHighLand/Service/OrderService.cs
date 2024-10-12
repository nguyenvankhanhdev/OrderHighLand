using Neo4j.Driver;
using OrderHighLand.Models;

namespace OrderHighLand.Service
{
    public class OrderService
    {
        private readonly IDriver _driver;
        public OrderService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<int> getMaxIdOrder()
        {
            string query = @"
                    MATCH (c:Order)
                    RETURN COALESCE(MAX(c.Id), 0) AS maxId
                ";

            try
            {
                var session = _driver.AsyncSession();
                var result = await session.RunAsync(query);

                var maxId = await result.SingleAsync(record => record["maxId"].As<int>());
                return maxId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0; // Trả về 0 nếu xảy ra lỗi
            }
        }

        public async Task<int> getMaxIdOrderDetail()
        {
            string query = @"
                    MATCH (c:OrderDetail)
                    RETURN COALESCE(MAX(c.Id), 0) AS maxId
                ";

            try
            {
                var session = _driver.AsyncSession();
                var result = await session.RunAsync(query);

                var maxId = await result.SingleAsync(record => record["maxId"].As<int>());
                return maxId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0; // Trả về 0 nếu xảy ra lỗi
            }
        }

        public async Task addToOrder(Orders order)
        {
            string query = @"
                    CREATE (o:Order {
                        Id: $Id,
                        Date: $Date,
                        Status: $Status,
                        TotalPrice: $TotalPrice,
                        Address_Id: $Address_Id,
                        A_Id: $A_Id
                    })";

            try
            {
                var session = _driver.AsyncSession();
                var parameters = new Dictionary<string, object>
                {
                    { "Id", order.Id },
                    { "Date", order.Date.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "Status", order.Status },
                    { "TotalPrice", order.TotalPrice },
                    { "Address_Id", order.Address_Id },
                    { "A_Id", order.A_Id }
                };

                // Thực thi câu lệnh Cypher
                await session.RunAsync(query, parameters);
                await session.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public async Task addToOrderDetail(OrderDetails orderDetail)
        {
            string query = @"
                    CREATE (od:OrderDetail {
                        Id: $Id,
                        Quantity: $Quantity,
                        Price: $Price,
                        Order_Id: $Order_Id,
                        Provar_Id: $Provar_Id,
                        Topping_Id: $Topping_Id
                    })";

            try
            {
                var session = _driver.AsyncSession();

                // Tạo dictionary chứa các tham số
                var parameters = new Dictionary<string, object>
                {
                    { "Id", orderDetail.Id },
                    { "Quantity", orderDetail.Quantity },
                    { "Price", orderDetail.Price },
                    { "Order_Id", orderDetail.Order_Id },
                    { "Provar_Id", orderDetail.Provar_Id },
                    { "Topping_Id", orderDetail.Topping_Id }
                };

                // Thực thi câu lệnh Cypher
                await session.RunAsync(query, parameters);
                await session.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        public async Task<List<Orders>> GetOrdersByAccountId(int accountId)
        {
            string query = @"
        MATCH (o:Order)
        WHERE o.A_Id = $A_Id
        RETURN o.Id AS Id, o.Date AS Date, o.Status AS Status, o.TotalPrice AS TotalPrice, o.Address_Id AS Address_Id, o.A_Id AS A_Id
    ";

            try
            {
                var session = _driver.AsyncSession();
                var parameters = new Dictionary<string, object>
        {
            { "A_Id", accountId }
        };

                var result = await session.RunAsync(query, parameters);
                var orders = await result.ToListAsync(record => new Orders
                {
                    Id = record["Id"].As<int>(),
                    Date = DateTime.Parse(record["Date"].As<string>()),
                    Status = record["Status"].As<string>(),
                    TotalPrice = record["TotalPrice"].As<float>(),
                    Address_Id = record["Address_Id"].As<int>(),
                    A_Id = record["A_Id"].As<int>()
                });

                return orders;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<Orders>();
            }
        }


        public async Task<Orders> GetOrderById(int orderId)
        {
            string query = @"
        MATCH (o:Order)
        WHERE o.Id = $Order_Id
        RETURN o.Id AS Id, o.Date AS Date, o.Status AS Status, o.TotalPrice AS TotalPrice, o.Address_Id AS Address_Id, o.A_Id AS A_Id
    ";

            try
            {
                var session = _driver.AsyncSession();
                var parameters = new Dictionary<string, object> { { "Order_Id", orderId } };

                var result = await session.RunAsync(query, parameters);
                var order = await result.SingleAsync(record => new Orders
                {
                    Id = record["Id"].As<int>(),
                    Date = DateTime.Parse(record["Date"].As<string>()),
                    Status = record["Status"].As<string>(),
                    TotalPrice = record["TotalPrice"].As<float>(),
                    Address_Id = record["Address_Id"].As<int>(),
                    A_Id = record["A_Id"].As<int>()
                });

                return order;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        public async Task<List<OrderDetails>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            string query = @"
        MATCH (o:Order {Id: $Order_Id})
        MATCH (od:OrderDetail {Order_Id: $Order_Id})
        MATCH (pv:ProductVariant {Id: od.Provar_Id})-[:VARIANT_OF]->(p:Product)
        OPTIONAL MATCH (t:Topping) WHERE t.Id IN od.Topping_Id
        RETURN od.Id AS OrderDetailId, od.Quantity AS Quantity, od.Price AS Price, 
               p.Name AS ProductName, 
               p.Image AS ProductImage,  /* Lấy hình ảnh sản phẩm */
               collect(t.Name) AS Toppings
    ";

            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var parameters = new Dictionary<string, object>
            {
                { "Order_Id", orderId }
            };

                    var result = await session.RunAsync(query, parameters);

                    var orderDetailsList = await result.ToListAsync(record => new OrderDetails
                    {
                        Id = record["OrderDetailId"].As<int>(),
                        Quantity = record["Quantity"].As<int>(),
                        Price = record["Price"].As<float>(),
                        ProductName = record["ProductName"].As<string>(),
                        ProductImage = record["ProductImage"].As<string>(),  
                        Toppings = record["Toppings"].As<List<string>>()
                    });

                    return orderDetailsList;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<OrderDetails>();
            }
        }



    }
}
