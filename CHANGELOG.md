# Changelog

## [0.1.0]
Rename project from RecurlyEx to NaturalCron

## [0.0.3] - 2025-07-15
### Added
- Make @ optional we support both "@every day @at 9:00am" and "every day at 9:00am"
- GetOccurrences in the current thread's timezone

## [0.0.2] - 2025-07-14
### Fixed
- Fixed bug where using @between with a smaller time unit than @every (e.g., @every week @on [friday, tuesday] @between 1:00pm and 3:00pm) would incorrectly match the base time when it already falls within the @between range. (See test case 33.81)
- Allowed combining @every day with @on week rules (e.g., @every day @on [monday, wednesday]).

## [0.0.1] - 2025-07-13
### Added
- Custom cache implementation (`RecurlyExCache`)
- Automatic cache cleanup
- Removed dependency on Microsoft.Extensions.Caching.Memory

### Fixed
- NuGet README and metadata issues

## [0.0.0] - 2025-07-01
- Initial release
