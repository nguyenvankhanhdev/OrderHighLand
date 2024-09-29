using OrderHighLand.Service;
using Neo4j.Driver;

using OrderHighLand.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IDriver>(provider =>
{
	var uri = "bolt://localhost:7687"; // Replace with your Neo4j URI
	var user = "neo4j"; // Replace with your Neo4j username
	var password = "12345678"; // Replace with your Neo4j password
	return GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
});

builder.Services.AddControllersWithViews();
builder.Services.AddSession();

builder.Services.AddSingleton<ProductService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<ConnecNeo4J>();




var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}
//app.MapControllerRoute(
//    name: "productDetails",
//    pattern: "Product/Detail/{slug}",
//    defaults: new { controller = "Product", action = "Detail" }
//);

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();




