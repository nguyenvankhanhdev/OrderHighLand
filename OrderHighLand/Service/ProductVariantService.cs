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
        public async Task<int> getProvarByProIdAndSizeId(int Pro_Id,int Size_Id)
        {
            var query = @"
                        MATCH (pv:ProductVariant)
                        WHERE pv.Pro_Id = $Pro_Id AND pv.Size_Id = $Size_Id
                        RETURN pv.Id as Id";

            try
            {
                using var session = _driver.AsyncSession();

                var result = await session.RunAsync(query, new { Pro_Id, Size_Id });

                // Chuyển kết quả về danh sách
                var records = await result.ToListAsync();

                if (records.Count == 1) // Đảm bảo chỉ có 1 kết quả
                {
                    return records[0]["Id"].As<int>();
                }
                else
                {
                    return -1; // Không tìm thấy hoặc có nhiều hơn 1 kết quả
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                return -1;
            }
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
