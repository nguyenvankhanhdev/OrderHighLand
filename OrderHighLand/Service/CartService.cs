using Neo4j.Driver;
using OrderHighLand.Models;
using System.ComponentModel;

namespace OrderHighLand.Service
{
    public class CartService
    {
        private readonly IDriver _driver;
        public CartService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<int> getCartIdMax()
        {
            string query = @"
                    MATCH (c:Cart)
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
        public async Task addToCart(Cart cart)
        {
            string query = @"
                            CREATE (c:Cart {
                                Id: $Id,
                                A_Id: $A_Id,
                                Quantity: $Quantity,
                                Provar_Id: $Provar_Id,
                                TotalPrice: $TotalPrice,
                                Topping_Id: $Topping_Id
                            })";

            try
            {
                var session = _driver.AsyncSession();

                // Tạo dictionary chứa các tham số
                var parameters = new Dictionary<string, object>
                {
                    { "Id", cart.Id },
                    { "A_Id", cart.A_Id },
                    { "Quantity", cart.Quantity },
                    { "Provar_Id", cart.Provar_Id },
                    { "TotalPrice", cart.TotalPrice },
                    { "Topping_Id", cart.Topping_Id }
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

        public async Task removeAllCartByAId(int A_Id)
        {
            string query = @"
                            MATCH (o:Cart)
                            WHERE o.A_Id = $A_Id
                            DELETE o
                        ";

            try
            {
                var session = _driver.AsyncSession();
                var result = await session.RunAsync(query, new { A_Id });

                // Sử dụng SummaryAsync để lấy thông tin về kết quả của truy vấn
                var summary = await result.ConsumeAsync();

                // Kiểm tra số lượng node bị xóa từ thông tin của summary
                if (summary.Counters.NodesDeleted > 0) // Kiểm tra số lượng node bị xóa
                {
                    Console.WriteLine($"Đã xóa {summary.Counters.NodesDeleted} đơn hàng.");
                }
                else
                {
                    Console.WriteLine("Không có đơn hàng nào bị xóa.");
                }

                await session.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public async Task<int> removeFromCartById(int Id)
        {
            string query = @"
                    MATCH (c:Cart)
                    WHERE c.Id = $Id
                    DELETE c
                ";

            try
            {
                var session = _driver.AsyncSession();
                var result = await session.RunAsync(query, new { Id });

                // Sử dụng SummaryAsync để lấy thông tin về kết quả của truy vấn
                var summary = await result.ConsumeAsync();

                // Kiểm tra số lượng node bị xóa từ thông tin của summary
                if (summary.Counters.NodesDeleted > 0) // Số lượng node bị xóa
                {
                    return 1; // Xóa thành công
                }

                return 0; // Không có node nào bị xóa
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0; // Lỗi xảy ra, trả về 0
            }
        }

        public async Task<int> countCartByAId(int A_Id)
        {
            // Truy vấn Cypher để đếm số lượng Cart theo A_Id
            string query = @"
                    MATCH (c:Cart)
                    WHERE c.A_Id = $A_Id
                    RETURN COUNT(c) AS cartCount
                ";

            try
            {
                var session = _driver.AsyncSession();
                var result = await session.RunAsync(query, new { A_Id });

                // Lấy giá trị đếm được
                var count = await result.SingleAsync(record => record["cartCount"].As<int>());
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0; // Trả về 0 trong trường hợp có lỗi
            }
        }
        public async Task<int> countItemInCartById(int A_Id)
        {
            string query = @"
                            MATCH (c:Cart)
                            WHERE c.A_Id = $A_Id
                            RETURN COUNT(c) AS itemCount
                        ";

            try
            {
                var session = _driver.AsyncSession();
                var result = await session.RunAsync(query, new { A_Id });

                var count = await result.SingleAsync(record => record["itemCount"].As<int>());
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0;
            }
        }
        public async Task<List<Cart>> getAllCartById(int A_Id)
        {
            var query = @" MATCH (c:Cart)
                           WHERE c.A_Id = $A_Id
                           RETURN c";

            try
            {
                var session = _driver.AsyncSession();
                var result=await session.RunAsync(query, new {A_Id});
                var Carts=new List<Cart>();

                await result.ForEachAsync(record =>
                {
                    var node = record["c"].As<INode>();
                    var cart = new Cart
                    {
                        Id = node["Id"].As<int>(),
                        Provar_Id = node["Provar_Id"].As<int>(),
                        Quantity = node["Quantity"].As<int>(),
                        TotalPrice = node["TotalPrice"].As<float>(),
                        A_Id = node["A_Id"].As<int>(),
                        // Chuyển đổi Topping_Id từ danh sách object sang List<int>
                        Topping_Id = node["Topping_Id"].As<List<int>>(),

                    };
                    Carts.Add(cart);
                });
                return Carts;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: ");
                return new List<Cart>();
            }
        }
    }
}
