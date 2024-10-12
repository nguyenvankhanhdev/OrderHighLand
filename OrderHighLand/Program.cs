using OrderHighLand.Service;
using Neo4j.Driver;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IDriver>(provider =>
{

	var uri = "bolt://localhost:7687"; 
	var user = "neo4j";
	var password = "12345678";

	return GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
});

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddSession();
builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton<DashboardService>();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<RoleService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<ConnecNeo4J>();
builder.Services.AddSingleton<CategoryService>();
builder.Services.AddSingleton<SizeService>();
builder.Services.AddSingleton<ProductVartiantService>();
builder.Services.AddSingleton<ProductVariantService>();
builder.Services.AddSingleton<ProductService>();


// Đăng ký bot với dependency injection
builder.Services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
builder.Services.AddTransient<IBot, ChatbotService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});
builder.Services.AddSingleton<ToppingService>();
builder.Services.AddSingleton<CartService>();
builder.Services.AddSingleton<AddressService>();
builder.Services.AddSingleton<OrderService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}
app.UseCors("AllowAllOrigins");
app.UseSession();
//app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",

	pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();




