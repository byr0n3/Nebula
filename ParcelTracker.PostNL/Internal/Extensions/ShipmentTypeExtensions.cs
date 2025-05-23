using System.Runtime.CompilerServices;
using ParcelTracker.Common.Models;
using ParcelTracker.PostNL.Models;

namespace ParcelTracker.PostNL.Internal.Extensions
{
	internal static class ShipmentTypeExtensions
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
