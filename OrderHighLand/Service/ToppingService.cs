using Neo4j.Driver;
using OrderHighLand.Models;

namespace OrderHighLand.Service
{
    public class ToppingService
    {
        private readonly IDriver _driver;
        public ToppingService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<List<Topping>> getAllTopping()
        {
            var query = "MATCH (c:Topping) RETURN c";
            try
            {
                var session= _driver.AsyncSession();
                var result= await session.RunAsync(query);
                var toppings=new List<Topping>();

                await result.ForEachAsync(record =>
                {
                    var node = record["c"].As<INode>();
                    var topping = new Topping
                    {
                        Id = node["Id"].As<int>(),
                        Name = node["Name"].As<string>(),
                        Price = node["Price"].As<float>(),
                    };
                    toppings.Add(topping);
                });
                return toppings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<Topping>();
            }
        }
    }
}
