using Elegance.Enums;

namespace ParcelTracker.Models
{
	[Enum]
	public enum UserClaim
	{
		[EnumValue("id")] Id,
		[EnumValue("username")] Username,
		[EnumValue("email")] Email,
		[EnumValue("created")] Created,
	}
}
