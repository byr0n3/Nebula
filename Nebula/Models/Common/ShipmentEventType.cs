using System.ComponentModel.DataAnnotations;
using Elegance.Enums;

namespace Nebula.Models.Common
{
	/// <summary>
	/// <p>The type of event that happened.</p>
	/// <p>This could be a shipment state update, but also a manual change.</p>
	/// </summary>
	[Enum]
	public enum ShipmentEventType
	{
		/// <inheritdoc cref="ShipmentState.Registered"/>
		[EnumValue("registered")]
		[Display(Name = "registered", ResourceType = typeof(Resources.ShipmentStateLocalization))]
		Registered = 0,

		/// <inheritdoc cref="ShipmentState.Received"/>
		[EnumValue("received")]
		[Display(Name = "received", ResourceType = typeof(Resources.ShipmentStateLocalization))]
		Received,

		/// <inheritdoc cref="ShipmentState.Sorted"/>
		[EnumValue("sorted")]
		[Display(Name = "sorted", ResourceType = typeof(Resources.ShipmentStateLocalization))]
		Sorted,

		/// <inheritdoc cref="ShipmentState.OutForDelivery"/>
		[EnumValue("out_for_delivery")]
		[Display(Name = "out_for_delivery", ResourceType = typeof(Resources.ShipmentStateLocalization))]
		OutForDelivery,

		/// <inheritdoc cref="ShipmentState.Delivered"/>
		[EnumValue("delivered")]
		[Display(Name = "delivered", ResourceType = typeof(Resources.ShipmentStateLocalization))]
		Delivered,

		/// <summary>
		/// The receiver/sender updated some information about the shipment.
		/// </summary>
		[EnumValue("information_update")]
		[Display(Name = "information_update", ResourceType = typeof(Resources.ShipmentStateLocalization))]
		InformationUpdate,
	}
}
