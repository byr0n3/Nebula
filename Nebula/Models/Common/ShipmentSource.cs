using Elegance.Enums;

namespace Nebula.Models.Common
{
	[Enum]
	public enum ShipmentSource
	{
		[EnumValue("unknown")] Unknown,
		[EnumValue("postnl")] PostNL,
		[EnumValue("dhl")] DHL,
	}
}
