using Npgsql;
using ShipmentTracker.Common.Models;

namespace ShipmentTracker.Database.Internal.Translators
{
	internal sealed class ShipmentStateNameTranslator : INpgsqlNameTranslator
	{
		public static readonly ShipmentStateNameTranslator Instance = new();

		public string TranslateTypeName(string clrName) =>
			"shipment_state";

		public string TranslateMemberName(string clrName) =>
			ShipmentStateEnumData.FromLabel(clrName);
	}
}
