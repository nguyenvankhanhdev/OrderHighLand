using Neo4j.Driver;
using OrderHighLand.Models;

namespace OrderHighLand.Service
{
    public class AddressService
    {
        private readonly IDriver _driver;
        public AddressService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<int> getAddressIdMax()
        {
            string query = @"
                    MATCH (c:Address)
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

        public async Task addToAddress(Addresses address)
        {
            string query = @"
                    CREATE (a:Address {
                        Id: $Id,
                        Phone: $Phone,
                        Ward: $Ward,
                        Address: $Address,
                        District: $District,
                        A_Id: $A_Id
                    })";

            try
            {
                var session = _driver.AsyncSession();

                // Tạo dictionary chứa các tham số
                var parameters = new Dictionary<string, object>
                {
                    { "Id", address.Id },
                    { "Phone", address.Phone },
                    { "Ward", address.Ward },
                    { "Address", address.Address },
                    { "District", address.District },
                    { "A_Id", address.A_ID }
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
