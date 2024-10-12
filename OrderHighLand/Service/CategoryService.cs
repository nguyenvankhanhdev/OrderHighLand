using Neo4j.Driver;
using OrderHighLand.Models;
namespace OrderHighLand.Service
{
	public class CategoryService
	{
		private readonly IDriver _driver;
		private readonly ProductService _productService;
		public CategoryService(IDriver driver, ProductService productService)
		{
			_driver = driver;
			_productService = productService;
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

		public async Task<Models.Category> CreateAsync(Models.Category category)
		{
			var session = _driver.AsyncSession();
			var result = await session.ExecuteWriteAsync(async transaction =>
			{
				var createQuery = @"CREATE (c:Category {Id: $Id, Name: $Name, Slug: $Slug})";
				var generateSlug = _productService.GenerateSlug(category.Name);
				var createResult = await transaction.RunAsync(createQuery, new
				{
					Id = getIdMax() + 1,
					Name = category.Name,
					Slug = generateSlug 
				});
				return await createResult.ConsumeAsync(); 
			});
			await session.CloseAsync();
			return category;
		}
		public async Task<Models.Category> GetByIdAsync(int id)
		{
			var session = _driver.AsyncSession();
			var result = await session.ExecuteReadAsync(async transaction =>
			{
				var readQuery = "MATCH (c:Category {Id: $Id}) RETURN c";
				var readResult = await transaction.RunAsync(readQuery, new { Id = id });
				var category = await readResult.SingleAsync(record =>
				{
					var node = record["c"].As<INode>();
					return new Models.Category
					{
						Id = node.Properties["Id"].As<int>(),
						Name = node.Properties["Name"].As<string>(),
						Slug = node.Properties["Slug"].As<string>(),
					};
				});
				return category;
			});
			await session.CloseAsync();
			return result;
		}
		public async Task<Models.Category> UpdateAsync(int id,Models.Category category)
		{
			var session = _driver.AsyncSession();
			var result = await session.ExecuteWriteAsync(async transaction =>
			{
				var updateQuery = "MATCH (c:Category {Id: $Id}) SET c.Name = $Name, c.Slug = $Slug";
				var generateSlug = _productService.GenerateSlug(category.Name);
				var updateResult = await transaction.RunAsync(updateQuery, new { Id = id, Name = category.Name, Slug = generateSlug });
				return await updateResult.SingleAsync();
			});
			await session.CloseAsync();
			return category;

		}
		public async Task DeleteAsync(int id)
		{
			var session = _driver.AsyncSession();
			var result = await session.ExecuteWriteAsync(async transaction =>
			{
				var deleteQuery = "MATCH (c:Category {Id: $Id}) DETACH DELETE c";
				var deleteResult = await transaction.RunAsync(deleteQuery, new { Id = id });
				return await deleteResult.SingleAsync();
			});
			await session.CloseAsync();
		}




	}
}