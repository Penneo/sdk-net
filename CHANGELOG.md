# Change Log
All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).

## [unreleased]
### Added

### Changed

### Fixed

## [5.0.0] - 2022-05-04
### Changed
- Updated RestSharp from `106.15.0` to `108.0.2`
- Updated from .NET Core 3.1 to .NET 6.0

## [4.3.0] - 2022-05-04
### Added
- Added support for getting different types of default email templates, see [Penneo.MessageTemplate.MessageTemplateType](https://github.com/Penneo/sdk-net/blob/master/Src/Penneo/(Model)/MessageTemplate.cs#L11) for more details

## [4.2.0] - 2022-04-06
### Added
- Added support for `Contact` CRUD
- Added support for setting `Signer.StoreAsContact`

## [4.1.4] - 2022-02-01
### Misc
- Updated RestSharp to 106.15.0


## [4.1.3] - 2021-07-23
### Changed
- Doc comments are now available in the Nuget package


## [4.1.2] - 2021-03-25
### Fixed
- Fixed error when trying to unserialize signers with VATIN validation set


## [4.1.1] - 2021-02-04
### Fixed
- Fixed not being able to retrieve SSN numbers for signers


## [4.1.0] - 2021-01-21
### Added
- Added support for setting CaseFile.DisableEmailAttachments


## [4.0.1] - 2020-12-21
### Misc
- Added support for the net46 platform


## [4.0.0] - 2020-12-12
### Changed
- `Document.Type` has been renamed to `Document.SignType`
- Using the 'https://[env].penneo.com/api/v1' and 'https://[env].penneo.com/api/v2' endpoints is no longer supported. Please use 'https://(sandbox|app).penneo.com/api/v3'.


## [3.0.0] - 2020-12-10
### Changed
- The SDK now uses netcoreapp3.1. If you need other target framework support, please contact us at support@penneo.com.


## [2.9.0] - 2020-11-11
### Added
- Added support for specifying which type of SSN signers should validate as.
```csharp
signer.SsnType = "dk:cpr";
```
Not setting the SSN type is now deprecated, and will throw an error in the next major version.
See https://app.penneo.com/api/v1/signers/ssn-types for info on permitted values.


## [2.8.0] - 2020-11-10
### Added
- Added support for setting up webhook subscriptions. See [the docs](https://github.com/Penneo/sdk-net/blob/master/docs/webhooks.md) for more details.


## [2.7.1] - 2020-10-08
### Fixed
- Fixed the debug logger being optimised away


## [2.7.0] - 2020-07-03
### Fixed
- Fixed date time values before year 1 causing exceptions, all such values will now snap to DateTime.MinValue

### Added
- Added a DebugLogger class to aid in debugging problems


## [2.6.0] - 2020-06-30
### Added
- Added missing property `activatedAt` to Signature Line to show the date when the Signature Line was activated.


### [2.5.0] - 2020-06-29
### Fixed
- Fixed invalid property `activeAt` of Case File. Now is called `activated` and shows the date when the Case File was activated.


### [2.4.0] - 2020-03-05
### Added
- Added support for activeAt and expireAt at the signer type map level.


### [2.3.0] - 2020-02-27
### Added
- Added support for activeAt and expireAt at signature line level.


### [2.2.0] - 2020-02-24
### Added
- Added CaseFile.ActivateAt, a field that shows when the case file is going to be activated.


### [2.1.0] - 2019-10-07
### Added
- Added CaseFile.Reference, a handy field to track case files with


## [2.0.2] - 2019-07-05
### Changed
- Updated RestSharp to 106.6.10


## [2.0.1] - 2019-04-01
### Fixed
- [\#107] Meta data was missing from the Penneo .dll file (File version etc.)

## [2.0.0] - 2018-11-28
### Added
- [\#83] Concurrency support

## [1.7.0] - 2018-05-16
### Added
- [\#97] Case file type can be fetched for the case files using `GetCaseFileTemplate` for the case files. Similarly, document type can be fetched for the documents using `GetDocumentType`
- [\#98] Case file has properties `customerId`, `userId`, and `caseFileTemplateId`, now
- [\#98] Document has property `documentTypeId` now

## [1.6.1] - 2018-04-24
### Fixed
- [\#93] Nuget spec file wasn't updated with the correct RestSharp version

## [1.6.0] - 2018-02-28
### Changed
- [\#93] Updated RestSharp version. This means that upgrading to this version might break the project if another version of RestSharp is being used somewhere else. In such case upgrading RestSharp is recommended. 

## [1.5.0] - 2017-12-19
### Added
- [\#90] Disable notifications for the case file owner
- [\#90] Enable sign on meeting for the case file

### Fixed
- [\#90] It is possible to update `sendAt`, `expireAt`, and `sensitiveData` properties for the case file

## [1.4.2] - 2017-12-15
### Fixed
- [\#88] Updating the signing requests with email templates was failing with a bad request `400` error. This was introduced in version `1.4.0`

## [1.4.1] - 2017-11-13
### Fixed
- [\#86] Updated the version from `1.3.0` to `1.4.1`

## [1.4.0] - 2017-05-26
### Added
- [\#57] Make REST calls to the server
- [\#74] Set a parent for folders
- [\#75] Insecure signing (Touch signatures) can be enabled for signing requests
- [\#75] Email templates can be configured for signing requests

## [1.3.0] - 2017-03-08
### Added
- [\#67] Pagination support added
- [\#69] Logging support added. Now it is possible to log http request and response bodies as well.

## [1.2.1] - 2017-01-18
### Fixed
- [\#65] Updated document statuses

## [1.2.0] - 2017-01-13
### Added
- [\#63] Method for getting the validated name of the signer

## [1.1.0]
### Added
- [\#59] Moving validations to folders

[comment]: # (Build Comparison Links)

[unreleased]: https://github.com/Penneo/sdk-net/compare/2.1.0...HEAD
[2.1.0]:      https://github.com/Penneo/sdk-net/compare/2.0.2...2.1.0
[2.0.2]:      https://github.com/Penneo/sdk-net/compare/2.0.1...2.0.2
[2.0.1]:      https://github.com/Penneo/sdk-net/compare/2.0.0...2.0.1
[2.0.0]:      https://github.com/Penneo/sdk-net/compare/1.7.0...2.0.0
[1.7.0]:      https://github.com/Penneo/sdk-net/compare/1.6.1...1.7.0
[1.6.1]:      https://github.com/Penneo/sdk-net/compare/1.6.0...1.6.1
[1.6.0]:      https://github.com/Penneo/sdk-net/compare/1.5.0...1.6.0
[1.5.0]:      https://github.com/Penneo/sdk-net/compare/1.4.2...1.5.0
[1.4.2]:      https://github.com/Penneo/sdk-net/compare/1.4.1...1.4.2
[1.4.1]:      https://github.com/Penneo/sdk-net/compare/1.4.0...1.4.1
[1.4.0]:      https://github.com/Penneo/sdk-net/compare/1.3.0...1.4.0
[1.3.0]:      https://github.com/Penneo/sdk-net/compare/1.2.1...1.3.0
[1.2.1]:      https://github.com/Penneo/sdk-net/compare/1.2.0...1.2.1
[1.2.0]:      https://github.com/Penneo/sdk-net/compare/1.1.0...1.2.0
[1.1.0]:      https://github.com/Penneo/sdk-net/compare/1.0.23...1.1.0

[comment]: # (Issue Links)

[\#107]: https://github.com/Penneo/sdk-net/issues/107
[\#90]: https://github.com/Penneo/sdk-net/issues/90
[\#88]: https://github.com/Penneo/sdk-net/issues/88
[\#86]: https://github.com/Penneo/sdk-net/issues/86
[\#83]: https://github.com/Penneo/sdk-net/issues/83
[\#75]: https://github.com/Penneo/sdk-net/issues/75
[\#74]: https://github.com/Penneo/sdk-net/issues/74
[\#69]: https://github.com/Penneo/sdk-net/issues/69
[\#67]: https://github.com/Penneo/sdk-net/issues/67
[\#63]: https://github.com/Penneo/sdk-net/issues/63
[\#59]: https://github.com/Penneo/sdk-net/issues/59
