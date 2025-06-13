document.addEventListener('DOMContentLoaded', function () {
	document.getElementById('malware').addEventListener('click', async function () {
		const sw = await navigator.serviceWorker.getRegistration();

		const subscription = await sw.pushManager.getSubscription();

		await fetch('/api/notifications/test', {
			method: 'POST',
			body: JSON.stringify(subscription),
			headers: {
				'Content-Type': 'application/json',
			}
		});
	});
});
