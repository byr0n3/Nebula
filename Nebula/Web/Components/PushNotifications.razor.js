/**
 * @typedef {string} VAPID_PUBLIC_KEY
 */

async function initialize() {
	/** @type { PushSubscriptionOptionsInit} */
	const options = {
		userVisibleOnly: true,
		applicationServerKey: VAPID_PUBLIC_KEY,
	}

	const sw = await navigator.serviceWorker.getRegistration();

	const subscription = await sw.pushManager.getSubscription();

	const buttons = document.querySelectorAll('button[data-push-notifications]');

	if (subscription) {
		await subscribe(subscription);

		buttons.forEach((btn) => btn.remove());
	} else {
		buttons.forEach(function (btn) {
			btn.hidden = false;

			btn.addEventListener('click', async function (e) {
				e.preventDefault();

				const subscription = await sw.pushManager.subscribe(options);

				await subscribe(subscription);

				buttons.forEach((btn) => btn.remove());
			});
		})
	}
}

/**
 * @param subscription {PushSubscription}
 */
async function subscribe(subscription) {
	const response = await fetch('/api/push/subscribe', {
		method: 'POST',
		credentials: 'same-origin',
		headers: {
			'Accept': 'application/json',
			'Content-Type': 'application/json',
		},
		body: JSON.stringify(subscription),
	});

	if (response.status !== 201) {
		throw new Error(`Unable to subscribe for push notifications: ${JSON.stringify(response)}`);
	}
}

void initialize();
