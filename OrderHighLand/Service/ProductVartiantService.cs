using Neo4j.Driver;
using OrderHighLand.Models;
using System.Text.RegularExpressions;

namespace OrderHighLand.Service
{
	public class ProductVartiantService
	{
		private readonly IDriver _driver;
		public ProductVartiantService(IDriver driver)
		{
			_driver = driver;

		}
		public string GenerateSlug(string name)
		{
			name = name.ToLowerInvariant();
			name = Regex.Replace(name, @"[^a-z0-9\s-]", "");
			name = Regex.Replace(name, @"\s+", "-").Trim('-');
			name = Regex.Replace(name, @"-+", "-");
			return name;
		}

		private int getIdMax()
		{
			var session = _driver.AsyncSession();
			var result = session.ExecuteReadAsync(async transaction =>
			{
				var readQuery = "MATCH (p:ProductVariant) RETURN max(p.Id) as max";
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
		public async Task<List<ProductVariant>> GetAllAsync(int id)
		{
			var session = _driver.AsyncSession();
			var result = await session.ExecuteReadAsync(async transaction =>
			{
				var readQuery = "MATCH (n:ProductVariant) WHERE n.Pro_Id = $PRO_ID RETURN n";
				var parameters = new { PRO_ID = id };
				var readResult = await transaction.RunAsync(readQuery,parameters);
				var products = await readResult.ToListAsync(record =>
				{
					var node = record["n"].As<INode>();
					return new ProductVariant
					{
						Id = node.Properties["Id"].As<int>(),
						Pro_Id = node.Properties["Pro_Id"].As<int>(),
						Price = node.Properties["Price"].As<float>(),
						Quantity = node.Properties["Quantity"].As<int>(),
						Size_Id = node.Properties["Size_Id"].As<int>()

					};
				});
				return products;
			});
			await session.CloseAsync();
			return result;
		}
		public async Task<ProductVariant> CreateAsync(ProductVariant product)
		{
			var session = _driver.AsyncSession();
			try
			{
				var result = await session.ExecuteWriteAsync(async transaction =>
				{
					var createQuery = "CREATE (n:ProductVariant {Id: $Id, Pro_Id: $Pro_Id, Price: $Price, Quantity: $Quantity, Size_Id: $Size_Id}) RETURN n";
					var newID = getIdMax() + 1;
					var createResult = await transaction.RunAsync(createQuery, new
					{
						Id = newID,
						Pro_Id = product.Pro_Id,
						Price = product.Price,
						Quantity = product.Quantity,
						Size_Id = product.Size_Id
					});
					var products = await createResult.SingleAsync(record =>
					{
						var node = record["n"].As<INode>();
						return new ProductVariant
						{
							Id = node.Properties["Id"].As<int>(),
							Pro_Id = node.Properties["Pro_Id"].As<int>(),
							Price = node.Properties["Price"].As<float>(),
							Quantity = node.Properties["Quantity"].As<int>(),
							Size_Id = node.Properties["Size_Id"].As<int>()
						};
					});
					var createRelationQuery = @"
						MATCH (p:ProductVariant {Id: $PROVar_ID})
						MATCH (v:Product {Id: $PRO_ID})
						MATCH (S:Size {Id:$SIZE_ID})
						CREATE (p)-[:VARIANT_OF]->(v)
						CREATE (p)-[:HAS_SIZE]->(s)";

					var createRelationParams = new
					{
						PROVar_ID = newID,
						PRO_ID = product.Pro_Id, 
						SIZE_ID = product.Size_Id // Cate_Id từ sản phẩm
					};

					await transaction.RunAsync(createRelationQuery, createRelationParams);




					return products;
				});

				return result;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				await session.CloseAsync();
			}
		}
		public async Task<ProductVariant> UpdateAsync(ProductVariant productVariant, int variantId)
		{
			var session = _driver.AsyncSession();
			try
			{
				var result = await session.ExecuteWriteAsync(async transaction =>
				{
					var updateQuery = "MATCH (n:ProductVariant {Id: $variantId}) SET n.Price = $Price,n.Pro_Id= $Pro_Id, n.Quantity= $Quantity, n.Size_Id = $Size_Id RETURN n";
					var readResult = await transaction.RunAsync(updateQuery, new
					{
						Price = productVariant.Price,
						Pro_Id = productVariant.Pro_Id,
						Quantity = productVariant.Quantity,
						Size_Id = productVariant.Size_Id,
						variantId = variantId
					});
					var updateVariant = await readResult.SingleAsync(record =>
					{
						var node = record["n"].As<INode>();
						return new ProductVariant
						{
							Id = node.Properties["Id"].As<int>(),
							Pro_Id = node.Properties["Pro_Id"].As<int>(),
							Price = node.Properties["Price"].As<float>(),
							Quantity = node.Properties["Quantity"].As<int>(),
							Size_Id = node.Properties["Size_Id"].As<int>()
						};
					});


					return updateVariant;
				});
				return result;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				await session.CloseAsync();
			}

		}

		public async Task<ProductVariant> GetByIdAsync(int id)
		{
			var session = _driver.AsyncSession();
			try
			{
				var result = await session.ExecuteReadAsync(async transaction =>
				{
					var readQuery = "MATCH (n:ProductVariant) WHERE n.Id = $Id RETURN n";
					var readResult = await transaction.RunAsync(readQuery, new { Id = id });
					var products = await readResult.SingleAsync(record =>
					{
						var node = record["n"].As<INode>();
						return new ProductVariant
						{
							Id = node.Properties["Id"].As<int>(),
							Pro_Id = node.Properties["Pro_Id"].As<int>(),
							Price = node.Properties["Price"].As<float>(),
							Quantity = node.Properties["Quantity"].As<int>(),
							Size_Id = node.Properties["Size_Id"].As<int>()
						};
					});
					return products;
				});
				return result;
			}
			catch (Exception ex)
			{
				throw ex;
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
					var deleteQuery = "MATCH (n:ProductVariant) WHERE n.Id = $Id DETACH DELETE n";

					await transaction.RunAsync(deleteQuery, new { Id = id });
				});
				
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				await session.CloseAsync();
			}
		}

	}

}
