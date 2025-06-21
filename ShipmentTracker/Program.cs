using Elegance.AspNet.Authentication.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShipmentTracker.Extensions;
using ShipmentTracker.Models.Database;
using ShipmentTracker.Models.Requests;
using ShipmentTracker.Services;
using ShipmentTracker.Temporal;
using ShipmentTracker.Temporal.Extensions;
using ShipmentTracker.Web;
using ShipmentTracker.WebPush;

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
services.AddHttpClient<VapidClient>("Web Push");

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
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.MapPost("/api/notifications/test",
			static async (HttpContext context, [FromBody] StorePushSubscriptionModel model, VapidClient client) =>
			{
				var pushSubscription = new PushSubscription
				{
					Endpoint = model.Endpoint,
					P256dh = model.Keys.P256dh,
					Auth = model.Keys.Auth,
				};

				var notification = new PushNotification
				{
					Title = "Hello from the server!",
					Body = "Please work",
					Navigate = "https://localhost:5001/account",
				};

				var result = await client.SendAsync(pushSubscription, notification, context.RequestAborted).ConfigureAwait(false);
				context.Response.StatusCode = result ? StatusCodes.Status201Created : StatusCodes.Status500InternalServerError;
			});

await app.RunAsync().ConfigureAwait(false);
