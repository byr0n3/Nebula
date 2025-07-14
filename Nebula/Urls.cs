using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Nebula.Models.Common;
using Nebula.Models.Dto;

namespace Nebula
{
	internal static class Urls
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetTrackingUrl(in ShipmentDto shipment) =>
			Urls.GetTrackingUrl(shipment.Code, shipment.ZipCode, shipment.Source);

		public static string GetTrackingUrl(string code, string zipCode, ShipmentSource source = default)
		{
			zipCode = Urls.SanitizeZipCode(zipCode);

			var builder = new StringBuilder($"/shipments/{code}/{zipCode}");

			if (source != default)
			{
				builder.Append($"?{Web.Pages.Shipment.SourceQueryKey}={ShipmentSourceEnumData.GetValue(source)}");
			}

			return builder.ToString();
		}

		// @todo Country support
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetOriginalTrackingUrl(string code, string zipCode, ShipmentSource source)
		{
			zipCode = Urls.SanitizeZipCode(zipCode);

			return (source) switch
			{
				ShipmentSource.PostNL =>
					string.Create(null, $"https://jouw.postnl.nl/track-and-trace/{code}-NL-{zipCode}"),

				ShipmentSource.DHL =>
					string.Create(null, $"https://my.dhlecommerce.nl/home/tracktrace/{code}/{zipCode}"),

				_ => throw new System.ArgumentException("Unknown source", nameof(source)),
			};
		}

		private static string SanitizeZipCode(string zipCode)
		{
			if (!System.MemoryExtensions.Contains(zipCode, ' ') &&
				!System.MemoryExtensions.Contains(zipCode, '-') &&
				!System.MemoryExtensions.Contains(zipCode, '–'))
			{
				return zipCode;
			}

			System.Span<char> buffer = stackalloc char[zipCode.Length];

			var written = 0;

			foreach (var @char in zipCode.Where(static (@char) => @char != ' ' && @char != '-' && @char != '–'))
			{
				buffer[written++] = @char;
			}

			return new string(buffer.Slice(0, written));
		}
	}
}
