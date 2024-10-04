using Neo4j.Driver;
using OrderHighLand.Models;


namespace OrderHighLand.Service
{
	public class CategoryService
	{
		private readonly IDriver _driver;
		public CategoryService(IDriver driver)
		{
			_driver = driver;
		}
		public async Task<List<Models.Category>> GetAllAsync()
		{
			var session = _driver.AsyncSession();
			var result = await session.ExecuteReadAsync(async transaction =>
			{
				var readQuery = "MATCH (c:Category) RETURN c";
				var readResult = await transaction.RunAsync(readQuery);
				var categories = await readResult.ToListAsync(record =>
				{
					var node = record["c"].As<INode>();
					return new Models.Category
					{
						Id = node.Properties["Id"].As<int>(),
						Name = node.Properties["Name"].As<string>(),
						Slug = node.Properties["Slug"].As<string>(),
					};
				});
				return categories;
			});
			await session.CloseAsync();
			return result;
		}

		private int getIdMax()
		{
			var session = _driver.AsyncSession();
			var result = session.ExecuteReadAsync(async transaction =>
			{
				var readQuery = "MATCH (c:Category) RETURN max(c.Id) as max";
				var readResult = await transaction.RunAsync(readQuery);
				var categories = await readResult.ToListAsync(record =>
				{
					var node = record["max"].As<int>();
					return node;
				});
				return categories;
			});
			session.CloseAsync();
			return result.Result[0];
		}
		public async Task<int> getIdBySlug(string slug)
		{
			var query = @"MATCH (c:Category {Slug: $slug }) RETURN c.Id";
			try
			{
				var session = _driver.AsyncSession();
				var result = await session.RunAsync(query, new { slug });

				var record = await result.SingleAsync();
				int categoryId = record["c.Id"].As<int>();

				await session.CloseAsync();
				return categoryId;

			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi: {ex.Message}");
				return 0;
			}
		}
		public async Task<List<(string Name, string Slug, int Count)>> countAllCate()
		{
			var query = @" MATCH (p:Product), (c:Category)
                           WHERE p.Cate_Id = c.Id
                           RETURN c.Name AS Name, c.Slug AS Slug, COUNT(p) AS PCount";
			var categoryCount = new List<(string, string, int)>();
			try
			{
				var session = _driver.AsyncSession();
				var result = await session.RunAsync(query);

				while (await result.FetchAsync())
				{
					var name = result.Current["Name"].As<string>();
					var slug = result.Current["Slug"].As<string>();
					var count = result.Current["PCount"].As<int>();

					categoryCount.Add((name, slug, count));
				}

				await session.CloseAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");

			}
			return categoryCount;
		}


	}
}