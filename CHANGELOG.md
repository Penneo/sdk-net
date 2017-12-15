# Change Log
All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).

## [unreleased]
### Added

### Changed

### Fixed

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

[unreleased]: https://github.com/Penneo/sdk-net/compare/1.4.2...HEAD
[1.4.2]:     https://github.com/Penneo/sdk-net/compare/1.4.1...1.4.2
[1.4.1]:     https://github.com/Penneo/sdk-net/compare/1.4.0...1.4.1
[1.4.0]:     https://github.com/Penneo/sdk-net/compare/1.3.0...1.4.0
[1.3.0]:     https://github.com/Penneo/sdk-net/compare/1.2.1...1.3.0
[1.2.1]:     https://github.com/Penneo/sdk-net/compare/1.2.0...1.2.1
[1.2.0]:     https://github.com/Penneo/sdk-net/compare/1.1.0...1.2.0
[1.1.0]:     https://github.com/Penneo/sdk-net/compare/1.0.23...1.1.0

[comment]: # (Issue Links)

[\#88]: https://github.com/Penneo/sdk-net/issues/88
[\#86]: https://github.com/Penneo/sdk-net/issues/86
[\#75]: https://github.com/Penneo/sdk-net/issues/75
[\#74]: https://github.com/Penneo/sdk-net/issues/74
[\#69]: https://github.com/Penneo/sdk-net/issues/69
[\#67]: https://github.com/Penneo/sdk-net/issues/67
[\#63]: https://github.com/Penneo/sdk-net/issues/63
[\#59]: https://github.com/Penneo/sdk-net/issues/59
