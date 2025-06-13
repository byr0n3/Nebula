using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Elegance.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShipmentTracker.WebPush.Internal;
using ShipmentTracker.WebPush.Internal.Models;

namespace ShipmentTracker.WebPush
{
	public sealed class VapidClient
	{
		private readonly HttpClient client;
		private readonly VapidOptions options;
		private readonly ILogger<VapidClient> logger;

		public VapidClient(HttpClient client, IOptions<VapidOptions> options, ILogger<VapidClient> logger)
		{
			this.client = client;
			this.logger = logger;
			this.options = options.Value;
		}

		public async Task<bool> SendAsync(PushSubscription subscription, PushNotification notification, CancellationToken token = default)
		{
			const int defaultTtl = 2419200;

			var jwtToken = Jwt.GetSignedToken(subscription.Endpoint, this.options);

			var declarativeNotification = new DeclarativePushNotification
			{
				WebPush = 8030,
				Notification = notification,
			};

			var payload = Encryption.Encrypt(subscription, declarativeNotification, this.options);

			var content = new ByteArrayContent(payload.Payload);
			{
				content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
				content.Headers.ContentLength = payload.Payload.Length;
				content.Headers.ContentEncoding.Add("aesgcm");
			}

			var request = new HttpRequestMessage(HttpMethod.Post, subscription.Endpoint);
			{
				// @todo Configurable
				request.Headers.TryAddWithoutValidation("TTL", defaultTtl.Str());

				request.Content = content;
				request.Headers.TryAddWithoutValidation("Encryption", $"salt={UrlSafeBase64.Encode(payload.Salt)}");
				request.Headers.TryAddWithoutValidation("Authorization", $"WebPush {jwtToken}");
				request.Headers.TryAddWithoutValidation(
					"Crypto-Key", $"dh={UrlSafeBase64.Encode(payload.PublicKey)};p256ecdsa={this.options.PublicKey}");
			}

			using (request)
			using (var response = await this.client.SendAsync(request, token).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode)
				{
					var msg = await response.Content.ReadAsStringAsync(token).ConfigureAwait(false);
					this.logger.LogError("Error sending push notification: {Msg}", msg);
				}

				return response.IsSuccessStatusCode;
			}
		}
	}
}
