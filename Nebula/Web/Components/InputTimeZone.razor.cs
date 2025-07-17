using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Nebula.Web.Components
{
	public sealed partial class InputTimeZone : ComponentBase
	{
		[Parameter] [EditorRequired] public required string? Value { get; set; }

		[Parameter] public EventCallback<string?> ValueChanged { get; set; }

		[Parameter] public Expression<System.Func<string?>>? ValueExpression { get; set; }

		[Parameter(CaptureUnmatchedValues = true)]
		public Dictionary<string, object>? AdditionalAttributes { get; set; }
	}
}
