namespace ShipmentTracker.WebPush.Internal.Models
{
	public readonly struct EncryptionResult
	{
		public required byte[] Salt { get; init; }

		public required byte[] Payload { get; init; }

		public required byte[] PublicKey { get; init; }
	}
}
