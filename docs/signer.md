# Signers
A signer object represents the person that signs. Apart from the obvious name, a signer can also be assigned and organisation name (that he/she signs on behalf of) and a social security number or VAT identification number to verify his/her identity.

## Creating a signer
A signer is always linked to a case file and can't exist on its own. On construction of a signer object, a case file object must be passed to the signer constructor.
The example below illustrates how to create a new signer on an existing case file object in _myCaseFile_:

```csharp
// Create a new signer object, and set the signer name
var mySigner = new Signer(myCaseFile, "John Doe");

// Set the optional organisation to sign on behalf of
mySigner.OnBehalfOf = "Acme Corporation";

// Set the optional social security number
mySigner.SocialSecurityNumber = "0101501111";

// Set the optional VAT identification number
mySigner.VATIdentificationNumber = 12345678;

// Finally, persist the new object
mySigner.Persist();
```

## Creating a signer with delayed sendout, expiry or a role lable:

```csharp
var mySigner = new Signer(myCaseFile, "John Doe");
mySigner.Persist();

// This is similar to mySigner.addSignerType(), but allows for finer graned control
var map = new SignerTypeMap
{
    Signer = mySigner,
    SignerTypeId = 237,
    ActiveAt = DateTime.Today.AddDays(70),
    Role = "Animator",
    ExpireAt = DateTime.Today.AddDays(90),
};

map.Persist();
// Be careful, persisting the map multiple times will make the signer have to sign multiple times
```

## Retrieving the signing request
Every time you create a new signer, a signing request is also generated for the new signer. After persisting the new signer object, you can retrieve the signing request like so:

```csharp
// Retrieve the signing request object
mySigningRequest = mySigner.GetSigningRequest();
```
