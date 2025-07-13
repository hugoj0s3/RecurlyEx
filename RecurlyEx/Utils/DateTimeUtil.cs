namespace RecurlyEx.Utils;

internal static class DateTimeUtil
{
    internal static bool IsWeekDay(DateTime dateTime) => dateTime.DayOfWeek != DayOfWeek.Saturday &&
                                                         dateTime.DayOfWeek != DayOfWeek.Sunday;

    internal static DateTime Add(RecurlyExTimeUnit timeUnit, DateTime dateTime, int value)
    {
        return timeUnit switch
        {
            RecurlyExTimeUnit.Year => dateTime.AddYears(value),
            RecurlyExTimeUnit.Month => dateTime.AddMonths(value),
            RecurlyExTimeUnit.Week => dateTime.AddDays(value * 7),
            RecurlyExTimeUnit.Day => dateTime.AddDays(value),
            RecurlyExTimeUnit.Hour => dateTime.AddHours(value),
            RecurlyExTimeUnit.Minute => dateTime.AddMinutes(value),
            RecurlyExTimeUnit.Second => dateTime.AddSeconds(value),
            _ => dateTime
        };
    }
    
    internal static DateTime Substract(RecurlyExTimeUnit timeUnit, DateTime dateTime, int value)
    {
        return timeUnit switch
        {
            RecurlyExTimeUnit.Year => dateTime.AddYears(value *-1),
            RecurlyExTimeUnit.Month => dateTime.AddMonths(value *-1),
            RecurlyExTimeUnit.Week => dateTime.AddDays((value * 7) *-1),
            RecurlyExTimeUnit.Day => dateTime.AddDays(value *-1),
            RecurlyExTimeUnit.Hour => dateTime.AddHours(value *-1),
            RecurlyExTimeUnit.Minute => dateTime.AddMinutes(value *-1),
            RecurlyExTimeUnit.Second => dateTime.AddSeconds(value *-1),
            _ => dateTime
        };
    }
    
    internal static int GetPartValue(RecurlyExTimeUnit timeUnit, DateTime dateTime)
    {
        var value = timeUnit switch
        {
            RecurlyExTimeUnit.Second => dateTime.Second,
            RecurlyExTimeUnit.Minute => dateTime.Minute,
            RecurlyExTimeUnit.Hour => dateTime.Hour,
            RecurlyExTimeUnit.Day => dateTime.Day,
            RecurlyExTimeUnit.Week => (int)dateTime.DayOfWeek + 1,
            RecurlyExTimeUnit.Month => dateTime.Month,
            RecurlyExTimeUnit.Year => dateTime.Year,
            _ => throw new ArgumentOutOfRangeException(nameof(timeUnit), timeUnit, null)
        };

        return value;
    }
    
    internal static bool IsGreaterOrEqual(RecurlyExTimeUnit timeUnit, DateTime dateTime, int value)
    {
        return GetPartValue(timeUnit, dateTime) >= value;
    }

    internal static bool IsLessOrEqual(RecurlyExTimeUnit timeUnit, DateTime dateTime, int value)
    {
        return GetPartValue(timeUnit, dateTime) <= value;
    }
    
    internal static bool IsGreater(RecurlyExTimeUnit timeUnit, DateTime dateTime, int value)
    {
        return GetPartValue(timeUnit, dateTime) > value;
    }

    internal static bool IsLess(RecurlyExTimeUnit timeUnit, DateTime dateTime, int value)
    {
        return GetPartValue(timeUnit, dateTime) < value;
    }

    internal static bool IsEqual(RecurlyExTimeUnit timeUnit, DateTime dateTime, int value)
    {
        return GetPartValue(timeUnit, dateTime) == value;
    }
    
    internal static DateTime SetPartValue(RecurlyExTimeUnit timeUnit, DateTime dateTime, int value)
    {
        switch (timeUnit)
        {
            case RecurlyExTimeUnit.Second:
                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, value);
            case RecurlyExTimeUnit.Minute:
                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, value, dateTime.Second);
            case RecurlyExTimeUnit.Hour:
                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, value, dateTime.Minute, dateTime.Second);
            case RecurlyExTimeUnit.Day:
                return new DateTime(dateTime.Year, dateTime.Month, value, dateTime.Hour, dateTime.Minute, dateTime.Second);
            case RecurlyExTimeUnit.Month:
                return new DateTime(dateTime.Year, value, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            case RecurlyExTimeUnit.Year:
                return new DateTime(value, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            default:
                throw new ArgumentOutOfRangeException(nameof(timeUnit), timeUnit, null);
        }
    }
    
    public static double GetDurationInSeconds(RecurlyExTimeUnit unit, int amount, int year, int month)
    {
        switch (unit)
        {
            case RecurlyExTimeUnit.Second:
                return amount;
            case RecurlyExTimeUnit.Minute:
                return amount * 60;
            case RecurlyExTimeUnit.Hour:
                return amount * 3600;
            case RecurlyExTimeUnit.Day:
                return amount * 86400;
            case RecurlyExTimeUnit.Week:
                return amount * 7 * 86400;
            case RecurlyExTimeUnit.Month:
                int daysInMonth = DateTime.DaysInMonth(year, month);
                return amount * daysInMonth * 86400;
            case RecurlyExTimeUnit.Year:
                int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
                return amount * daysInYear * 86400;
            default:
                throw new NotSupportedException($"Unit {unit} not supported");
        }
    }

    internal static DateTime AdvanceTimeSafety(RecurlyExTimeUnit timeUnit, DateTime dateTime, int amount)
    {
        if (timeUnit == RecurlyExTimeUnit.Week)
        {
            timeUnit = RecurlyExTimeUnit.Day;
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