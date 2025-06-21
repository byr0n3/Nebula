namespace ShipmentTracker.Models.Common
{
	/// <summary>
	/// The type of shipment.
	/// </summary>
	public enum ShipmentType
	{
		/// <summary>
		/// The delivery service didn't specify what type of shipment this is.
		/// </summary>
		Unknown,

		/// <summary>
		/// A standard delivery package.
		/// </summary>
		/// <remarks>These shipments have to be accepted by the recipient themselves and might require a signature.</remarks>
		Package,

		/// <summary>
		/// A standard letter.
		/// </summary>
		/// <remarks>These shipments get delivered to the recipient's letterbox.</remarks>
		Letter,

		/// <summary>
		/// A package that's small enough to fit in a letterbox.
		/// </summary>
		/// <remarks>Event tho this is technically a package, hese shipments get delivered to the recipient's letterbox.</remarks>
		LetterboxPackage,
	}
}
