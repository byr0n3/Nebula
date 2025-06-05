using Npgsql;
using ShipmentTracker.Common.Models;

namespace ShipmentTracker.Database.Internal.Translators
{
	internal sealed class ShipmentSourceNameTranslator : INpgsqlNameTranslator
	{
		public static readonly ShipmentSourceNameTranslator Instance = new();

		public string TranslateTypeName(string clrName) =>
			"shipment_source";

		public string TranslateMemberName(string clrName) =>
			ShipmentSourceEnumData.FromLabel(clrName);
	}
}
