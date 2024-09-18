using Neo4j.Driver;
using OrderHighLand.Models;
using Category = OrderHighLand.Models.Category;

namespace OrderHighLand.Service
{
	public class ProductService
	{
		private readonly IDriver _driver;
		public ProductService(IDriver driver)
		{
			_driver = driver;
		}
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
						Id = node.Properties["PRO_ID"].As<int>(),
						Name = node.Properties["PRO_NAME"].As<string>(),
						Slug = node.Properties["PRO_SLUG"].As<string>(),
						Cate_Id = node.Properties["CATE_ID"].As<int>()
					};
				});
				return products;
			});
			await session.CloseAsync();
			return result;
		}

		private int getIdMax()
		{
			var session = _driver.AsyncSession();
			var result = session.ExecuteReadAsync(async transaction =>
			{
				var readQuery = "MATCH (p:Product) RETURN max(p.PRO_ID) as max";
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
                    PRO_ID: $PRO_ID, 
                    PRO_NAME: $PRO_NAME, 
                    PRO_SLUG: $PRO_SLUG, 
                    PRO_IMAGE: $PRO_IMAGE, 
                    CATE_ID: $CATE_ID 
                })
                RETURN p";

					// Tạo các tham số truy vấn
					var createParams = new
					{
						PRO_ID = getIdMax() + 1, // Giả sử hàm getIdMax() trả về ID lớn nhất hiện có
						PRO_NAME = product.Name,
						PRO_SLUG = product.Slug,
						PRO_IMAGE = product.Image,  // Assuming you have this property
						CATE_ID = product.Cate_Id
					};

					// Thực thi truy vấn và lấy kết quả
					var createResult = await transaction.RunAsync(createQuery, createParams);

					// Xử lý kết quả trả về
					var createdProduct = await createResult.SingleAsync(record =>
					{
						var node = record["p"].As<INode>();
						return new Products
						{
							Id = node.Properties["PRO_ID"].As<int>(),
							Name = node.Properties["PRO_NAME"].As<string>(),
							Slug = node.Properties["PRO_SLUG"].As<string>(),
							Image = node.Properties["PRO_IMAGE"].As<string>(),
							Cate_Id = node.Properties["CATE_ID"].As<int>()
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
