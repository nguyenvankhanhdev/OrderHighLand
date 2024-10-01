using Neo4j.Driver;
using OrderHighLand.Models;


namespace OrderHighLand.Service
{
	public class ProductService
	{
		private readonly IDriver _driver;
		public ProductService(IDriver driver)
		{
			_driver = driver;
		}

		// Lấy tất cả các Product
		public async Task<List<Products>> GetAllAsync()
		{
			var session = _driver.AsyncSession();
			var result = await session.ExecuteReadAsync(async transaction =>
			{
				var readQuery = "MATCH (p:Product) RETURN p";
				var readResult = await transaction.RunAsync(readQuery);
				var products = await readResult.ToListAsync(record =>
				{
					var node = record["p"].As<INode>();
					return new Products
					{
						Id = node.Properties["Id"].As<int>(),
						Name = node.Properties["Name"].As<string>(),
						Slug = node.Properties["Slug"].As<string>(),
						Type = node.Properties["Type"].As<string>(),		
						Cate_Id = node.Properties["Cate_Id"].As<int>()
					};
				});
				return products;
			});
			await session.CloseAsync();
			return result;
		}

        // Lấy 4 sản phẩm type Hot
        public async Task<List<Products>> getFourHotProduct()
		{
            var query = @"MATCH (a:Product {Type:'Hot'}) 
						RETURN a
						LIMIT 4";
            try
            {
                var session = _driver.AsyncSession();
                var result = await session.RunAsync(query);

                var products = new List<Products>();

                await result.ForEachAsync(record =>
                {
                    var node = record["a"].As<INode>();
                    var product = new Products
                    {
                        Id = node.Properties["Id"].As<int>(),
                        Name = node.Properties["Name"].As<string>(),
                        Image = node.Properties["Image"].As<string>(),
                        Type = node.Properties["Type"].As<string>(),
                        Cate_Id = node.Properties["Cate_Id"].As<int>(),
                        Slug = node.Properties["Slug"].As<string>()
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

		// Lấy 2 sản phẩm type New
		public async Task<List<Products>> getTwoNewProduct()
		{
			var query = @"MATCH (a:Product {Type:'New'}) 
						RETURN a
						LIMIT 2";
			try
			{
                var session = _driver.AsyncSession();
                var result = await session.RunAsync(query);

                var products = new List<Products>();

                await result.ForEachAsync(record =>
                {
                    var node = record["a"].As<INode>();
                    var product = new Products
                    {
                        Id = node.Properties["Id"].As<int>(),
                        Name = node.Properties["Name"].As<string>(),
                        Image = node.Properties["Image"].As<string>(),
                        Type = node.Properties["Type"].As<string>(),
                        Cate_Id = node.Properties["Cate_Id"].As<int>(),
                        Slug = node.Properties["Slug"].As<string>()
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

		public async Task<List<Products>> getProductByCateId(int cateId)
		{
			var query = @"MATCH (c:Product {Cate_Id: $cateId}) RETURN c";

			try
			{
				var session = _driver.AsyncSession();
				var result = await session.RunAsync(query, new { cateId });
				var products = new List<Products>();
				await result.ForEachAsync(record =>
				{
					var node = record["c"].As<INode>();
					var product = new Products
					{
                        Id = node.Properties["Id"].As<int>(),
                        Name = node.Properties["Name"].As<string>(),
                        Image = node.Properties["Image"].As<string>(),
                        Type = node.Properties["Type"].As<string>(),
                        Cate_Id = node.Properties["Cate_Id"].As<int>(),
                        Slug = node.Properties["Slug"].As<string>()
                    };
					products.Add(product);
				});
                return products;
            }
			catch(Exception ex)
			{
				Console.WriteLine($"Lỗi: {ex.Message}");
				return new List<Products>();
			}

		}

		// Lấy Id lớn nhất của Product
		private int getIdMax()
		{
			var session = _driver.AsyncSession();
			var result = session.ExecuteReadAsync(async transaction =>
			{
				var readQuery = "MATCH (p:Product) RETURN max(p.Id) as max";
				var readResult = await transaction.RunAsync(readQuery);
				var products = await readResult.ToListAsync(record =>
				{
					var node = record["max"].As<int>();
					return node;
				});
				return products;
			});
			session.CloseAsync();
			return result.Result[0];
		}
		public async Task<Products> CreateAsync(Products product)
		{
			var session = _driver.AsyncSession();
			try
			{
				var result = await session.ExecuteWriteAsync(async transaction =>
				{
					// Truy vấn tạo node Product với các thuộc tính được truyền vào
					var createQuery = @"
					CREATE (p:Product {
                    Id: $Id, 
                    Name: $Name, 
                    Slug: $Slug, 
                    Image: $Image,
					Type: $Type,
                    Cate_Id: $Cate_Id 
                })
                RETURN p";

					// Tạo các tham số truy vấn
					var createParams = new
					{
                        Id = getIdMax() + 1, // Giả sử hàm getIdMax() trả về ID lớn nhất hiện có
                        Name = product.Name,
                        Slug = product.Slug,
                        Image = product.Image,
						Type= product.Type,
                        Cate_Id = product.Cate_Id
					};

					// Thực thi truy vấn và lấy kết quả
					var createResult = await transaction.RunAsync(createQuery, createParams);

					// Xử lý kết quả trả về
					var createdProduct = await createResult.SingleAsync(record =>
					{
						var node = record["p"].As<INode>();
						return new Products
						{
							Id = node.Properties["Id"].As<int>(),
							Name = node.Properties["Name"].As<string>(),
							Slug = node.Properties["Slug"].As<string>(),
							Image = node.Properties["Image"].As<string>(),
							Type = node.Properties["Type"].As<string>(),
							Cate_Id = node.Properties["Cate_Id"].As<int>()
						};
					});

					return createdProduct;
				});

				return result;
			}
			finally
			{
				await session.CloseAsync();
			}
		}



		public Task<T> DeleteAsync<T>(int id)
		{
			throw new NotImplementedException();
		}



		public Task<T> GetByIdAsync<T>(int id)
		{
			throw new NotImplementedException();
		}

		public Task<T> UpdateAsync<T>(int id, Products product)
		{
			throw new NotImplementedException();
		}
	}

}
