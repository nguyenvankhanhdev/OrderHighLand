using Neo4j.Driver;
using OrderHighLand.Models;
using System.Xml.Linq;

namespace OrderHighLand.Service
{
	public class SizeService
	{
		private readonly IDriver _driver;
		public SizeService(IDriver driver)
		{
			_driver = driver;
		}
		private int getIdMax()
		{
			var session = _driver.AsyncSession();
			var result = session.ExecuteReadAsync(async transaction =>
			{
				var readQuery = "MATCH (s:Size) RETURN max(s.Id) as max";
				var readResult = await transaction.RunAsync(readQuery);
				var sizes = await readResult.ToListAsync(record =>
				{
					var node = record["max"].As<int>();
					return node;
				});
				return sizes;
			});
			session.CloseAsync();
			return result.Result[0];
		}
		public async Task<List<Sizes>> GetAllAsync()
		{
			var session = _driver.AsyncSession();
			var result = await session.ExecuteReadAsync(async transaction =>
			{
				var readQuery = "MATCH (s:Size) RETURN s";
				var readResult = await transaction.RunAsync(readQuery);
				var sizes = await readResult.ToListAsync(record =>
				{
					var node = record["s"].As<INode>();
					return new Sizes
					{
						Id = node.Properties["Id"].As<int>(),
						Size = node.Properties["Size"].As<string>(),
						Price = node.Properties["Price"].As<float>(),
					};
				});
				return sizes;
			});
			await session.CloseAsync();
			return result;
		}

		public async Task<Sizes> CreateAsync(Sizes size)
		{
			var session = _driver.AsyncSession();
			try
			{
				var result = await session.ExecuteWriteAsync(async transaction =>
				{
					var createQuery = 
					"CREATE (s:Size {Id: $Id, Size: $Size, Price: $Price}) RETURN s";
					var createResult = await transaction.RunAsync(createQuery, new
					{
						Id = getIdMax() + 1,
						Size = size.Size,
						Price = size.Price
					});
				
					var sizes = await createResult.SingleAsync(record =>
					{
						var node = record["s"].As<INode>();
						Console.WriteLine($"Node properties: {string.Join(", ", node.Properties.Keys)}");
						return new Sizes
						{
							Id = node.Properties["Id"].As<int>(),
							Price = node.Properties["Price"].As<float>(),
							Size = node.Properties["Size"].As<string>(),
							
						};
					
					});
					return sizes;
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
		public async Task<Sizes> GetByIdAsync(int id)
		{
			var session = _driver.AsyncSession();
			var result = await session.ExecuteReadAsync(async transaction =>
			{
				var readQuery = "MATCH (s:Size) WHERE s.Id = $Id RETURN s";
				var readResult = await transaction.RunAsync(readQuery, new { Id = id });
				var sizes = await readResult.SingleAsync(record =>
				{
					var node = record["s"].As<INode>();
					return new Sizes
					{
						Id = node.Properties["Id"].As<int>(),
						Size = node.Properties["Size"].As<string>(),
						Price = node.Properties["Price"].As<float>(),
					};
				});
				return sizes;
			});
			await session.CloseAsync();
			return result;
		}
		public async Task<Sizes> UpdateAsync(Sizes size, int id)
		{
			var session = _driver.AsyncSession();
			try
			{
				var result = await session.ExecuteWriteAsync(async transaction =>
				{
					var updateQuery = "MATCH (s:Size) WHERE s.Id = $Id SET s.Size = $Size, s.Price = $Price RETURN s";
					var updateResult = await transaction.RunAsync(updateQuery, new
					{
						Id = id,
						Size = size.Size,
						Price = size.Price
					});
					var sizes = await updateResult.SingleAsync(record =>
					{
						var node = record["s"].As<INode>();
						return new Sizes
						{
							Id = node.Properties["Id"].As<int>(),
							Size = node.Properties["Size"].As<string>(),
							Price = node.Properties["Price"].As<float>(),
						};
					});
					return sizes;
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
					var deleteQuery = "MATCH (s:Size) WHERE s.Id = $Id DETACH DELETE s";
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
