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

	let subscription = await sw.pushManager.getSubscription();

	if (!subscription) {
		subscription = await sw.pushManager.subscribe(options);
	}

	const response = await fetch('/api/push/subscribe', {
		method: 'POST',
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
