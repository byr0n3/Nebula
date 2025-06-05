namespace ShipmentTracker.Common.Models
{
	/// <summary>
	/// <p>The type of event that happened.</p>
	/// <p>This could be a shipment state update, but also a manual change.</p>
	/// </summary>
	public enum ShipmentEventType
	{
		/// <inheritdoc cref="ShipmentState.Registered"/>
		Registered = 0,

		/// <inheritdoc cref="ShipmentState.Received"/>
		Received,

		/// <inheritdoc cref="ShipmentState.Sorted"/>
		Sorted,

		/// <inheritdoc cref="ShipmentState.OutForDelivery"/>
		OutForDelivery,

		/// <inheritdoc cref="ShipmentState.Delivered"/>
		Delivered,

		/// <summary>
		/// The receiver/sender updated some information about the shipment.
		/// </summary>
		InformationUpdate,
	}
}
