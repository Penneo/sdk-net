# Change Log
All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).

## [unreleased]
### Added

### Changed

### Fixed

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

[unreleased]: https://github.com/Penneo/sdk-net/compare/1.7.0...HEAD
[1.7.0]:     https://github.com/Penneo/sdk-net/compare/1.6.1...1.7.0
[1.6.1]:     https://github.com/Penneo/sdk-net/compare/1.6.0...1.6.1
[1.6.0]:     https://github.com/Penneo/sdk-net/compare/1.5.0...1.6.0
[1.5.0]:     https://github.com/Penneo/sdk-net/compare/1.4.2...1.5.0
[1.4.2]:     https://github.com/Penneo/sdk-net/compare/1.4.1...1.4.2
[1.4.1]:     https://github.com/Penneo/sdk-net/compare/1.4.0...1.4.1
[1.4.0]:     https://github.com/Penneo/sdk-net/compare/1.3.0...1.4.0
[1.3.0]:     https://github.com/Penneo/sdk-net/compare/1.2.1...1.3.0
[1.2.1]:     https://github.com/Penneo/sdk-net/compare/1.2.0...1.2.1
[1.2.0]:     https://github.com/Penneo/sdk-net/compare/1.1.0...1.2.0
[1.1.0]:     https://github.com/Penneo/sdk-net/compare/1.0.23...1.1.0

[comment]: # (Issue Links)

[\#90]: https://github.com/Penneo/sdk-net/issues/90
[\#88]: https://github.com/Penneo/sdk-net/issues/88
[\#86]: https://github.com/Penneo/sdk-net/issues/86
[\#75]: https://github.com/Penneo/sdk-net/issues/75
[\#74]: https://github.com/Penneo/sdk-net/issues/74
[\#69]: https://github.com/Penneo/sdk-net/issues/69
[\#67]: https://github.com/Penneo/sdk-net/issues/67
[\#63]: https://github.com/Penneo/sdk-net/issues/63
[\#59]: https://github.com/Penneo/sdk-net/issues/59
