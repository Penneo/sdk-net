# Signing requests
Think of the signing request as being the instructions for the signer on what to sign. It can either be the formal letter accompanying the document, the yellow post-its showing where to sign, or both. Using Penneo, the envelope containing the contract is replaced by a hyperlink. This link points to the Penneo signing portal, where the signer can go and sign.

So, before the signer can actually sign anything, he/she needs a link to the Penneo signing portal. This link can be distributed in two ways. Either you let Penneo handle the distribution of a signing link via email, or you handle the distribution yourself. Both approaches are handled through the signing request.

It should be noted, that a signing request is implicitly created, every time a new signer is created.

## Distributing the signing link through Penneo
Penneo can distribute the signing link for you. The link will be send out to the signer in an email through Penneos infrastructure.

The process is best explained by an example. Lets say that we have a signing request in _mySigningRequest_ that we wish to deliver via Penneo:

```csharp
// Set the signers email address
mySigningRequest.Email = "john@doe.com";

// Define the content of the email
mySigningRequest.EmailText = "Dear john. Please sign the contract.";

// Store the changes to the signing request
mySigningRequest.Persist(mySigningRequest);
```

Note that the signing request emails won't actually be send out until you call the __Send()__ method on the owning case file object.

If you need to re-send the signing request email (fx. due to a change in the email address), all you need to do is call the __Send()__ method on the signing request object like so:

```csharp
// Re-send the signing request email
mySigningRequest.Send();
```

## Distributing the signing link yourself
If you don't want Penneo to distribute your signing links, you can handle the process yourself. All you need to do is to fetch the link from the signing request object:

```csharo
var myLink = mySigningRequest.GetLink();
```

Note that the signing link won't be active until you send out the case file by calling the __Send()__ method on the owning case file object.

## Customizing the signing process
When the signer completes an action on the Penneo signing portal (that is, he/she signs or rejects to sign), the signer is redirected to the default Penneo success/failure page. You can choose to use your own custom status pages instead. All you need to do is pass the urls to the signing request like so:

```csharp
// Set the url for the custom success page
mySigningRequest.SuccessUrl = "http://go/here/on/success";

// Set the url for the customer failure/reject page
mySigningRequeset.FailUrl = "http://go/here/on/failure";

// Store the changes to the signing request
mySigningRequeset.Persist();
```

## State variables
A series state variables are used to describe the signing state. The methods for retrieving the state variables are described below:

* __GetStatus()__
Returns the status of the signer as a string. Possible status values are:
 * _New_: The signing request hasn't been sent out yet
 * _Pending_: The signer still needs to sign something.
 * _Rejected_: The signer has rejected to sign
 * _Deleted_: The signing request has been distributed, but has since been deleted.
 * _Signed_: The signer is done signing.
 * _Undeliverable_: The signing request email could not be delivered
* __RejectReason__
Returns the reason, given by the signer, for rejecting to sign the documents in the case file. The response is only valid, if the status of the signing request is _rejected_.
