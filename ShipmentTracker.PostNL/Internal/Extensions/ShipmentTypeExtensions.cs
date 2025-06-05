using System.Runtime.CompilerServices;
using ShipmentTracker.Common.Models;
using ShipmentTracker.PostNL.Models;

namespace ShipmentTracker.PostNL.Internal.Extensions
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
