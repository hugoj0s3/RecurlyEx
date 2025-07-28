# NaturalCron
**NaturalCron** is a lightweight .NET library for writing **human-readable recurrence rules** — an easier alternative to cryptic CRON syntax.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
![.NET](https://github.com/hugoj0s3/NaturalCron/actions/workflows/dotnet.yml/badge.svg)
[![Awesome .NET](https://awesome.re/badge.svg)](https://github.com/quozd/awesome-dotnet)


## 🔁 Examples of expressions you can write:
- `@every day @at 9:00am` or `every day at 9:00am`
- `@every week @on [Tuesday, Thursday] @at 18:00` or `every week on [Tuesday, Thursday] at 18:00`
- `@every month @on ClosestWeekdayTo 6th` or `every month on ClosestWeekdayTo 6th`
- `@every 25 seconds @between 1:20pm and 01:22pm` or `every 25 seconds between 1:20pm and 01:22pm`


## ✨ Features
- Readable, expressive and easy to memorize — e.g use `every 5 minutes on friday` instead of `*/5 * * * 5`
- Powerful `between`, `upto`, `from`, and more filtering
- Closest weekday, first/last day handling
- Time zone support with IANA TZ names
- Friendly aliases (`daily`, `hourly`, etc.)


## Why NaturalCron?
Cron is powerful but hard to read. NaturalCron is designed to:
- Be human-readable
- Stay close to natural language
- Still flexible enough for advanced use cases


## 📦 Installation
```bash
dotnet add package NaturalCron
```


## 🚀 Usage
```csharp
using System;
using NaturalCron;

// Define a recurrence rule using natural language
var recur = NaturalCronExpr.Parse("every 25 min on friday between 1:00pm and 03:00pm");

// Get the next 11 occurrences starting from Jan 1, 2020
var nextOccurrences = recur.TryGetNextOccurrences(
    DateTime.Parse("2020-01-01 00:00:00"), 6
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
....
*/
```


## 📚 Other Examples
```csharp
// Every 5 minutes on Fridays
var recur1 = NaturalCron.Parse("every 5 min on friday");

// Every 25 seconds between 1:20pm and 1:22pm
var recur2 = NaturalCron.Parse("every 25 seconds between 1:20pm and 1:22pm");

// Every month on the closest weekday to the 6th
var recur3 = NaturalCron.Parse("every month on ClosestWeekdayTo 6th");

// Every day at 9:00 AM
var recur4 = NaturalCron.Parse("every day at 9:00am");

// Every week on Tuesday and Thursday at 18:00
var recur5 = NaturalCron.Parse("every week on [Tuesday, Thursday] at 18:00");
```


▶ Try it online: [Run on .NET Fiddle](https://dotnetfiddle.net/GLph9G)


## 📖 Documentation
- [Expression Syntax](docs/expression-syntax.md) — Learn how to write human-readable recurrence rules.
- [Fluent Builder Guide](docs/builder.md) — Build expressions easily with type safety and IntelliSense.
- [API Reference](docs/api-reference.md) — Full API details and usage examples.


## 🤝 Contributing
Contributions, bug reports, and feature requests are welcome!

Please open an [issue](../../issues) or submit a [pull request](../../pulls).

## Changelog
See [CHANGELOG.md](./CHANGELOG.md) for release history and details.
