using Elegance.Enums;

namespace ShipmentTracker.Common.Models
{
	/// <summary>
	/// The current state/progress of the shipment.
	/// </summary>
	[Enum]
	public enum ShipmentState
	{
		/// <summary>
		/// The shipment has been registered, but not received by the delivery service.
		/// </summary>
		[EnumValue("registered")]
		Registered = 0,

		/// <summary>
		/// The shipment has been received by the delivery service. It now needs to be sorted.
		/// </summary>
		[EnumValue("received")]
		Received,

		/// <summary>
		/// The shipment has arrived at the delivery service and has been sorted accordingly.
		/// </summary>
		[EnumValue("sorted")]
		Sorted,

		/// <summary>
		/// The shipment is out for delivery and will arrive soon.
		/// </summary>
		[EnumValue("out_for_delivery")]
		OutForDelivery,

		/// <summary>
		/// The shipment has been delivered.
		/// </summary>
		[EnumValue("delivered")]
		Delivered,
	}
}
