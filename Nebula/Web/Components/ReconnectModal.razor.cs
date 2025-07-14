using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Nebula.Web.Components
{
	public sealed partial class ReconnectModal : ComponentBase
	{
		[Inject] public required IStringLocalizer<ReconnectModalLocalization> Localizer { get; init; }
	}
}
