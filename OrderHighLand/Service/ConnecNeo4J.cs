using Neo4j.Driver;
using OrderHighLand.Models;

namespace OrderHighLand.Service
{
    public class ConnecNeo4J : IDisposable
    {
        private readonly IDriver _driver;

        public ConnecNeo4J(IDriver driver)
        {
            _driver = driver;
        }
        public async Task<List<Products>> getAllProducts()
        {
            var query = @"MATCH(s:Product)
                        RETURN s.PRO_ID as ID, s.PRO_NAME as Name, s.PRO_IMAGE as Image,
                               s.PRO_SLUG as Slug, s.CATE_ID as Cate_Id";
            try
            {
                using var session = _driver.AsyncSession();

                var result = await session.RunAsync(query);
                var products = new List<Products>();

                await result.ForEachAsync(record =>
                {
                    var product = new Products
                    {

                        Id = record["ID"].As<int>(),
                        Name = record["Name"].As<string>(),
                        Image = record["Image"].As<string>(),
                        Slug = record["Slug"].As<string>(),
                        Cate_Id = record["Cate_Id"].As<int>()

                    };
                    products.Add(product);

                });
                return products;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                return new List<Products>();
            }
           
        }
        // End_Product

        public void Dispose()
        {
            _driver?.Dispose();
        }

    }
}
