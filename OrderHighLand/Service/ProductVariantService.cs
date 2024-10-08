using Neo4j.Driver;
using OrderHighLand.Models;

namespace OrderHighLand.Service
{
    public class ProductVariantService
    {
        private readonly IDriver _driver;
        public ProductVariantService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<List<ProductVariant>> getAllProductVariants()
        {
            var query = @"MATCH(s:ProductVariant)
                        RETURN s.Id as Id, s.Pro_Id as Pro_Id, s.Size_Id as Size_Id,
                               s.Quantity as Quantity, s.Price as Price";
            try
            {
                using var session = _driver.AsyncSession();

                var result = await session.RunAsync(query);
                var productVariants = new List<ProductVariant>();

                await result.ForEachAsync(record =>
                {
                    var product = new ProductVariant
                    {
                        Id = record["Id"].As<int>(),
                        Pro_Id = record["Pro_Id"].As<int>(),
                        Size_Id = record["Size_Id"].As<int>(),
                        Quantity = record["Quantity"].As<int>(),
                        Price = record["Price"].As<float>()

                    };
                    productVariants.Add(product);

                });
                return productVariants;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                return new List<ProductVariant>();
            }

        }
    }
}
