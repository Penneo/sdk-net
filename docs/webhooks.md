# Webhook Subscriptions
You set up Penneo to call your servers on certain actions like a signer signing or a case file finalizing.

You can use https://webhook.site to play around with the system.
More info about our webhooks can be found here https://penneo.readme.io/docs/webhooks.


If for some reason your servers don't respond with a OK range header (200-299), our system will re-try the request a few times before giving up.

## Setting up a webhook subscription
```csharp
var hook = new WebhookSubscription();
hook.Endpoint = "https://your url .your domain/path";
hook.Topic = "casefile";

hook.Persist(con);
```

Currently, the `casefile` and `signer` topics are the only ones supported.

After calling `.Persist()`, your servers will receive a HTTP call which will have a `confirmationToken` field in the body.
> Note: the body of the request is a JSON object, even though the proper `application/json` header might not be set.


## Confirming a webhook subscription
```csharp
var hook = query.Find<WebhookSubscription>(webhookId);

hook.Confirm(confirmationToken);
```

If everything was successful, your servers will now be called on case file/signer updates.

## Finding webhook subscriptions
```csharp
var hook = query.Find<WebhookSubscription>(webhookId);
var allHooks = query.FinaAll<WebhookSubscription>();
```


## Deleting a webhook subscription
```csharp
hook.Delete(con);
```
