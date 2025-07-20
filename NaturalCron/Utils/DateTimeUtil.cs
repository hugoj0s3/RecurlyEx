namespace NaturalCron.Utils;

internal static class DateTimeUtil
{
    internal static bool IsWeekDay(DateTime dateTime) => dateTime.DayOfWeek != DayOfWeek.Saturday &&
                                                         dateTime.DayOfWeek != DayOfWeek.Sunday;

    internal static DateTime Add(NaturalCronTimeUnit timeUnit, DateTime dateTime, int value)
    {
        return timeUnit switch
        {
            NaturalCronTimeUnit.Year => dateTime.AddYears(value),
            NaturalCronTimeUnit.Month => dateTime.AddMonths(value),
            NaturalCronTimeUnit.Week => dateTime.AddDays(value * 7),
            NaturalCronTimeUnit.Day => dateTime.AddDays(value),
            NaturalCronTimeUnit.Hour => dateTime.AddHours(value),
            NaturalCronTimeUnit.Minute => dateTime.AddMinutes(value),
            NaturalCronTimeUnit.Second => dateTime.AddSeconds(value),
            _ => dateTime
        };
    }
    
    internal static DateTime Substract(NaturalCronTimeUnit timeUnit, DateTime dateTime, int value)
    {
        return timeUnit switch
        {
            NaturalCronTimeUnit.Year => dateTime.AddYears(value *-1),
            NaturalCronTimeUnit.Month => dateTime.AddMonths(value *-1),
            NaturalCronTimeUnit.Week => dateTime.AddDays((value * 7) *-1),
            NaturalCronTimeUnit.Day => dateTime.AddDays(value *-1),
            NaturalCronTimeUnit.Hour => dateTime.AddHours(value *-1),
            NaturalCronTimeUnit.Minute => dateTime.AddMinutes(value *-1),
            NaturalCronTimeUnit.Second => dateTime.AddSeconds(value *-1),
            _ => dateTime
        };
    }
    
    internal static int GetPartValue(NaturalCronTimeUnit timeUnit, DateTime dateTime)
    {
        var value = timeUnit switch
        {
            NaturalCronTimeUnit.Second => dateTime.Second,
            NaturalCronTimeUnit.Minute => dateTime.Minute,
            NaturalCronTimeUnit.Hour => dateTime.Hour,
            NaturalCronTimeUnit.Day => dateTime.Day,
            NaturalCronTimeUnit.Week => (int)dateTime.DayOfWeek + 1,
            NaturalCronTimeUnit.Month => dateTime.Month,
            NaturalCronTimeUnit.Year => dateTime.Year,
            _ => throw new ArgumentOutOfRangeException(nameof(timeUnit), timeUnit, null)
        };

        return value;
    }
    
    internal static bool IsGreaterOrEqual(NaturalCronTimeUnit timeUnit, DateTime dateTime, int value)
    {
        return GetPartValue(timeUnit, dateTime) >= value;
    }

    internal static bool IsLessOrEqual(NaturalCronTimeUnit timeUnit, DateTime dateTime, int value)
    {
        return GetPartValue(timeUnit, dateTime) <= value;
    }
    
    internal static bool IsGreater(NaturalCronTimeUnit timeUnit, DateTime dateTime, int value)
    {
        return GetPartValue(timeUnit, dateTime) > value;
    }

    internal static bool IsLess(NaturalCronTimeUnit timeUnit, DateTime dateTime, int value)
    {
        return GetPartValue(timeUnit, dateTime) < value;
    }

    internal static bool IsEqual(NaturalCronTimeUnit timeUnit, DateTime dateTime, int value)
    {
        return GetPartValue(timeUnit, dateTime) == value;
    }
    
    internal static DateTime SetPartValue(NaturalCronTimeUnit timeUnit, DateTime dateTime, int value)
    {
        switch (timeUnit)
        {
            case NaturalCronTimeUnit.Second:
                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, value);
            case NaturalCronTimeUnit.Minute:
                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, value, dateTime.Second);
            case NaturalCronTimeUnit.Hour:
                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, value, dateTime.Minute, dateTime.Second);
            case NaturalCronTimeUnit.Day:
                return new DateTime(dateTime.Year, dateTime.Month, value, dateTime.Hour, dateTime.Minute, dateTime.Second);
            case NaturalCronTimeUnit.Month:
                return new DateTime(dateTime.Year, value, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            case NaturalCronTimeUnit.Year:
                return new DateTime(value, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            default:
                throw new ArgumentOutOfRangeException(nameof(timeUnit), timeUnit, null);
        }
    }
    
    public static double GetDurationInSeconds(NaturalCronTimeUnit unit, int amount, int year, int month)
    {
        switch (unit)
        {
            case NaturalCronTimeUnit.Second:
                return amount;
            case NaturalCronTimeUnit.Minute:
                return amount * 60;
            case NaturalCronTimeUnit.Hour:
                return amount * 3600;
            case NaturalCronTimeUnit.Day:
                return amount * 86400;
            case NaturalCronTimeUnit.Week:
                return amount * 7 * 86400;
            case NaturalCronTimeUnit.Month:
                int daysInMonth = DateTime.DaysInMonth(year, month);
                return amount * daysInMonth * 86400;
            case NaturalCronTimeUnit.Year:
                int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
                return amount * daysInYear * 86400;
            default:
                throw new NotSupportedException($"Unit {unit} not supported");
        }
    }

    internal static DateTime AdvanceTimeSafety(NaturalCronTimeUnit timeUnit, DateTime dateTime, int amount)
    {
        if (timeUnit == NaturalCronTimeUnit.Week)
        {
            timeUnit = NaturalCronTimeUnit.Day;
            amount = 1;
        }
        
        if (dateTime <= DateTime.MaxValue.AddSeconds(-1))
        {
            DateTime advanced;
            try
            {
                advanced = DateTimeUtil.Add(timeUnit, dateTime, amount);
                if (advanced > DateTime.MaxValue)
                    advanced = DateTime.MaxValue;
            }
            catch
            {
                advanced = DateTime.MaxValue;
            }
            
            dateTime = advanced;
        }
        else
        {
            dateTime = DateTime.MaxValue;
        }
        return dateTime;
    }
}