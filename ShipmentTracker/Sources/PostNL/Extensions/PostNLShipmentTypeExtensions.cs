using System.Runtime.CompilerServices;
using ShipmentTracker.Models.Common;
using ShipmentTracker.Sources.PostNL.Models;

namespace ShipmentTracker.Sources.PostNL.Extensions
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
