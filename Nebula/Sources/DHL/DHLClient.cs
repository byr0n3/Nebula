using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nebula.Extensions;
using Nebula.Models.Common;
using Nebula.Sources.DHL.Extensions;
using Nebula.Sources.DHL.Json;
using Nebula.Sources.DHL.Models;

namespace Nebula.Sources.DHL
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

		// @todo Validate code using Regex?
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
										 .ReadFromJsonAsync(DHLJsonSerializerContext.Default.DHLShipmentArray!, token)
										 .ConfigureAwait(false);

				shipment = (data is not null) && (data.Length != 0) ? data[0] : default;
			}

			var events = shipment.Events
								 .Select(ToShipmentEvent)
								 .OrderByDescending(static (e) => e.Timestamp)
								 .ToArray();

			var state = events.Where(static (e) => e.State != ShipmentEventType.InformationUpdate)
							  .Select(static (e) => e.State.ToShipmentState())
							  .First();

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
				Details = new ShipmentDetails
				{
					// Dimensions are given in CM, convert to MM.
					Height = shipment.Height * 10,
					Width = shipment.Width * 10,
					Length = shipment.Length * 10,
					// `Weight` is given in KG, convert to G.
					Weight = shipment.Weight * 1000,
				},
				Events = events,
				Created = shipment.Created,
				Updated = shipment.Updated,
				Arrived = shipment.DeliveryDate,
				Eta = shipment.EstimatedDeliveryTime,
				// @todo Is this a thing?
				Delay = default,
			};

			static ShipmentEvent ToShipmentEvent(DHLShipmentEvent @event)
			{
				var state = (@event.Status) switch
				{
					DHLShipmentEventStatus.Registered => ShipmentEventType.Registered,
					DHLShipmentEventStatus.Received   => ShipmentEventType.Received,
					DHLShipmentEventStatus.Sorted     => ShipmentEventType.Sorted,
					_                                 => @event.Category.ToShipmentEventType(),
				};

				return new ShipmentEvent
				{
					State = state,
					Timestamp = @event.Timestamp,
					Description = @event.Remarks,
				};
			}
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
