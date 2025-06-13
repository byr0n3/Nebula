self.importScripts('./service-worker-assets.js');
self.importScripts('./service-worker.shared.js');

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [/\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.webp$/, /\.ico$/, /\.webmanifest$/];
const offlineAssetsExclude = [/^service-worker\.js$/];

self.addEventListener('install', (event) => event.waitUntil(onInstall(event)));
self.addEventListener('activate', (event) => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', (event) => event.respondWith(onFetch(event)));

const base = '/';
const baseUrl = new URL(base, self.origin);
const manifestUrlList = self.assetsManifest.assets.map(asset => new URL(asset.url, baseUrl).href);

// Create new cache.
async function onInstall(event) {
	const assetsRequests = self.assetsManifest.assets
		.filter((asset) => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
		.filter((asset) => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
		.map((asset) => new Request(asset.url, {integrity: asset.hash, cache: 'no-cache'}));

	await caches.open(cacheName).then((cache) => cache.addAll(assetsRequests));
}

// Delete unused caches.
async function onActivate(event) {
	const cacheKeys = await caches.keys();

	await Promise.all(
		cacheKeys
			.filter((key) => key.startsWith(cacheNamePrefix) && key !== cacheName)
			.map((key) => caches.delete(key))
	);
}

// Serve cashed assets when available.
async function onFetch(event) {
	let cachedResponse = null;

	if (event.request.method === 'GET') {
		const shouldServeIndexHtml = (event.request.mode === 'navigate') && !manifestUrlList.some((url) => url === event.request.url);

		const request = shouldServeIndexHtml ? 'index.html' : event.request;
		const cache = await caches.open(cacheName);
		cachedResponse = await cache.match(request);
	}

	return cachedResponse || fetch(event.request);
}
