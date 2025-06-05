using Elegance.AspNet.Authentication.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShipmentTracker.Database.Extensions;
using ShipmentTracker.Database.Models;
using ShipmentTracker.DHL.Extensions;
using ShipmentTracker.PostNL.Extensions;
using ShipmentTracker.Services;
using ShipmentTracker.Web;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddRazorComponents()
		.AddInteractiveServerComponents();

services.AddDatabase(
	builder.Configuration.GetConnectionString("Shipment") ?? throw new System.Exception("No database connection string"),
	builder.Environment.IsDevelopment()
);

services.AddAuth<User, UserClaimsProvider>(static (options) =>
{
	options.Cookie.Name = "Shipment.Cookies";

	options.ExpireTimeSpan = System.TimeSpan.FromMinutes(60);
	options.SlidingExpiration = true;
	options.AccessDeniedPath = "/404";
	options.LoginPath = "/login";
	options.LogoutPath = "/logout";
});

services.AddSingleton<ShipmentsService>()
		.AddPostNLClient()
		.AddDHLClient();

services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseHttpsRedirection();
}
else
{
	app.UseExceptionHandler("/error");
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

await app.RunAsync().ConfigureAwait(false);
