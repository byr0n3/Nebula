using Elegance.Enums;

namespace ShipmentTracker.Models.Common
{
	[Enum]
	public enum ShipmentSource
	{
		[EnumValue("unknown")] Unknown,
		[EnumValue("postnl")] PostNL,
		[EnumValue("dhl")] DHL,
	}
}
