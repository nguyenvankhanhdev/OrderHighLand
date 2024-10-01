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

        public async Task<List<Sizes>> getAllSize()
        {
            var query = @"MATCH(s:Size)
                  RETURN s.Id as Id, s.Size as Size, s.Price as Price";
            try
            {
                using var session = _driver.AsyncSession();

                var result = await session.RunAsync(query);
                var sizes = new List<Sizes>();

                await result.ForEachAsync(record =>
                {
                    var size = new Sizes
                    {
                        Id = record["Id"].As<int>(),
                        Size = record["Size"].As<string>(),
                        Price = record["Price"].As<int>()
                    };
                    sizes.Add(size);
                });

                return sizes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                return new List<Sizes>();
            }
        }
        public async Task<List<Products>> getAllProducts()
        {
            var query = @"MATCH(s:Product)
                        RETURN s.Id as ID, s.Name as Name, s.Image as Image,
                               s.Slug as Slug, s.Cate_Id as Cate_Id";
            try
            {
                using var session = _driver.AsyncSession();

                var result = await session.RunAsync(query);
                var products = new List<Products>();

                await result.ForEachAsync(record =>
                {
                    var product = new Products
                    {

                        Id = record["Id"].As<int>(),
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
