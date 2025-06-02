# Parcel Tracker

To-be-named parcel tracking app, able to track parcels from multiple delivery companies.

## Integration

### PostNL

Packages can be publicly retrieved using this public API call:

https://jouw.postnl.nl/track-and-trace/api/trackAndTrace/{TRACKING_CODE}-{COUNTRY_CODE}-{ZIP_CODE}?language={LANGUAGE}

‘Country code’ and ‘Zip code’ are used as a form of validation, which seems to only be to prevent randomly/accidentally
finding a valid tracking code, as there are no other security measures.

Unlike DHL, no additional headers are required to get a valid response.

[//]: # (@todo Figure out how to track letters)

### DHL

Packages can be publicly retrieved using this public API call:

https://my.dhlecommerce.nl/receiver-parcel-api/track-trace?key={TRACKING_CODE}%2B{ZIP_CODE}&role=consumer-receiver

'Zip code' is used as a form of validation.

The server requires the following headers for the request to be accepted:
‘Accept’ - application/json, */* (‘*/*’ is needed, or the response will be empty for some reason)
‘X-XSRF-TOKEN’ - Needs to have a value, can be any value

[//]: # (@todo UPS)

## Development

### Database

```shell
createuser --pwprompt parcel_tracker # Make the pwd 'parcel_tracker'
createdb --owner parcel_tracker parcel_tracker
```
