using Npgsql;
using Nebula.Models.Common;

namespace Nebula.Npgqsl.Translators
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
