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
		

	}
}
