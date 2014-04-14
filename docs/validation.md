# Validations
Money laundering regulations require companies to validate the identity of their clients. The validation object can accomplish this, using only a social security number and an electronic ID.

The process and the validation object is very similar to the signing request. And like the signing request, the validation link can be send out via Penneo by email, or you can choose to distribute it to the user in your own system.

## Distributing the validation link through Penneo
Penneo can distribute the validation link for you. The link will be send out to the user in an email through Penneos infrastructure.

The process is best explained by an example:

```csharp
// Create a new validation with details of the user to validate
myValidation = new Validation("John Doe", "john@doe.com");
myValidation.Title = "My validation";

// Define the content of the email
myValidation.EmailSubject = "Validation inquiry";
myValidation.EmailText = "Dear john. Please validate yourself using this link.";

// Persist the new validation object
myValidation.Persist();

// Finally, send out the validation link
myValidation.Send();
```

## Distributing the validation link yourself
If you don't want Penneo to distribute your validation links, you can handle the process yourself. All you need to do is to fetch the link from the validation object:

```csharp
// Create a new validation with details of the user to validate
myValidation = new Validation("John Doe");
myValidation.Title = "My validation";

// Persist the new validation object
myValidation.Persist();

// Activate the validation object
myValidation.Send();

// Retrieve the validation link
myLink = myValidation.GetLink();;
```

Note that the validation link won't be active until you call the __Send()__ method on the validation object.

## Retrieving the validation document
Once the validation is completed (when __GetStatus()__ returns _Completed_), the resulting validation document can be retrieved:

```csharp
myValidation.GetPdf();
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
