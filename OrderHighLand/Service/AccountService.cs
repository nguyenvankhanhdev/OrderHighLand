using Neo4j.Driver;
using OrderHighLand.Models;

namespace OrderHighLand.Service
{
	public class AccountService
	{
		private readonly IDriver _driver;
		public AccountService(IDriver driver)
		{
			_driver = driver;
		}

		public async Task<List<Accounts>> getAllAccountAsync()
		{
			var session = _driver.AsyncSession();

			var result = await session.ExecuteReadAsync(async transaction =>
			{
				var readQuery = "MATCH (a:Account) RETURN a";
				var readResult = await transaction.RunAsync(readQuery);
				var accounts = await readResult.ToListAsync(record =>
				{
					var node = record["a"].As<INode>();
					return new Accounts
					{
						Id = node.Properties["Id"].As<int>(),
						Name = node.Properties["Name"].As<string>(),
						Username = node.Properties["Username"].As<string>(),
						Password = node.Properties["Password"].As<string>(),
						Role_Id = node.Properties["Role"].As<int>(),
					};
				});
				return accounts;
			});
			await session.CloseAsync();
			return result;
		}

	}

}

