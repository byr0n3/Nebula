using System.Runtime.CompilerServices;
using Nebula.Models.Common;
using Nebula.Models.Dto;

namespace Nebula
{
	internal static class Urls
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetTrackingUrl(in ShipmentDto shipment) =>
			Urls.GetTrackingUrl(shipment.Code, shipment.ZipCode, shipment.Source);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetTrackingUrl(string code, string zipCode, ShipmentSource shipmentSource)
		{
			var source = ShipmentSourceEnumData.GetValue(shipmentSource);

			return string.Create(
				null,
				// @todo Sanitize zipcode format
				$"/shipments/{code}/{zipCode}?{Web.Pages.Shipment.SourceQueryKey}={source}"
			);
		}

		// @todo Country support
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetOriginalTrackingUrl(string code, string zipCode, ShipmentSource source) =>
			// @todo Sanitize zipcode format
			(source) switch
			{
				ShipmentSource.PostNL =>
					string.Create(null, $"https://jouw.postnl.nl/track-and-trace/{code}-NL-{zipCode}"),

				ShipmentSource.DHL =>
					string.Create(null, $"https://my.dhlecommerce.nl/home/tracktrace/{code}/{zipCode}"),

				_ => throw new System.ArgumentException("Unknown source", nameof(source)),
			};
	}
}
