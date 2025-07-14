using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Nebula.Services;

namespace Nebula.Web.Layout
{
	public sealed partial class MainLayout : LayoutComponentBase
	{
		private const string signOutFormName = "logout";

		[Inject] public required IStringLocalizer<MainLayoutLocalization> Localizer { get; init; }

		[Inject] public required AuthenticationService Authentication { get; init; }

		[Inject] public required NavigationManager Navigation { get; init; }

		private readonly EditContext context = new(0);

		private async Task SignOutAsync()
		{
			await this.Authentication.DeauthenticateAsync();

			this.Navigation.NavigateTo("/login", true);
		}
	}
}
