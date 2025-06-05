using Elegance.Enums;

namespace ShipmentTracker.Common.Models
{
	[Enum]
	public enum ShipmentSource
	{
		[EnumValue("unknown")] Unknown,
		[EnumValue("postnl")] PostNL,
		[EnumValue("dhl")] DHL,
	}
}
