/**
 * @typedef {Object} PushNotification
 * @property {string} title
 * @property {string} lang
 * @property {'auto'|'ltr'|'rtl'} dir
 * @property {string} body
 * @property {string} navigate
 * @property {string|undefined} topic
 * @property {boolean} silent
 * @property {string|undefined} app_badge
 */

self.addEventListener('push', function (event) {
	event.waitUntil(showNotification(event));
});

self.addEventListener('notificationclick', function (event) {
	event.notification.close();

	event.waitUntil(onNotificationClick(event));
});

async function showNotification(event) {
	if (!event.data) {
		throw new Error('Received push event without any data.');
	}

	const data = await event.data.json();
	/** @type {PushNotification} */
	const notification = data.notification;

	await self.registration.showNotification(notification.title, {
		icon: '/icon-512.png',
		lang: notification.lang,
		dir: notification.dir,
		body: notification.body,
		silent: notification.silent,
		tag: notification.topic,
		data: {
			navigate: notification.navigate,
		}
	});
}

async function onNotificationClick(event) {
	// @todo `event.action`

	/** @type {Notification} */
	const notification = event.notification;

	const url = notification.data.navigate;

	const windows = await clients.matchAll({type: 'window'});

	for (const client of windows) {
		if (client.url === url && 'focus' in client) {
			return await client.focus();
		}
	}

	if (clients.openWindow) {
		return clients.openWindow(url);
	}
}
