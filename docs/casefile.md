# Case files
The case file object is a container used to bundle documents and signers. Every signing process starts with a case file.

## Creating a new case file
Creating a case file is dead simple: 

```csharp
// Create a new case file object
var myCaseFile = new CaseFile("My brand new case file");

// Finally, persist the object
myCaseFile.Persist();
```

## Sending a case file out for signing
When the case file contains the relevant documents and signers, it has to be "send out for signing" before the signing process can begin. This is accomplished by calling the __send()__ method on the case file object.

If you want to distribute the signing links yourself, use the __activate()__ method instead to activate the case file signing links.

Once the case file has been sent or activated, documents and signers can no longer be added or removed.

## Retrieve existing case files
There is several ways to retrieve case files from Penneo. Available methods for retrieving case files are:

* __Query.Find<Casefile>(int id)__
Find one specific case file by its ID.
* __Query.FindAll<CaseFile>__
Find all case files accessible by the authenticated user.
* __Query.FindBy<Casefile>(Dictionary\<string, object\> criteria = null, Dictionary\<string, string\> orderBy = null, int? limit = null, int? offset = null)__
Find all case files matching _criteria_ ordered by _orderBy_. If _limit_ is set, only _limit_ results are returned. If _offset_ is set, the _offset_ first results are skipped.
Criteria can either be _title_ or _metaData_.
* __Query.FindOneBy<Casefile>(Dictionary\<string, object\> criteria = null, Dictionary\<string, string\> orderBy = null)__
Same as _FindBy_ setting _limit_ = 1 and _offset_ = null
* __Query.FindByTitle<Casefile>(string title, Dictionary\<string, string\> orderBy = null, int? limit, int? offset)__
Same as _FindBy_ using title as criteria.
* __Query.FindOneByTitle<Casefile>(string title, Dictionary\<string, string\> orderBy = null)__
Same as _FindOneBy_ using title as criteria.
* __Query.FindByMetaData<Casefile>(string metaData, Dictionary\<string, string\> orderBy = null, int? limit, int? offset)__
Same as _FindBy_ using metaData as criteria.
* __Query.FindOneByMetaData<Casefile>(string metaData, Dictionary\<string, string\> orderBy = null)__
Same as _FindOneBy_ using metaData as criteria.

Below is a couple of examples:

```csharp
// Retrieve all case files
var myCaseFiles = Query.FindAll<CaseFile>();

// Retrieve a specific case file (by id)
var myCaseFile = Query.Find<CaseFile>(271184);

// Retrieve all case files that contains the word "the" in their title and sort descending on creation date
var myCaseFiles = Query.FindByTitle<CaseFile>(
	"the",
	orderBy: new Dictionary<string, string>(){ {"created", "desc" } }	
);

// Retrieve case files from offset 10 until 110 ordered by title in ascending order
var myCaseFiles = Query.FindBy<CaseFile>(	
	orderBy: new Dictionary<string, string>(){ {"title", "asc" } },
	limit: 10,
	offset: 100
);
```

## Retrieving linked objects
A case file contains both signer and document objects. These objects can be retrieved using the following methods:

* __GetDocuments()__
Returns the documents linked to the case file as an array of document objects.
* __GetSigners()__
Returns the signers linked to the case file as an array of signer objects.
* __FindSigner(int id)__
Find and return a specific signer by _id_.

## State variables
A series state variables are used to describe the case file state over the course of its life time. The methods for retrieving the state variables are described below:

* __GetStatus()__
Returns the status of the case file as a string. Possible status values are:
 * _New_: The case file hasn't been sent out for signing yet
 * _Pending_: The case file is out for signing
 * _Rejected_: One of the signers has rejected to sign
 * _Deleted_: The case file has been out for signing but have since been deleted
 * _Signed_: The case file is signed, but the signed documents are not generated
 * _Completed_: The signing process is completed
* __Created__
Returns the date and time the case file was created as a _DateTime_ object
* __SignIteration__
Returns the current sign iteration. This is only relevant if the signing process is not parallel. In that case, the signing process is broken into iterations.
