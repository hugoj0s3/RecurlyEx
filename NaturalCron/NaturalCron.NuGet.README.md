# NaturalCron

NaturalCron is a C# library for scheduling recurring events using easy-to-read, natural language–inspired expressions.  
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
- Fluent builder for easy expression construction.
