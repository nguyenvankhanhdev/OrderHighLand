using Neo4j.Driver;
using OrderHighLand.Models;
using BCrypt.Net;

namespace OrderHighLand.Service
{
    public class UserService
    {
        private readonly IDriver _driver;
        public UserService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<int> getMaxIdAsync()
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteReadAsync(async transaction =>
                {
                    var query = "MATCH (a:Account) RETURN max(a.Id) as maxId";
                    var maxResult = await transaction.RunAsync(query);
                    var maxId = await maxResult.SingleAsync(record => record["maxId"].As<int>());
                    return maxId;
                });
                return result;
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<Accounts> GetUserByEmailAsync(string email)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteReadAsync(async transaction =>
                {
                    var query = "MATCH (a:Account {Email: $email}) RETURN a";
                    var parameters = new { email };

                    var queryResult = await transaction.RunAsync(query, parameters);
                    var records = await queryResult.ToListAsync();

                    if (records.Count > 0)
                    {
                        var node = records[0]["a"].As<INode>();
                        return new Accounts
                        {
                            A_ID = node.Properties["Id"].As<int>(),
                            A_NAME = node.Properties["Name"].As<string>(),
                            A_EMAIL = node.Properties["Email"].As<string>(),
                            A_PASSWORD = node.Properties["Password"].As<string>(),
                            ROLE_ID = node.Properties["Role_Id"].As<int>()
                        };
                    }
                    return null;
                });

                return result;
            }
            finally
            {
                await session.CloseAsync();
            }
        }



        public async Task<Accounts> RegisterAsync(Register model)
        {
            var session = _driver.AsyncSession();
            try
            {
                var newUserId = await getMaxIdAsync() + 1;
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var result = await session.ExecuteWriteAsync(async transaction =>
                {
                    var createQuery = @"
            CREATE (a:Account {
                Id: $A_ID,
                Name: $A_NAME,    
                Email: $A_EMAIL,   
                Password: $A_PASSWORD,
                Role_Id: 2  // Default to regular user role
            }) RETURN a";

                    var createParams = new
                    {
                        A_ID = newUserId,
                        A_NAME = model.Name,
                        A_EMAIL = model.Email,
                        A_PASSWORD = hashedPassword
                    };

                    var queryResult = await transaction.RunAsync(createQuery, createParams);
                    var account = await queryResult.SingleAsync(record =>
                    {
                        var node = record["a"].As<INode>();
                        return new Accounts
                        {
                            A_ID = node.Properties["Id"].As<int>(),
                            A_NAME = node.Properties["Name"].As<string>(),
                            A_EMAIL = node.Properties["Email"].As<string>(),
                            A_PASSWORD = node.Properties["Password"].As<string>(),
                            ROLE_ID = node.Properties.ContainsKey("Role_Id") ? node.Properties["Role_Id"].As<int>() : 2
                        };
                    });

                    return account;
                });

                return result;
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<Accounts> LoginAsync(string email, string password)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteReadAsync(async transaction =>
                {
                    var query = "MATCH (a:Account {Email: $email}) RETURN a";
                    var parameters = new { email };

                    var queryResult = await transaction.RunAsync(query, parameters);
                    var records = await queryResult.ToListAsync();

                    if (records.Count > 0)
                    {
                        var node = records[0]["a"].As<INode>();
                        var storedPassword = node.Properties["Password"].As<string>();

                        if (BCrypt.Net.BCrypt.Verify(password, storedPassword))
                        {
                            return new Accounts
                            {
                                A_ID = node.Properties["Id"].As<int>(),
                                A_NAME = node.Properties["Name"].As<string>(),
                                A_EMAIL = node.Properties["Email"].As<string>(),
                                A_PASSWORD = storedPassword,
                                ROLE_ID = node.Properties.ContainsKey("Role_Id") ? node.Properties["Role_Id"].As<int>() : 2
                            };
                        }
                    }
                    return null; 
                });

                return result;
            }
            finally
            {
                await session.CloseAsync();
            }
        }


    }
}
