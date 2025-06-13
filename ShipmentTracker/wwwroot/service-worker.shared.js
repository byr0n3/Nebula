/**
 * @typedef {Object} PushNotification
 * @property {string} title
 * @property {string} lang
 * @property {'auto'|'ltr'|'rtl'} dir
 * @property {string} body
 * @property {string} navigate
 * @property {boolean} silent
 * @property {string} app_badge
 */

self.addEventListener('push', function (event) {
	event.waitUntil(showNotification(event));
});

async function showNotification(event) {
	const data = await event.data.json();
	/** @type {PushNotification} */
	const notification = data.notification;

	await self.registration.showNotification(notification.title, {
		lang: notification.lang,
		dir: notification.dir,
		body: notification.body,
		silent: notification.silent,
		data: {
			url: notification.navigate,
		}
	});

	// @todo Handle badging
}
