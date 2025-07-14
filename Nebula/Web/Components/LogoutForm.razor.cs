using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Nebula.Services;

namespace Nebula.Web.Components
{
	public sealed partial class LogoutForm : ComponentBase
	{
		private const string formName = "logout";

		[Inject] public required AuthenticationService Authentication { get; init; }

		[Inject] public required NavigationManager Navigation { get; init; }

		[Parameter] [EditorRequired] public required string Label { get; set; }

		private readonly EditContext context = new(0);

		private async Task LogoutAsync()
		{
			await this.Authentication.DeauthenticateAsync();

			this.Navigation.NavigateTo("/login", true);
		}
	}
}
