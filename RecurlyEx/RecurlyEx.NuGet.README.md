# RecurlyEx

> **⚠️ IMPORTANT: This package has been renamed to NaturalCron**
> 
> RecurlyEx is now **NaturalCron**! Please migrate to the new package for future updates and support.
> 
> **New Package:** [NaturalCron](https://www.nuget.org/packages/NaturalCron)
> 
> This RecurlyEx package will continue to work but will no longer receive updates. All new features and bug fixes will be available in NaturalCron.

RecurlyEx is a C# library for scheduling recurring events using easy-to-read, natural language–inspired expressions.  
It's like cron, but readable.

🔁 Examples of expressions you can write:

- @every day @at 9:00am or every day at 9:00am
- @every week @on [Tuesday, Thursday] @at 18:00 or every week on [Tuesday, Thursday] at 18:00
- @every month @on ClosestWeekdayTo 6th or every month on ClosestWeekdayTo 6th
- @every 25 seconds @between 1:20pm and 01:22pm or every 25 seconds between 1:20pm and 01:22pm


## Features
- Human-readable cron-like recurrence
- Filtering by time, day, and more
- Time zone support
