using Elegance.AspNet.Authentication.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nebula;
using Nebula.Extensions;
using Nebula.Models.Database;
using Nebula.Services;
using Nebula.Temporal;
using Nebula.Temporal.Extensions;
using Nebula.Web;
using Vapid.NET;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddRazorComponents()
		.AddInteractiveServerComponents();

services.AddDatabase(
	builder.Configuration.GetConnectionString("Shipment") ?? throw new System.Exception("No database connection string"),
	builder.Environment.IsDevelopment()
);

services.AddScoped<AuthenticationService>();
services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider, AuthenticationStateProvider>();
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

services.Configure<VapidOptions>((options) => builder.Configuration.Bind("Vapid", options));
services.AddHttpClient<VapidClient>("Vapid");

services.AddSingleton<PushNotificationsService>();

var temporalOptions = builder.Configuration.GetSection("Temporal").Get<ShipmentTemporalOptions>();

if (temporalOptions is not null)
{
	services.AddWorkflow(temporalOptions);
}
else if (!builder.Environment.IsDevelopment())
{
	throw new System.Exception("Temporal settings haven't been configured.");
}

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

var api = app.MapGroup("/api");
{
	api.MapPost("push/subscribe", ApiEndpoints.PushSubscribeAsync);
}

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

await app.RunAsync().ConfigureAwait(false);
