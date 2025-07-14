using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Nebula.Web.Layout
{
	public sealed partial class MainLayout : LayoutComponentBase
	{
		[Inject] public required IStringLocalizer<MainLayoutLocalization> Localizer { get; init; }
	}
}
