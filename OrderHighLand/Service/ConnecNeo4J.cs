using Neo4j.Driver;

namespace OrderHighLand.Service
{
    public class ConnecNeo4J : IDisposable
    {
        private readonly IDriver _driver;

        public ConnecNeo4J(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

<<<<<<< Updated upstream
=======
        public async Task<string> PrintGreetingAsync(string message)
        {
            await using var session = _driver.AsyncSession();
            var greeting = await session.ExecuteWriteAsync(
                async tx =>
                {
                    var result = await tx.RunAsync(
                        "CREATE (a:Greeting) " +
                        "SET a.message = $message " +
                        "RETURN a.message + ', from node ' + id(a)",
                        new { message });

                    var record = await result.SingleAsync();
                    return record[0].As<string>();
                });

            return greeting;
        }

        public async Task<List<string>> GetAllCustomersAsync()
        {
            var customers = new List<string>();

            await using var session = _driver.AsyncSession();
            var result = await session.RunAsync(
            "MATCH (c:Account) WHERE c.cus_id = 'cus_id' RETURN c.cus_id, c.name");
            await foreach (var record in result)
            {

                customers.Add(record["c.name"].As<string>());
            }

            return customers;
        }

>>>>>>> Stashed changes
        public void Dispose()
        {
            _driver?.Dispose();
        }

    }
}
