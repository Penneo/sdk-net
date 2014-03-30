# Documents
The document object represents (and contains) the actual PDF document. A document can either be a signable document or an unsignable _annex_. Note that document are always linked to a case file and can't exist on their own.

## Creating a document
Creating a document requires that you have a case file first since a document can't exist on its own. The case file must be passed to the _Document_ constructor and the document will be linked to the case file.
Per default, a document is an _annex_ that can't be signed. To make a document signable, the __makeSignable()__ method must be called.
The following example shows how to create a signable document linked to _$myCaseFile_:

```php
// Create a new document object
$myDocument = new Document($myCaseFile);

// Set the document title
$myDocument->setTitle('My brand new document');

// Add the actual PDF document
$myDocument->setPdfFile('/path/to/pdfFile');

// Make the document signable
$myDocument->makeSignable();

// Finally, persist the object
Document::persist($myDocument);
```

## Retrieve existing documents
There is several ways to retrieve document from Penneo. Available methods for retrieving documents are:

* __Document::find($id)__
Find one specific document by its ID.
* __Document::findAll()__
Find all documents accessible by the authenticated user.
* __Document::findBy(array $criteria, array $orderBy, $limit, $offset)__
Find all documents matching _$criteria_ ordered by _$orderBy_. If _$limit_ is set, only _$limit_ results are returned. If _$offset_ is set, the _$offset_ first results are skipped.
Criteria can either be _title_ or _metaData_.
* __Document::findOneBy(array $criteria, array $orderBy)__
Same as _findBy_ setting _$limit_ = 1 and _$offset_ = null
* __Document::findByTitle($title, array $orderBy, $limit, $offset)__
Same as _findBy_ using title as criteria.
* __Document::findOneByTitle($title, array $orderBy)__
Same as _findOneBy_ using title as criteria.
* __Document::findByMetaData($metaData, array $orderBy, $limit, $offset)__
Same as _findBy_ using metaData as criteria.
* __Document::findOneByMetaData($metaData, array $orderBy)__
Same as _findOneBy_ using metaData as criteria.

Below is a couple of examples:

```php
// Retrieve all documents
$myDocuments = Document:findAll();

// Retrieve a specific document (by id)
$myDocument = Document::find(7382393);

// Retrieve all documents that contains the word "the" in their title and sort descending by creation date
$myDocuments = Document::findByTitle(
	'the',
	array('created' => 'desc')
);

// Retrieve documents from offset 10 until 110 ordered by title in ascending order
$myDocuments = Document::findBy(
	array(),
	array('title' => 'asc'),
	10,
	100
);
```

## Retrieving the signed document
When the signing process is completed (when __getStatus()__ returns "completed"), the signed PDF document can be retrieved by calling the __getPdf()__ method on the document object in question.

## Retrieving linked objects
A signable document contains signature lines. These objects can be retrieved using the following methods:

* __getSignatureLines()__
Returns the signature lines linked to the document as an array of signature line objects.
* __findSignatureLine($id)__
Find and return a specific signature line by _$id_.

## State variables
A series state variables are used to describe the document state over the course of its life time. The methods for retrieving the state variables are described below:

* __getStatus()__
Returns the status of the document as a string. Possible status values are:
 * _new_: The document hasn't been sent out for signing yet
 * _pending_: The document is out for signing
 * _rejected_: One of the signers has rejected to sign
 * _deleted_: The document has been out for signing but have since been deleted
 * _signed_: The document is signed, but the signed document is not generated yet
 * _completed_: The signing process is completed
* __getCreatedAt()__
Returns the date and time when the document was created as a _DateTime_ object.
* __getModifiedAt()__
Returns the date and time when the document was last modified as a _DateTime_ object.
* __getCompletedAt()__
Returns the date and time when the document signing process was finalized as a _DateTime_ object.
* __getDocumentId()__
Returns the unique ID that is stamped on every page in the document for identification purposes.
* __getOptions()__
Returns the option values assigned to the document.
