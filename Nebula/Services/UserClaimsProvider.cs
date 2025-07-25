using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading;
using Elegance.AspNet.Authentication;
using Elegance.Extensions;
using Nebula.Models;
using Nebula.Models.Database;

namespace Nebula.Services
{
	internal sealed class UserClaimsProvider : IClaimsProvider<User>
	{
		public async IAsyncEnumerable<Claim> GetClaimsAsync(User user, [EnumeratorCancellation] CancellationToken token)
		{
			yield return UserClaimsProvider.Claim(UserClaim.Id, user.Id);
			yield return UserClaimsProvider.Claim(UserClaim.Email, user.Email);
			yield return UserClaimsProvider.Claim(UserClaim.Flags, ((int)user.Flags).Str());
			yield return UserClaimsProvider.Claim(UserClaim.Culture, user.Culture);
			yield return UserClaimsProvider.Claim(UserClaim.UiCulture, user.UiCulture);
			yield return UserClaimsProvider.Claim(UserClaim.TimeZone, user.TimeZone);
			yield return UserClaimsProvider.Claim(UserClaim.Created, user.Created.ToString("O", DateTimeFormatInfo.InvariantInfo));
		}

		private static Claim Claim(UserClaim claim, string value) =>
			new(UserClaimEnumData.GetValue(claim)!, value);

		private static Claim Claim<T>(UserClaim claim, T value) where T : INumber<T> =>
			new(UserClaimEnumData.GetValue(claim)!, value.Str());
	}
}
