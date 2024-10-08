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
    }
}
