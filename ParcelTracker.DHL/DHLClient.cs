using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ParcelTracker.Common;
using ParcelTracker.Common.Models;
using ParcelTracker.DHL.Internal.Extensions;
using ParcelTracker.DHL.Models;

namespace ParcelTracker.DHL
{
	public sealed class DHLClient : IShipmentSource, System.IDisposable
	{
		private static readonly System.Uri baseAddress = new("https://my.dhlecommerce.nl/receiver-parcel-api/", System.UriKind.Absolute);

		public ShipmentSource Source =>
			ShipmentSource.DHL;

		private readonly HttpClient client;

		public DHLClient(HttpClient client)
		{
			this.client = client;
			{
				this.client.BaseAddress = DHLClient.baseAddress;

				this.client.DefaultRequestHeaders.Clear();
				this.client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", MediaTypeNames.Application.Json);
				this.client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
				// Server validates that a XSRF token is present, yet doesn't validate the token itself.
				this.client.DefaultRequestHeaders.TryAddWithoutValidation("X-XSRF-Token", "a");
			}
		}

		// @todo Validate code using Regex
		public async ValueTask<bool> ValidateAsync(ShipmentRequest request, CancellationToken token = default)
		{
			var response = await this.GetShipmentAsync(request.Code, request.ZipCode, token).ConfigureAwait(false);

			using (response)
			{
				return response.IsSuccessStatusCode;
			}
		}

		public async ValueTask<Shipment> GetShipmentAsync(ShipmentRequest request, CancellationToken token = default)
		{
			DHLShipment shipment;

			var response = await this.GetShipmentAsync(request.Code, request.ZipCode, token).ConfigureAwait(false);

			using (response)
			{
				var data = await response.Content
										 .ReadFromJsonAsync(Internal.DHLJsonSerializerContext.Default.DHLShipmentArray!, token)
										 .ConfigureAwait(false);

				shipment = (data is not null) && (data.Length != 0) ? data[0] : default;
			}

			// @todo Phase `Underway` contains both `Received` and `Sorted` shipment states
			var state = shipment.View
								.Phases
								.Where(static (p) => p.Completed)
								.Select(static (p) => p.Phase.ToShipmentState())
								.LastOrDefault();

			return new Shipment
			{
				Id = shipment.Id,
				TrackingCode = shipment.TrackingCodes[^1],
				Source = this.Source,
				// @todo
				Type = ShipmentType.Unknown,
				State = state,
				Recipient = new ShipmentContact
				{
					Name = shipment.Recipient.Name,
					Street = shipment.Recipient.Address.Street,
					HouseNumber = shipment.Recipient.Address.HouseNumber,
					HouseNumberSuffix = shipment.Recipient.Address.HouseNumberAddition,
					ZipCode = shipment.Recipient.Address.ZipCode,
					Place = shipment.Recipient.Address.City,
					Country = CountryEnumData.FromValue(shipment.Recipient.Address.CountryCode),
				},
				Sender = new ShipmentContact
				{
					Name = shipment.Sender.Name,
					Street = shipment.Origin.Address.Street ?? string.Empty,
					HouseNumber = shipment.Origin.Address.HouseNumber ?? string.Empty,
					HouseNumberSuffix = shipment.Origin.Address.HouseNumberAddition ?? string.Empty,
					ZipCode = shipment.Origin.Address.ZipCode ?? string.Empty,
					Place = shipment.Origin.Address.City ?? string.Empty,
					Country = CountryEnumData.FromValue(shipment.Origin.Address.CountryCode),
				},
				Dimensions = new ShipmentDimensions
				{
					// Dimensions are given in CM, convert to MM.
					Height = shipment.Height * 10,
					Width = shipment.Width * 10,
					Length = shipment.Length * 10,
					// `Weight` is given in KG, convert to G.
					Weight = shipment.Weight * 1000,
				},
				// @todo
				Events = null,
				Created = shipment.Created,
				Updated = shipment.Updated,
				Arrived = shipment.DeliveryDate,
				EstimatedTimeOfArrival = shipment.EstimatedDeliveryTime,
				Delay = default,
			};
		}

		[MustDisposeResource]
		private async ValueTask<HttpResponseMessage> GetShipmentAsync(string code, string zipCode, CancellationToken token = default)
		{
			var path = string.Create(
				null,
				stackalloc char[128],
				$"track-trace?key={code}%2B{zipCode}&role=consumer-receiver"
			);

			using (var request = new HttpRequestMessage(HttpMethod.Get, path))
			{
				return await this.client.SendAsync(request, token).ConfigureAwait(false);
			}
		}

		public void Dispose() =>
			this.client.Dispose();
	}
}
