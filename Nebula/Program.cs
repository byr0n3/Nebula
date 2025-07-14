using Elegance.AspNet.Authentication.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
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
	builder.Configuration.GetConnectionString("Nebula") ?? throw new System.Exception("No database connection string"),
	builder.Environment.IsDevelopment()
);

services.AddScoped<AuthenticationService>();
services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider, AuthenticationStateProvider>();
services.AddAuth<User, UserClaimsProvider>(static (options) =>
{
	options.Cookie.Name = "Nebula.Cookies";

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

var temporalOptions = builder.Configuration.GetSection("Temporal").Get<NebulaTemporalOptions>();

if (temporalOptions is not null)
{
	services.AddWorkflow(temporalOptions);
}
else if (!builder.Environment.IsDevelopment())
{
	throw new System.Exception("Temporal settings haven't been configured.");
}

services.AddHttpContextAccessor();

services.AddLocalization();

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

app.UseMiddleware<CultureMiddleware>();

app.UseRequestLocalization(
	new RequestLocalizationOptions
	{
		RequestCultureProviders =
		[
			new CookieRequestCultureProvider
			{
				CookieName = Cultures.CookieName,
			},
		],
		DefaultRequestCulture = new RequestCulture(Cultures.Default),
		SupportedCultures = Cultures.Supported,
		SupportedUICultures = Cultures.Supported,
	}
);

var api = app.MapGroup("/api");
{
	api.MapPost("push/subscribe", ApiEndpoints.PushSubscribeAsync);
}

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

await app.RunAsync().ConfigureAwait(false);
