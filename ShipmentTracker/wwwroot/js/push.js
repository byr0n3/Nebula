async function handlePushSubscription() {
	/** @type {PushSubscriptionOptionsInit}*/
	const options = {
		userVisibleOnly: true,
		// @todo From app settings/config
		applicationServerKey: 'BOTRi2tLFCqBlsU77UcB3ddhNwueIEIVXXi2RIuCrkIQxDRVQo2SKuHrfIwBgrli08l2JyavfiO81zkGt6W_ECU',
	};

	const sw = await navigator.serviceWorker.getRegistration();

	const state = await sw.pushManager.permissionState(options);

	const subscription = (state === 'granted') ? await sw.pushManager.getSubscription() : await sw.pushManager.subscribe(options);

	console.log(JSON.stringify(subscription));
}

void handlePushSubscription();
