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

        public void Dispose()
        {
            _driver?.Dispose();
        }

    }
}
