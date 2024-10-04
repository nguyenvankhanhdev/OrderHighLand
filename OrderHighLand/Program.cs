using OrderHighLand.Service;
using Neo4j.Driver;

using OrderHighLand.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IDriver>(provider =>
{

	var uri = "bolt://localhost:7687"; 
	var user = "neo4j";
	var password = "12345678";

	return GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
});

builder.Services.AddControllersWithViews();
builder.Services.AddSession();

builder.Services.AddSingleton<CategoryService>();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<ConnecNeo4J>();
builder.Services.AddSingleton<CategoryService>();
builder.Services.AddSingleton<SizeService>();
builder.Services.AddSingleton<ProductVartiantService>();
builder.Services.AddSingleton<ProductVariantService>();




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




