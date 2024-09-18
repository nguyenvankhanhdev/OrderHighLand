using Microsoft.AspNetCore.Mvc;
using OrderHighLand.Service;
using OrderHighLand.Models;
using Neo4j.Driver;

namespace OrderHighLand.Controllers.Admin
{
	public class ASizeController : Controller
	{
		private readonly IDriver _driver;
		public ASizeController( IDriver driver )
		{
			_driver = driver;
	
		}


		public IActionResult GetSizes()
		{
			var sizes = new List<Size>();

			using (var session = _driver.AsyncSession())
			{
				var result = session.ExecuteReadAsync(async tx =>
				{
					var cursor = await tx.RunAsync("MATCH (s:Size) RETURN s.S_ID AS id, s.S_SIZE AS name, s.S_PRICE AS price");
					return await cursor.ToListAsync(record => new Size
					{
						ID = record["id"].As<int>(),
						S_SIZE = record["name"].As<string>(),
						S_PRICE = record["price"].As<float>()
					});
				}).Result;

				sizes.AddRange(result);
			}

			return View(sizes);
		}

		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Create(Size size)
		{
			
			return RedirectToAction("Index");
		}
	}
}
