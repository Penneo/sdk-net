# Templates
Case files can be very complex and the configuration of the documents and signers (who is going to sign what) is often non-trivial.

Therefore Penneo has an archive of common case file types that can be used directly as templates. When using a template you only specify the documents and the signers of the case file, and their respective types. Then Penneo makes sure that everything is mapped together with the correct signature lines.

## Creating a case file from a template
Let's try to create a new case file based on a pre-existing case file template:

```csharp
// Create a new case file
var myCaseFile = new CaseFile("Example Case File");

// Get the case file templates available to the authenticated user
var availableTemplates = (await myCaseFile.GetTemplates()).Objects;

// Assign the first list item as the case file template
myCaseFile.SetCaseFileTemplate(availableTemplates.First());

await myCaseFile.Persist(connector)

```

### Adding documents
When using a templated case file, every document assigned to the case file must be given a type. As above, the available types can be extracted from the case file object, once it has been persisted. Let's look at an example:

```csharp
// Create a new document
var myDocument = new Document(myCaseFile);

// Get available document types from the case file object
var availableDocumentTypes = await myCaseFile.GetDocumentTypes();

// Lets just assign the first type available
myDocument.SetDocumentType(availableDocumentTypes.First());

await myDocument.Persist(connector);
```

### Adding signers
Like documents, signers must also be given types. The procedure looks almost identical:

```csharp
// Create a new signer
var mySigner = new Signer(myCaseFile);

// Get available signer types from the case file object
availableSignerTypes = await myCaseFile.GetSignerTypes();

// Lets just assign the first type available
mySigner.AddSignerType(availableSignerTypes.First());

await mySigner.Persist(connector);
```

## Sending the case file out for signing
Case file templates can contain restrictions on both documents and signers. A template can fx. require that certain document types are present, or limit the number of signers for a certain signer type.

The case file object has a method for checking templated case files for validation errors:

```csharp
// Check case file for validation errors
var errors = (await myCaseFile.GetErrors()).ToList();

// Check the $errors array to see if any configuration errors were encountered.
if (errors.Any())
{
	Console.WriteLine("The case file configuration has the following problems:");
	foreach(var error in errors)
	{
		Console.WriteLine(error);
	}
}
else 
{
	Console.WriteLine("The case file is error free!");
}

```
