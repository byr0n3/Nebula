using System.Runtime.CompilerServices;
using Nebula.Models.Common;
using Nebula.Sources.PostNL.Models;

namespace Nebula.Sources.PostNL.Extensions
{
	internal static class PostNLShipmentTypeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ShipmentType ToShipmentType(this PostNLShipmentType @this) =>
			(@this) switch
			{
				PostNLShipmentType.Parcel          => ShipmentType.Package,
				PostNLShipmentType.LetterboxParcel => ShipmentType.LetterboxPackage,
				_                                  => ShipmentType.Unknown,
			};
	}
}
