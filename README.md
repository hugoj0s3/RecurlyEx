# RecurlyEx
RecurlyEx is a C# library for scheduling recurring events using easy-to-read, natural language–inspired expressions.
It's like cron, but readable.

🔁 Examples of expressions you can write:

- @every day @at 9:00am
- @every week @on [Tuesday, Thursday] @at 18:00
- @every month @on ClosestWeekdayTo 6th
- @every 25 seconds @between 1:20pm and 01:22pm

## ✨ Features

- Human-readable cron-like recurrence
- Powerful `@between`, `@upto`, `@from` filtering
- Closest weekday, first/last day handling
- Time zone support with IANA TZ names
- Friendly aliases (`@daily`, `@hourly`, etc.)


## Installation
```bash
dotnet add package RecurlyEx
```

## Usage
```csharp
using RecurlyEx;

// Define a recurrence rule using natural language
var recurlyEx = RecurlyEx.Parse("@every 25 min @on friday @between 1:00pm and 03:00pm");

// Get the next 11 occurrences starting from Jan 1, 2020
var nextOccurrences = recurlyEx.TryGetNextOccurrencesInUtc(
    DateTime.Parse("2020-01-01 00:00:00"), 11
);

foreach (var nextOccurrence in nextOccurrences)
{
    Console.WriteLine(nextOccurrence.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
}

/*
Output:
Friday, 03 January 2020 13:00:00
Friday, 03 January 2020 13:25:00
Friday, 03 January 2020 13:50:00
Friday, 03 January 2020 14:15:00
Friday, 03 January 2020 14:40:00
Friday, 10 January 2020 13:00:00
Friday, 10 January 2020 13:25:00
Friday, 10 January 2020 13:50:00
Friday, 10 January 2020 14:15:00
Friday, 10 January 2020 14:40:00
Friday, 17 January 2020 13:00:00
*/
```

## Supported Recurrence Expression Syntax

### `@every` — Main Recurrence Interval

Defines how often the recurrence happens.  
The interval must be a positive integer, and the time unit must be one of the supported units.

**Syntax:**
- `@every <number> <time-unit>`

**Examples:**
- `@every day` — every day
- `@every 2 weeks` — every 2 weeks
- `@every 6 months` — every 6 months
- `@every year` — every year

**Anchoring Recurrences:**

The `AnchoredOn` keyword lets you specify the exact starting point or anchor for recurrences that repeat in multi-month intervals. By default, such recurrences would count intervals from the base date, but with anchoring, you can ensure the recurrence always aligns with specific months, weeks, or ordinal days—regardless of the start date.

This is especially useful for business, fiscal, or seasonal schedules that must always occur in certain months or on specific days, no matter when the schedule was created.

**Syntax:**
- `@every <number> <time-unit> AnchoredOn <month|number|week|ordinal day>`

> **Note:** The anchor must be of the same time unit as the recurrence interval (e.g., for `@every 2 months`, only months are valid for `AnchoredOn`).

**Examples:**
- `@every 2 months AnchoredOn Nov @on 1st`  
  Occurrences will always be on the 1st of November, January, March, May, July, and September (i.e., every two months, starting from November, regardless of the base date).
- `@every 6 months AnchoredOn Feb @on 1st`  
  Occurrences will always be in Feb and August of each year.

**Notes:**
- Only one `@every` rule is allowed per expression.
- The interval must be greater than zero (e.g., `@every 0 days` is invalid).

### Shortcut Interval Keywords

In addition to the `@every` rule, you can use shortcut keywords for common intervals:

- `@secondly`
- `@minutely`
- `@hourly`
- `@daily`
- `@weekly`
- `@monthly`
- `@yearly`

These are equivalent to `@every 1 <time-unit>` and can be used anywhere you would use `@every`.  
For example:
- `@monthly @on 1st` — the first day of every month
- `@hourly @between 09:00 and 17:00` — every hour between 9am and 5pm

You can use these shortcuts for more concise and readable recurrence expressions.

### `@on`, `@in`, `@at` — Specific Occurrences

Defines when the recurrence happens.  
The interval must be a positive integer, and the time unit must be one of the supported units.

**Syntax:**
- `@on <time-unit> <number>`
- `@in <time-unit> <number>`
- `@at <time-unit> <number>`

For multiple uses bracket the time units and numbers:
- `@on [<time-unit> <number>, ...]`
- `@in [<time-unit> <number>, ...]`
- `@at [<time-unit> <number>, ...]`
**Examples:**
- `@on day 10` — on the 10th day of the month or just `@on 10th`
- `@in month 10` — in december or use `@in december` or `@in dec`
- `@at hour 10 @at minute 30` — at the 10th day of the month or use `@at 10:30am`

**Examples:**
- `@on [day 1, day 20, day 30]` or `@on [1st, 20th, 30th]` — on the 1st, 20th, and 30th days of the month
- `@at [10:00pm, 12:00am]` or `@at [22:00, 00:00]` — - `@at [10:00pm, 12:00am]` or `@at [22:00, 00:00]` — If neither `am` nor `pm` is specified, the time will be interpreted using 24-hour notation by default.
- `@on [friday, saturday]` or `@on [fri, sat]` — on Friday or Saturday

**Special Days**

- **Closest weekday to a specific day:**  
  `@on ClosestWeekdayTo <number>` — Matches the weekday (Monday–Friday) that is closest to the given day of the month.  
  _Example:_ `@on ClosestWeekdayTo 15th` — the closest weekday to the 15th of the month.

  **Edge Cases:**
    - If the given day falls on a weekday (Monday–Friday), it is selected directly.
    - If it falls on a Saturday, the engine will select the preceding Friday.
    - If it falls on a Sunday, the engine will select the following Monday.
    - **Special case:** If the first day of the month is a Saturday (`@on ClosestWeekdayTo 1st`), the closest weekday is the following Monday (the 3rd of the month).
    - Similarly, if the last day of the month is a Sunday, the closest weekday is the preceding Friday.

  _Examples:_
    - If the 1st of the month is a Saturday, `@on ClosestWeekdayTo 1st` matches the 3rd (Monday).
    - If the 1st is a Sunday, it matches the 2nd (Monday).
    - If the 15th is a Saturday, it matches the 14th (Friday).
    - If the 15th is a Sunday, it matches the 16th (Monday).
  
- **First weekday of the month:**  
  `@on FirstWeekday` — Matches the first weekday (Monday–Friday) of the month.  
  _Example:_ `@on FirstWeekday`

- **Last weekday of the month:**  
  `@on LastWeekday` — Matches the last weekday (Monday–Friday) of the month.  
  _Example:_ `@on LastWeekday`

- **First or last day of the month:**  
  You can use any of the following interchangeable keywords to match the first or last day of the month:

    - **Last day of the month:**
        - `@on LastDayOfTheMonth`
        - `@on LastDayOfMonth`
        - `@on LastDay`

    - **First day of the month:**
        - `@on FirstOfTheMonth`
        - `@on FirstDayOfMonth`
        - `@on FirstDay`

  _Examples:_
    - `@every month @on LastDay` — triggers on the last day of every month.
    - `@every month @on FirstDayOfMonth` — triggers on the first day of every month.

- **Specific weekday occurrence in the month:**  
  Use numeric or word-based ordinals, and full or abbreviated weekday names. Ordinals from `1st` to `5th` are supported (note: not all months have a 5th occurrence).
    - _Examples:_
        - `@on 1stMon`, `@on 1stMonday`, `@on FirstMon`, `@on FirstMonday` — all specify the first Monday of the month.
        - `@on 2ndTuesday`, `@on ThirdFri`, `@on 5thMonday` — specify the second Tuesday, third Friday, or fifth Monday of the month, respectively.

- **First or last day of the month:**  
  You can use any of the following interchangeable keywords to match the first or last day of the month:

    - **Last day of the month:**
        - `@on LastDayOfTheMonth`
        - `@on LastDayOfMonth`
        - `@on LastDay`

    - **First day of the month:**
        - `@on FirstOfTheMonth`
        - `@on FirstDayOfMonth`
        - `@on FirstDay`

  _Examples:_
    - `@every month @on LastDay` — triggers on the last day of every month.
    - `@every month @on FirstDayOfMonth` — triggers on the first day of every month.

- **Basic arithmetic with day keywords:**  
  The recurrence engine supports simple arithmetic operations (`+` and `-`) with day-based keywords.  
  For example:
    - `@on LastDay - 1` — matches the day before the last day of the month (i.e., the second-to-last day).
    - `@on FirstDay + 2` — matches the third day of the month.
It never crosses month boundaries, e.g `@on FirstDay + 31` it will match the last day of the month. or `@on LastDay - 31` it will match the first day of the month.

**Note:**  
The recurrence engine does not make a strict distinction between `@on`, `@at`, and `@in` when used in expressions. 
All three keywords are treated equivalently for specifying time units, even if this sometimes results in expressions that sound like broken English. 
This design choice is for flexibility and ease of parsing, so you can use whichever keyword feels most natural in your context.

### `@between` Between Rules

The `@between` rule allows you to restrict occurrences to a specific range—such as a range of days in the month, times within a day, or other supported units.

**Syntax:**
- `<start>` and `<end>` can be days, times, or other supported units/aliases.

**Examples:**
- `@every hour @between 10th and 15th`  
  Matches only the 10th through 15th day of each hour.
- `@every hour @between 09:00 and 17:00`  
  Matches only occurrences between 9:00am and 5:00pm each hour.
- `@every week @between Jun and Dec`  
  Matches weekly occurrences between June and December.
- `@every month @between 2nd and LastDay`
  Matches only the first through last day of each month.
- `@every 25 secs @between [1:00pm and 03:00pm, 6:00pm and 8:00pm]`
  Matches only occurrences between 1:00pm and 3:00pm, and 6:00pm and 8:00pm, every 25 seconds.

**Notes:**
- The boundaries are inclusive: both the start and end values are included in the range.
- You can use any supported aliases for days, times, or months in the `@between` rule.
- `@between` can be combined with other rules (such as `@on`, `@at`, etc.) for more precise filtering.


### Upto and From Rules

The `@upto` and `@from` rules allow you to restrict occurrences to only those before or after a specific point (such as a time, day, or other supported unit), relative to the recurrence interval.

**Syntax:**
- `@upto <value>`
- `@from <value>`

`<value>` can be a time, day, or any supported alias (e.g., `@upto 13:00`, `@after 09:00`, `@upto 15th`).

**Examples:**
- `@every 10 secs @upto 1:00pm`  
  Matches every 10 seconds, including 1:00pm, but only up to and including 1:00pm each day.
- `@every minute @from 18:00`  
  Matches every minute, including 6:00pm, and all times after on each day.
- `@every day @upto 15th`  
  Matches all days up to and including the 15th of each month.
- `@every hour @from 09:00`  
  Matches every hour, starting from and including 9:00am each day.

**Notes:**
- The boundary value itself is included; for example, `@upto 1:00pm` matches up to and including 1:00pm.
- You can use any supported aliases for times, days, or months.
- `@upto` and `@from` can be combined with other rules (such as `@on`, `@at`, etc.) for more precise filtering.

### Time Zone Support

You can specify the time zone for your recurrence expressions using either `@timezone` or the shorthand `@tz`.  
This ensures that all date and time calculations are made in the specified IANA time zone, rather than the default (usually UTC).

**Syntax:**
- `@timezone <IANA-timezone>`
- `@tz <IANA-timezone>`

**Examples:**
- `@every day @at 09:00 @timezone America/New_York`
- `@monthly @on 1st @tz Asia/Tokyo`

**Notes:**
- The engine uses IANA time zone names (e.g., `America/Los_Angeles`, `Europe/London`).
- If no time zone is specified, UTC is used by default.
- Time zone rules affect all time-based calculations, including DST transitions.

### Supported time units and aliases

The following units and their aliases can be used with all relevant rules, such as `@every`, `@on`, and `@between`, as well as `@upto` and `@from`:

| Unit    | Aliases                            |
|---------|------------------------------------|
| second  | `second`, `sec`, `secs`, `seconds` |
| minute  | `minute`, `min`, `mins`, `minutes` |
| hour    | `hour`, `hr`, `hrs`, `hours`       |
| day     | `day`, `dy`, `days`, `dys`         |
| week    | `week`, `wk`, `weeks`, `wks`       |
| month   | `month`, `mth`, `months`, `mths`   |
| year    | `year`,  `yr`, `years`, `yrs`      |

**Examples:**
- `@every 2 weeks`
- `@on day 10`
- `@between hour 9 and hour 17`
- `@on month 12`

You can use any alias interchangeably in your recurrence expressions.

### Specifying Time in Recurrence Expressions

You can specify times in your recurrence expressions using several supported formats. The engine accepts both 12-hour and 24-hour notation, and you can include seconds if needed.

**Supported Time Formats:**
- `2:00pm` — 12-hour clock with `am`/`pm`
- `14:00` — 24-hour clock (hours and minutes)
- `14:00:10` — 24-hour clock with seconds
- `2:00:10pm` — 12-hour clock with seconds and `am`/`pm`

**Examples:**
- `@every day @at 2:00pm` — triggers at 2:00pm each day
- `@every day @at 14:00` — triggers at 14:00 (2:00pm) each day
- `@every day @at 14:00:10` — triggers at 2:00:10pm each day
- `@every day @at 2:00:10pm` — triggers at 2:00:10pm each day

**Notes:**
- If neither `am` nor `pm` is specified, the time is interpreted as 24-hour format by default.
- Seconds are optional; if omitted, the time matches at the start of the specified minute.
- You can use these formats in any rule that accepts a time value, such as `@at`, `@on`, `@between`, `@from`, and `@upto`.

### Specifying Dates in Recurrence Expressions

You can specify dates in your recurrence expressions using the following formats:

**Supported Date Formats:**
Ordinal Date (e.g., 1st, 2nd, 3rd, 4th, 5th, etc.):
- `1st` — the first day of the month
- `2nd` — the second day of the month
- `3rd` — the third day of the month
- `4th` — the fourth day of the month
etc.

Month Name (e.g., January, February, etc.):
- `January` or `Jan`
- `February` or `Feb`
etc.

Weekday Name (e.g., Monday, Tuesday, etc.):
- `Monday` or `Mon`
- `Tuesday` or `Tue`
etc.

Dashed Separated Date (e.g., 2023-01-23):
- `Jan-23` — January 23rd
- `12-31` — December 31st
- `2023-01-23` — January 23rd, 2023
- `2023-12-31` — December 31st, 2023
- `2023-JAN` — January 2023

Ordinal Day + Month Name (e.g., 1st January, 2nd February, etc.):
- `1st January` or `1st Jan` or `January 1st` or `Jan 1st`
- `2nd February` or `2nd Feb` or `February 2nd` or `Feb 2nd`

Ordinal Day + Month Name + Year (e.g., 1st January 2023, 2nd February 2023, etc.):
- `1st January 2023` or `1st Jan 2023` or `January 1st 2023` or `Jan 1st 2023`
- `2nd February 2023` or `2nd Feb 2023` or `February 2nd 2023` or `Feb 2nd 2023`

Colon + Dashed Separated Date (e.g., 2023-01-23):
it allows the combination of date and time
- `2023-01-23 14:00` — January 23rd, 2023 at 14:00
- `JAN-23 14:00:10` — January 23rd, 2023 at 14:00:10

Colon + ordinal day (e.g. 1st JAN 14:00)
- `1st JAN 14:00` or `1st JAN 14:00:10` or `1st JAN 2:00pm` or `1st JAN 2:00:10pm`

PS: The comma is ignored if not in brackets for multiple dates
- `@on 1st, January` works, but not @on [1st, January]

## Changelog
See [CHANGELOG.md](./CHANGELOG.md) for release history and details.
