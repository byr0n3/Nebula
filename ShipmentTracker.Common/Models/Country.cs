using Elegance.Enums;

namespace ShipmentTracker.Common.Models
{
	[Enum]
	public enum Country
	{
		[EnumValue("NL")] Netherlands,
		[EnumValue("BE")] Belgium,
		[EnumValue("DE")] Germany,
		[EnumValue("FR")] France,
		[EnumValue("UK")] UnitedKingdom,
	}
}
