# Webhook Subscriptions
You can setup webhook subscriptions to get notified when certain happens happen in Penneo Sign.
You can use https://webhook.site to create a target url to test your webhook subscriptions.


If for some reason your servers don't respond with a OK range header (200-299), our system will re-try the request a few times before giving up.

## Setting up a webhook subscription
```csharp
    var hook = new WebhookSubscription()
    {
        Endpoint = "https://example.com",
        EventTypes = new[] { EventType.CaseFileCompleted, EventType.SignerSigned }
    };
    await hook.PersistAsync(connector);
```

See `EventType` for a list of possible event types.

If the call was successful, you will get a `WebhookSubscription` object back with the `Id` field set.
You should now receive notifications to the specified endpoint. The notifications will be sent as a POST request with a JSON body.
Example of a notification payload:
```json
{
  "topic": "casefile",
  "eventType": "completed",
  "eventTime": {
    "date": "2025-02-19 13:01:55.517791",
    "timezone_type": 3,
    "timezone": "UTC"
  },
  "payload": {
    "id": 531,
    "status": 2
  }
}
```
The request will have an `x-event-type` header with the event type, `x-event-id` with a unique id for the event and `x-penneo-signature` with a timestamp and signature of the payload.
> Important: The endpoint must be a valid URL that can receive POST requests over the internet.

## Finding webhook subscriptions
```csharp
var hook = await query.FindAsync<WebhookSubscription>(webhookId);
var allHooks = await query.FindAllAsync<WebhookSubscription>();
```


## Deleting a webhook subscription
```csharp
await hook.DeleteAsync(con);
```
