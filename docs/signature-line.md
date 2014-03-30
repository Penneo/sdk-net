# Signature lines
Every signable document must have at least one signature line. Think of it as the dashed line that people used to sign using a pen...

## Creating a signature line
A signature line is always linked to a document and can't exist on its own. On construction of a signature line object, a document object must be passed to the signature line constructor.
The example below illustrates how to link a new signature line to an existing document object in _myDocument_:

```csharp
// Create a new signature line object and set the role, that the signer will sign as
var mySignatureLine = new SignatureLine(myDocument, "attorney");

// Finally, persist the new object
mySignatureLine.Persist();
```

## Linking a signer to a signature line
To link a signer to a signature line, simply use the __SetSigner()__ method on the signature line object. Note that the signer has to be linked to the same case file as the document containing the signature line is.

```csharp
// Link an existing signer to the signature line
mySignatureLine.SetSigner(mySigner);
```