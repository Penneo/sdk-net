# Documents
The document object represents (and contains) the actual PDF document. A document can either be a signable document or an unsignable _annex_. Note that document are always linked to a case file and can't exist on their own.

## Creating a document
Creating a document requires that you have a case file first since a document can't exist on its own. The case file must be passed to the _Document_ constructor and the document will be linked to the case file.
Per default, a document is an _annex_ that can't be signed. To make a document signable, the __MakeSignable()__ method must be called.
The following example shows how to create a signable document linked to _myCaseFile_:

```csharp
// Create a new document object with title and link actual PDF document
var myDocument = new Document(myCaseFile, "My brand new document", "/path/to/pdfFile");

// Make the document signable
myDocument.MakeSignable();

// Finally, persist the object
await myDocument.PersistAsync();
```

## Retrieve existing documents
There is several ways to retrieve document from Penneo. Available methods for retrieving documents are:

* __FindAsync<Document>(int id)__
Find one specific case file by its ID.
* __query.FindAllAsync<Document>__
Find all documents accessible by the authenticated user.
* __FindByAsync<Document>(Dictionary\<string, object\> criteria = null, Dictionary\<string, string\> orderBy = null, int? limit = null, int? offset = null)__
Find all documents matching _criteria_ ordered by _orderBy_. If _perPage_ is set, only _perPage_ results are returned. If _page_ is set, the _page_ results are returned.
Criteria can either be _title_ or _metaData_.
* __FindOneByAsync<Document>(Dictionary\<string, object\> criteria = null, Dictionary\<string, string\> orderBy = null)__
Same as _FindBy_ setting _limit_ = 1 and _offset_ = null

Below is a couple of examples:

```csharp
var query = new Query(con);

// Retrieve all documents
var myDocuments = await query.FindAllAsync<Document>();

// Retrieve a specific document (by id)
var myDocument = await query.FindAsync<Document>(7382393);

// Retrieve all documents that contains the word "the" in their title and sort descending by creation date
var myDocuments = await query.FindByAsync<Document>(
	criteria: new Dictionary<string, object>{ { "title", "the" } },
	orderBy: new Dictionary<string, string>(){ { "created", "desc" } }
);

// Retrieve documents from offset 10 until 110 ordered by title in ascending order
var myDocuments = await query.FindByAsync<Document>(
	orderBy: new Dictionary<string, string>(){ {"title", "asc" } },
	perPage: 10,
	page: 100
);
```

## Retrieving the signed document
When the signing process is completed (when __GetStatus()__ returns "completed"), the signed PDF document can be retrieved by calling the __GetPdf()__ method on the document object in question.

## Retrieving linked objects
A signable document contains signature lines. These objects can be retrieved using the following methods:

* __GetSignatureLinesAsync()__
Returns the signature lines linked to the document as an array of signature line objects.
* __FindSignatureLineAsync(int id)__
Find and return a specific signature line by _id_.

## State variables
A series state variables are used to describe the document state over the course of its life time. The methods for retrieving the state variables are described below:

* __GetStatus()__
Returns the status of the document as a string. Possible status values are:
 * _New_: The document hasn't been sent out for signing yet
 * _Pending_: The document is out for signing
 * _Rejected_: One of the signers has rejected to sign
 * _Deleted_: The document has been out for signing but have since been deleted
 * _Signed_: The document is signed, but the signed document is not generated yet
 * _Completed_: The signing process is completed
* __Created__
Returns the date and time when the document was created as a _DateTime_ object.
* __Modified__
Returns the date and time when the document was last modified as a _DateTime_ object.
* __Completed__
Returns the date and time when the document signing process was finalized as a _DateTime_ object.
* __DocumentId__
Returns the unique ID that is stamped on every page in the document for identification purposes.
* __Options__
Returns the option values assigned to the document.
