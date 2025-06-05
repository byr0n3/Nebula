using System.Threading.Tasks;
using Elegance.AspNet.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using ShipmentTracker.Database.Models;

namespace ShipmentTracker.Web.Components
{
	public sealed partial class LogoutForm : ComponentBase
	{
		private const string formName = "logout";

		[CascadingParameter] public required HttpContext HttpContext { get; init; }

		[Inject] public required AuthenticationService<User> Authentication { get; init; }

		[Inject] public required NavigationManager Navigation { get; init; }

		private readonly EditContext context = new(0);

		private async Task LogoutAsync()
		{
			await this.Authentication.LogoutAsync(this.HttpContext);

			this.Navigation.NavigateTo("/", true);
		}
	}
}
