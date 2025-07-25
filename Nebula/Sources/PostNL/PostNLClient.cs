using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Nebula.Models;
using Nebula.Models.Common;
using Nebula.Sources.PostNL.Extensions;
using Nebula.Sources.PostNL.Json;
using Nebula.Sources.PostNL.Models;

namespace Nebula.Sources.PostNL
{
	public sealed class PostNLClient : IShipmentSource, System.IDisposable
	{
		private static readonly System.Uri baseAddress = new("https://jouw.postnl.nl/track-and-trace/api/", System.UriKind.Absolute);

		public ShipmentSource Source =>
			ShipmentSource.PostNL;

		private readonly HttpClient client;

		public PostNLClient(HttpClient client)
		{
			this.client = client;
			{
				this.client.BaseAddress = PostNLClient.baseAddress;

				this.client.DefaultRequestHeaders.Clear();
				this.client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", MediaTypeNames.Application.Json);
			}
		}

		public async ValueTask<bool> ValidateAsync(ShipmentRequest request, CancellationToken token = default)
		{
			// The PostNL public API sucks and returns an empty object when the shipment hasn't been found, instead of a 404 status code.
			var shipment = await this.GetShipmentAsync(request.Code, request.ZipCode, request.Country, request.Language, token)
									 .ConfigureAwait(false);

			return shipment != default;
		}

		public async ValueTask<Shipment> GetShipmentAsync(ShipmentRequest request, CancellationToken token = default)
		{
			var shipment = await this.GetShipmentAsync(request.Code, request.ZipCode, request.Country, request.Language, token)
									 .ConfigureAwait(false);

			var events = shipment.Observations
								 .Select(static (o) => new ShipmentEvent
								  {
									  State = o.Code.ToShipmentEventType(),
									  Timestamp = o.Date,
									  Description = o.Description,
								  })
								 .OrderByDescending(static (e) => e.Timestamp)
								 .ToArray();

			return new Shipment
			{
				Id = shipment.Id,
				TrackingCode = shipment.TrackingCode,
				Source = this.Source,
				Type = shipment.Context.Type.ToShipmentType(),
				State = shipment.Status.Type.ToShipmentState(),
				Recipient = new ShipmentContact
				{
					Name = shipment.Recipient.Name.Name,
					Street = shipment.Recipient.Address.Street,
					HouseNumber = shipment.Recipient.Address.HouseNumber,
					HouseNumberSuffix = shipment.Recipient.Address.HouseNumberSuffix,
					ZipCode = shipment.Recipient.Address.ZipCode,
					Place = shipment.Recipient.Address.Town,
					Country = CountryEnumData.FromValue(shipment.Recipient.Address.CountryCode),
				},
				Sender = new ShipmentContact
				{
					Name = shipment.Sender.Name.Name,
					Street = shipment.Sender.Address.Street,
					HouseNumber = shipment.Sender.Address.HouseNumber,
					HouseNumberSuffix = shipment.Sender.Address.HouseNumberSuffix,
					ZipCode = shipment.Sender.Address.ZipCode,
					Place = shipment.Sender.Address.Town,
					Country = CountryEnumData.FromValue(shipment.Sender.Address.CountryCode),
				},
				Details = new ShipmentDetails
				{
					Height = shipment.Details.Dimensions.GetValueOrDefault().Height,
					Width = shipment.Details.Dimensions.GetValueOrDefault().Width,
					Length = shipment.Details.Dimensions.GetValueOrDefault().Depth,
					Weight = shipment.Details.Dimensions.GetValueOrDefault().Weight,
				},
				Events = events,
				Created = shipment.Created,
				Updated = shipment.Updated,
				Arrived = shipment.DeliveryDate ?? default,
				Eta = new Range(shipment.EstimatedTimeOfArrival.Start, shipment.EstimatedTimeOfArrival.End),
				// `minutesOverdue` will be `null`/`0` if the shipment isn't overdue yet.
				// The value of `Delay` will be `default` in that case.
				Delay = System.TimeSpan.FromMinutes(shipment.EstimatedTimeOfArrival.MinutesOverdue.GetValueOrDefault()),
				RelatedTrackingCodes = shipment.Related?.Select(static (r) => r.Identification.TrackingCode).ToArray(),
			};
		}

		private async ValueTask<PostNLShipment> GetShipmentAsync(string code,
																 string zipCode,
																 Country country,
																 Language language,
																 CancellationToken token)
		{
			var path = string.Create(
				null,
				stackalloc char[128],
				$"trackAndTrace/{code}-{CountryEnumData.GetValue(country)}-{zipCode}?language={LanguageEnumData.GetValue(language)}"
			);

			using (var request = new HttpRequestMessage(HttpMethod.Get, path))
			using (var response = await this.client.SendAsync(request, token).ConfigureAwait(false))
			{
				var data = await response.Content
										 .ReadFromJsonAsync(PostNLJsonSerializerContext.Default.PostNLShipmentResponse!, token)
										 .ConfigureAwait(false);

				return data.Data.GetValueOrDefault(code);
			}
		}

		public void Dispose() =>
			this.client.Dispose();
	}
}
