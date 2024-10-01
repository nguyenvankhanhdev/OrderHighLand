using Neo4j.Driver;
using OrderHighLand.Models;
using System.Reflection.Emit;
using System.Text.RegularExpressions;


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
						Image = node.Properties["Image"].As<string>(),
						Type = node.Properties["Type"].As<string>(),		
						Cate_Id = node.Properties["Cate_Id"].As<int>()
					};
				});
				return products;
			});
			await session.CloseAsync();
			return result;
		}
		public string GenerateSlug(string name)
		{
			name = name.ToLowerInvariant();
			name = Regex.Replace(name, @"[^a-z0-9\s-]", "");
			name = Regex.Replace(name, @"\s+", "-").Trim('-');
			name = Regex.Replace(name, @"-+", "-");
			return name;
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

					var newProductId = getIdMax() + 1;
					var productSlug = GenerateSlug(product.Name);

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
					// Tạo sản phẩm mới
					var createProductQuery = @"
						CREATE (p:Product {
							Id: $PRO_ID, 
							Name: $PRO_NAME, 
							Slug: $PRO_SLUG, 
							Image: $PRO_IMAGE,
							Cate_Id: $CATE_ID

						})
						RETURN p";

					var createProductParams = new
					{

						PRO_ID = newProductId,
						PRO_NAME = product.Name,
						PRO_SLUG = productSlug,
						PRO_IMAGE = product.Image,
						CATE_ID= product.Cate_Id,

                        Id = getIdMax() + 1, // Giả sử hàm getIdMax() trả về ID lớn nhất hiện có
                        Name = product.Name,
                        Slug = product.Slug,
                        Image = product.Image,
						Type= product.Type,
                        Cate_Id = product.Cate_Id

					};

					var createResult = await transaction.RunAsync(createProductQuery, createProductParams);

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

					// Tạo liên kết đến Category
					var createRelationQuery = @"
                MATCH (p:Product {Id: $PRO_ID})
                MATCH (c:Category {Id: $CATE_ID})
                CREATE (p)-[:BELONGS_TO]->(c)";

					var createRelationParams = new
					{
						PRO_ID = newProductId,
						CATE_ID = product.Cate_Id // Cate_Id từ sản phẩm
					};

					await transaction.RunAsync(createRelationQuery, createRelationParams);

					return createdProduct;
				});

				return result;
			}
			catch (Exception ex)
			{
				throw new Exception("Error creating product and linking to category", ex);
			}
			finally
			{
				await session.CloseAsync();
			}
		}


		public async Task DeleteAsync(int id)
		{
			var session = _driver.AsyncSession();
			try
			{
				await session.ExecuteWriteAsync(async transaction =>
				{
					var deleteQuery = @"
                        MATCH (p:Product {Id: $PRO_ID})
                        DETACH DELETE p";

					var deleteParams = new
					{
						PRO_ID = id,
					};

					await transaction.RunAsync(deleteQuery, deleteParams);
				});
			}
			catch (Exception ex)
			{
				throw new Exception("Error deleting product", ex);
			}
			finally
			{
				await session.CloseAsync();
			}
		}
		public async Task<Products> GetByIdAsync(int id)
		{
			var session = _driver.AsyncSession();
			var result = await session.ExecuteReadAsync(async transaction =>
			{
				var readQuery = @"
					MATCH (p:Product {Id: $PRO_ID})
					RETURN p";

				var readParams = new
				{
					PRO_ID = id,
				};

				var readResult = await transaction.RunAsync(readQuery, readParams);
				var product = await readResult.SingleAsync(record =>
				{
					var node = record["p"].As<INode>();
					return new Products
					{
						Id = node.Properties["Id"].As<int>(),
						Name = node.Properties["Name"].As<string>(),
						Slug = node.Properties["Slug"].As<string>(),
						Image = node.Properties["Image"].As<string>(),
						Cate_Id = node.Properties["Cate_Id"].As<int>()
					};
				});

				return product;
			});
			await session.CloseAsync();
			return result;
		}

		public async Task<Products> UpdateAsync(int id, Products product)
		{
			var session = _driver.AsyncSession();
			try
			{
				var result = await session.ExecuteWriteAsync(async transaction =>
				{
					var productSlug = GenerateSlug(product.Name);
					var updateQuery = @"
						MATCH (p:Product {Id: $PRO_ID})
						SET p.Name = $PRO_NAME, p.Slug = $PRO_SLUG, p.Image = $PRO_IMAGE, p.Cate_Id = $CATE_ID
						RETURN p";

					var updateParams = new
					{
						PRO_ID = id,
						PRO_NAME = product.Name,
						PRO_SLUG = productSlug,
						PRO_IMAGE = product.Image,
						CATE_ID = product.Cate_Id
					};

					var updateResult = await transaction.RunAsync(updateQuery, updateParams);
					var updatedProduct = await updateResult.SingleAsync(record =>
					{
						var node = record["p"].As<INode>();
						return new Products
						{
							Id = node.Properties["Id"].As<int>(),
							Name = node.Properties["Name"].As<string>(),
							Slug = node.Properties["Slug"].As<string>(),
							Image = node.Properties["Image"].As<string>(),
							Cate_Id = node.Properties["Cate_Id"].As<int>()
						};
					});

					return updatedProduct;
				});

				return result;
			}
			catch (Exception ex)
			{
				throw new Exception("Error updating product", ex);
			}
			finally
			{
				await session.CloseAsync();
			}

		}
	}

}
