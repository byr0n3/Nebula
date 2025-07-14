using Elegance.Enums;

namespace Nebula.Models
{
	[Enum]
	public enum UserClaim
	{
		[EnumValue("id")] Id,
		[EnumValue("email")] Email,
		[EnumValue("flags")] Flags,
		[EnumValue("culture")] Culture,
		[EnumValue("created")] Created,
	}
}
