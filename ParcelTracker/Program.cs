using Elegance.AspNet.Authentication.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ParcelTracker.Database.Extensions;
using ParcelTracker.Database.Models;
using ParcelTracker.DHL.Extensions;
using ParcelTracker.PostNL.Extensions;
using ParcelTracker.Services;
using ParcelTracker.Web;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddRazorComponents()
		.AddInteractiveServerComponents();

services.AddDatabase(
	builder.Configuration.GetConnectionString("Parcel") ?? throw new System.Exception("No database connection string"),
	builder.Environment.IsDevelopment()
);

services.AddAuth<User, UserClaimsProvider>(static (options) =>
{
	options.Cookie.Name = "Parcel.Cookies";

	options.ExpireTimeSpan = System.TimeSpan.FromMinutes(60);
	options.SlidingExpiration = true;
	options.AccessDeniedPath = "/404";
	options.LoginPath = "/login";
	options.LogoutPath = "/logout";
});

services.AddSingleton<ShipmentsService>()
		.AddPostNLClient()
		.AddDHLClient();

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
