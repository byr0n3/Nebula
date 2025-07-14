using System.ComponentModel.DataAnnotations;
using Elegance.Enums;

namespace Nebula.Models.Common
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
		[Display(Name = "registered", ResourceType = typeof(Resources.ShipmentStateLocalization))]
		Registered = 0,

		/// <summary>
		/// The shipment has been received by the delivery service. It now needs to be sorted.
		/// </summary>
		[EnumValue("received")]
		[Display(Name = "received", ResourceType = typeof(Resources.ShipmentStateLocalization))]
		Received,

		/// <summary>
		/// The shipment has arrived at the delivery service and has been sorted accordingly.
		/// </summary>
		[EnumValue("sorted")]
		[Display(Name = "sorted", ResourceType = typeof(Resources.ShipmentStateLocalization))]
		Sorted,

		/// <summary>
		/// The shipment is out for delivery and will arrive soon.
		/// </summary>
		[EnumValue("out_for_delivery")]
		[Display(Name = "out_for_delivery", ResourceType = typeof(Resources.ShipmentStateLocalization))]
		OutForDelivery,

		/// <summary>
		/// The shipment has been delivered.
		/// </summary>
		[EnumValue("delivered")]
		[Display(Name = "delivered", ResourceType = typeof(Resources.ShipmentStateLocalization))]
		Delivered,
	}
}
