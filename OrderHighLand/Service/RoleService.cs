using Neo4j.Driver;
using OrderHighLand.Models;

namespace OrderHighLand.Service
{
	public class RoleService
	{
		private readonly IDriver _driver;
		public RoleService(IDriver driver)
		{
			_driver = driver;
		}

		public async Task<List<Role>> getAllRoleAsync()
		{
			var session = _driver.AsyncSession();
			var result = await session.ExecuteReadAsync(async tx =>
			{
				var query = "MATCH (r:Role) RETURN r";
				var readQuery = await tx.RunAsync(query);
				var roles = await readQuery.ToListAsync(record =>
				{
					var node = record["r"].As<INode>();
					return new Role
					{
						Id = node.Properties["Id"].As<int>(),
						Name = node.Properties["Name"].As<string>(),
					};
				});

				return roles;

			});
			await session.CloseAsync();
			return result;
		}
	}
}
