namespace ParcelTracker.Common.Models
{
	/// <summary>
	/// The current state/progress of the shipment.
	/// </summary>
	public enum ShipmentState
	{
		/// <summary>
		/// The shipment has been registered, but not received by the delivery service.
		/// </summary>
		Registered = 0,

		/// <summary>
		/// The shipment has been received by the delivery service. It now needs to be sorted.
		/// </summary>
		Received,

		/// <summary>
		/// The shipment has arrived at the delivery service and has been sorted accordingly.
		/// </summary>
		Sorted,

		/// <summary>
		/// The shipment is out for delivery and will arrive soon.
		/// </summary>
		OutForDelivery,

		/// <summary>
		/// The shipment has been delivered.
		/// </summary>
		Delivered,
	}
}
