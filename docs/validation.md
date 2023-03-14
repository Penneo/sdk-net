# Validations
Money laundering regulations require companies to validate the identity of their clients. The validation object can accomplish this, using only a social security number and an electronic ID.

The process and the validation object is very similar to the signing request. And like the signing request, the validation link can be send out via Penneo by email, or you can choose to distribute it to the user in your own system.

## Distributing the validation link through Penneo
Penneo can distribute the validation link for you. The link will be send out to the user in an email through Penneos infrastructure.

The process is best explained by an example:

```csharp
// Create a new validation with details of the user to validate
var myValidation = new Validation("John Doe", "john@doe.com");
myValidation.Title = "My validation";

// Define the content of the email
myValidation.EmailSubject = "Validation inquiry";
myValidation.EmailText = "Dear john. Please validate yourself using this link.";

// Persist the new validation object
await myValidation.Persist(connector);

// Finally, send out the validation link
await myValidation.Send(connector);
```

### Reminder emails
When using Penneo to distribute validation links, it is also possible to have Penneo remind the person regularly by email, until the he/she completes the validation. To set up a reminder, just use the __setReminderInterval()__ method to set the number of days between reminders.

## Distributing the validation link yourself
If you don't want Penneo to distribute your validation links, you can handle the process yourself. All you need to do is to fetch the link from the validation object:

```csharp
// Create a new validation with details of the user to validate
var myValidation = new Validation("John Doe");
myValidation.Title = "My validation";

// Persist the new validation object
await myValidation.Persist(connector);

// Activate the validation object
await myValidation.Send(Connector);

// Retrieve the validation link that you would like to distribute
var myLink = await myValidation.GetLink(connector);

// In case you would like to re-send the validation request from Penneo at a later point, you need to set the email details
myValidation.Email = "john@doe.com";
myValidation.EmailSubject = "Validation inquiry";
myValidation.EmailText = "Dear john. Please validate yourself using this link.";

// Persist the new validation object
await myValidation.Persist(connector);

```

Note that the validation link won't be active until you call the __Send()__ method on the validation object.

## Customizing the validation process
When the user completes the validation process, the signer is redirected to the default Penneo success page. You can choose to use your own custom status pages instead. All you need to do is pass the urls to the validation object like so:

```csharp
// Set the url for the custom success page
myValidation.SuccessUrl = "http://go/here/on/success";

// Store the changes to the validation object
await myValidation.Persist(connector);
```

It is also possible to change the default explanatory text provided on the validation web-page to better fit your companies or customers validation use case. You can set a custom text like so:

```csharp
// Set the url for the custom success page
myValidation.CustomText = "Here is my custom text<br>Please validate yourself!";

// Store the changes to the validation object
await myValidation.Persist(connector);
```

The custom text can't contain any HTML tags, except for the <br> tag.



## Retrieving the validation contents
When the user completes the validation process, the contents can be retrieved as follows:

```csharp
await myValidation.GetContents(connector);
```

If the validation is ready (when __GetStatus()__ returns _Ready_), the social security number is returned. However, if the validation is completed (when __GetStatus()__ returns _Completed_), information from the public registry is also returned.

## Retrieving the validation document
Once the validation is completed (when __GetStatus()__ returns _Completed_), the resulting validation document can be retrieved:

```csharp
var pdf = await myValidation.GetPdf(connector);
```

The validation document contains all the information that Penneo has gathered about the validated person.

## State variables
A series state variables are used to describe the validation state over the course of its life time. The methods for retrieving the state variables are described below:

* __GetStatus()__
Returns the status of the validation as a string. Possible status values are:
 * _New_: The validation request hasn't been sent out yet
 * _Pending_: Waiting for the user to complete the validation
 * _Undeliverable_: The validation request email could not be delivered
 * _Deleted_: The validation has been send but have since been deleted
 * _Ready_: The user has completed the validation process, but the validation document is not generated yet
 * _Completed_: The validation process is completed
