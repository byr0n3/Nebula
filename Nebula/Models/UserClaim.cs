using Elegance.Enums;

namespace Nebula.Models
{
	[Enum]
	public enum UserClaim
	{
		[EnumValue("id")] Id,
		[EnumValue("username")] Username,
		[EnumValue("email")] Email,
		[EnumValue("flags")] Flags,
		[EnumValue("created")] Created,
	}
}
