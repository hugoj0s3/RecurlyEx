using System.Text;
using NaturalCron.Builder.Selectors;

namespace NaturalCron.Builder;

internal class NaturalCronBuilderImpl :
    INaturalCronStarterSelector,
    INaturalEveryTimeUnitSelector,
    INaturalEveryTimeUnitWithValueSelector,
    INaturalCronTimeSpecificationSelector,
    INaturalCronTimeSpecificationWithAnchoredSelector
{
    private bool useAmpersat;
    private bool useWeekFullName;
    private bool useMonthFullName;
    
    private string? ianaTimeZoneId;
    
    private NaturalCronTimeUnit? everyTimeUnit;
    private int? everyValue;
    private string? everyRawExpr;
    private string? anchoredRawExpr;
    
    
    private readonly StringBuilder exprBuilder = new StringBuilder();
    private string prefix;

   private static readonly Dictionary<NaturalCronMonth, string> MonthNameAbbreviations =
    new Dictionary<NaturalCronMonth, string>
    {
        { NaturalCronMonth.Jan, "Jan" },
        { NaturalCronMonth.Feb, "Feb" },
        { NaturalCronMonth.Mar, "Mar" },
        { NaturalCronMonth.Apr, "Apr" },
        { NaturalCronMonth.May, "May" },
        { NaturalCronMonth.Jun, "Jun" },
        { NaturalCronMonth.Jul, "Jul" },
        { NaturalCronMonth.Aug, "Aug" },
        { NaturalCronMonth.Sep, "Sep" },
        { NaturalCronMonth.Oct, "Oct" },
        { NaturalCronMonth.Nov, "Nov" },
        { NaturalCronMonth.Dec, "Dec" },
    };

private static readonly Dictionary<NaturalCronMonth, string> MonthFullNames =
    new Dictionary<NaturalCronMonth, string>
    {
        { NaturalCronMonth.Jan, "January" },
        { NaturalCronMonth.Feb, "February" },
        { NaturalCronMonth.Mar, "March" },
        { NaturalCronMonth.Apr, "April" },
        { NaturalCronMonth.May, "may" },
        { NaturalCronMonth.Jun, "June" },
        { NaturalCronMonth.Jul, "July" },
        { NaturalCronMonth.Aug, "August" },
        { NaturalCronMonth.Sep, "September" },
        { NaturalCronMonth.Oct, "October" },
        { NaturalCronMonth.Nov, "November" },
        { NaturalCronMonth.Dec, "December" },
    };

private static readonly Dictionary<NaturalCronDayOfWeek, string> WeekNameAbbreviations =
    new Dictionary<NaturalCronDayOfWeek, string>
    {
        { NaturalCronDayOfWeek.Sun, "Sun" },
        { NaturalCronDayOfWeek.Mon, "Mon" },
        { NaturalCronDayOfWeek.Tue, "Tue" },
        { NaturalCronDayOfWeek.Wed, "Wed" },
        { NaturalCronDayOfWeek.Thu, "Thu" },
        { NaturalCronDayOfWeek.Fri, "Fri" },
        { NaturalCronDayOfWeek.Sat, "Sat" },
    };

private static readonly Dictionary<NaturalCronDayOfWeek, string> WeekFullNames =
    new Dictionary<NaturalCronDayOfWeek, string>
    {
        { NaturalCronDayOfWeek.Sun, "Sunday" },
        { NaturalCronDayOfWeek.Mon, "Monday" },
        { NaturalCronDayOfWeek.Tue, "Tuesday" },
        { NaturalCronDayOfWeek.Wed, "Wednesday" },
        { NaturalCronDayOfWeek.Thu, "Thursday" },
        { NaturalCronDayOfWeek.Fri, "Friday" },
        { NaturalCronDayOfWeek.Sat, "Saturday" },
    };

    public INaturalCronStarterSelector UseAmpersatPrefix()
    {
        this.useAmpersat = true;
        this.prefix = "@";
        return this;
    }

    public INaturalCronStarterSelector UseWeekFullName()
    {
        this.useWeekFullName = true;
        return this;
    }
    
    public INaturalCronStarterSelector UseMonthFullName()
    {
        this.useMonthFullName = true;
        return this;
    }

    
    public INaturalEveryTimeUnitSelector Every()
    {
        this.everyRawExpr = $"{prefix}Every";
        return this;
    }

    public INaturalEveryTimeUnitWithValueSelector Every(int value)
    {
        this.everyRawExpr = $"{prefix}Every";
        this.everyValue = value;
        return this;
    }

    public INaturalCronTimeSpecificationSelector Yearly()
    {
        this.everyRawExpr = $"{prefix}Yearly";
        return this;
    }

    public INaturalCronTimeSpecificationSelector Monthly()
    {
        this.everyRawExpr = $"{prefix}Monthly";
        return this;
    }

    public INaturalCronTimeSpecificationSelector Weekly()
    {
        this.everyRawExpr = $"{prefix}Weekly";
        return this;
    }

    public INaturalCronTimeSpecificationSelector Daily()
    {
        this.everyRawExpr = $"{prefix}Daily";
        return this;
    }

    public INaturalCronTimeSpecificationSelector Hourly()
    {
        this.everyRawExpr = $"{prefix}Hourly";
        return this;
    }

    public INaturalCronTimeSpecificationSelector Minutely()
    {
        this.everyRawExpr = $"{prefix}Minutely";
        return this;
    }
    
    public INaturalCronTimeSpecificationSelector Secondly()
    {
        this.everyRawExpr = $"{prefix}Secondly";
        return this;
    }
    
    public INaturalCronTimeSpecificationSelector On(params string[] rawValues) => AppendToken("On", rawValues);
    public INaturalCronTimeSpecificationSelector In(params string[] rawValues) => AppendToken("In", rawValues);
    public INaturalCronTimeSpecificationSelector At(params string[] rawValues) => AppendToken("At", rawValues);

    public INaturalCronTimeSpecificationSelector On(string rawValue) => AppendToken("On", rawValue);
    public INaturalCronTimeSpecificationSelector In(string rawValue) => AppendToken("In", rawValue);
    public INaturalCronTimeSpecificationSelector At(string rawValue) => AppendToken("At", rawValue);

    public INaturalCronTimeSpecificationSelector From(string rawValue) => AppendToken("From", rawValue);
    public INaturalCronTimeSpecificationSelector Upto(string rawValue) => AppendToken("Upto", rawValue);
    

    public INaturalCronTimeSpecificationSelector Between(string startRawValue, string endRawValue) => AppendRangeToken("Between", (startRawValue, endRawValue));

    public INaturalCronTimeSpecificationSelector Between(params (string startRawValue, string endRawValue)[] ranges) => AppendRangeToken("Between", ranges);

    public INaturalCronTimeSpecificationSelector Between(DayOfWeek start, DayOfWeek end)
    {
        NaturalCronDayOfWeek startDayOfWeek = (NaturalCronDayOfWeek)start + 1;
        NaturalCronDayOfWeek endDayOfWeek = (NaturalCronDayOfWeek)end + 1;
        return this.Between(startDayOfWeek, endDayOfWeek);
    }

    public INaturalCronTimeSpecificationSelector Between(NaturalCronDayOfWeek start, NaturalCronDayOfWeek end)
    {
        var startRawValue = GetWeekName(start);
        var endRawValue = GetWeekName(end);
        return this.Between(startRawValue, endRawValue);
        
    }

    public INaturalCronTimeSpecificationSelector From(NaturalCronDayOfWeek value)
    {
        var rawValue = GetWeekName(value);
        return this.From(rawValue);
    }

    public INaturalCronTimeSpecificationSelector Upto(NaturalCronDayOfWeek value)
    {
        var rawValue = GetWeekName(value);
        return this.Upto(rawValue);
    }

    public INaturalCronTimeSpecificationSelector Between(NaturalCronMonth start, NaturalCronMonth end)
    {
        var startRawValue = GetMonthName(start);
        var endRawValue = GetMonthName(end);
        return this.Between(startRawValue, endRawValue);
    }

    public INaturalCronTimeSpecificationSelector From(NaturalCronMonth value)
    {
        var rawValue = GetMonthName(value);
        return this.From(rawValue);
    }

    public INaturalCronTimeSpecificationSelector Upto(NaturalCronMonth value)
    {
        var rawValue = GetMonthName(value);
        return this.Upto(rawValue);
    }

    public INaturalCronTimeSpecificationSelector FromDay(int day)
    {
        var rawValue = GetDayRawValue(day);
        return this.From(rawValue);
    }

    public INaturalCronTimeSpecificationSelector UptoDay(int day)
    {
        var rawValue = GetDayRawValue(day);
        return this.Upto(rawValue);
    }

    public INaturalCronTimeSpecificationSelector AtDayPosition(NaturalCronDayPosition position)
    {
        var rawValue = position switch
        {
            NaturalCronDayPosition.FirstDay => "FirstDay",
            NaturalCronDayPosition.FirstWeekday => "FirstWeekday",
            NaturalCronDayPosition.LastDay => "LastDay",
            NaturalCronDayPosition.LastWeekday => "LastWeekday",
            _ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
        };
        
        return this.At(rawValue);
    }

    public INaturalCronTimeSpecificationSelector OnNthWeekday(NaturalCronNthWeekDay nthDay, NaturalCronDayOfWeek dayOfWeek)
    {
        var weekRawValue = GetWeekName(dayOfWeek);
        var nthRawValue = nthDay switch
        {
            NaturalCronNthWeekDay.First => "1st",
            NaturalCronNthWeekDay.Second => "2nd",
            NaturalCronNthWeekDay.Third => "3rd",
            NaturalCronNthWeekDay.Fourth => "4th",
            NaturalCronNthWeekDay.Fifth => "5th",
            NaturalCronNthWeekDay.Last => "Last",
            _ => throw new ArgumentOutOfRangeException(nameof(nthDay), nthDay, null)
        };
        
        return this.On($"{nthRawValue}{weekRawValue}");
    }

    public INaturalCronTimeSpecificationSelector OnClosestWeekdayTo(int day)
    {
        var rawValue = "ClosestWeekdayTo " + GetDayRawValue(day);
        return this.On(rawValue);
    }

    public INaturalCronTimeSpecificationSelector AtTime(int hour, int minute, int? second, NaturalCronAmOrPm? amOrPm)
    {
        var rawValue = GetTimeRawValue(hour, minute, second, amOrPm);
        return this.At(rawValue);
    }

    public INaturalCronTimeSpecificationSelector BetweenTime(int startHour, int startMinute, int endHour, int endMinute, NaturalCronAmOrPm? amOrPm = null)
    {
        var startRawValue = GetTimeRawValue(startHour, startMinute, null, amOrPm);
        var endRawValue = GetTimeRawValue(endHour, endMinute, null, amOrPm);
        return this.Between(startRawValue, endRawValue);
    }

    public INaturalCronTimeSpecificationSelector BetweenTime(
        int startHour, int startMinute, int startSecond, int endHour, int endMinute, int endSecond,
        NaturalCronAmOrPm? amOrPm = null)
    {
        var startRawValue = GetTimeRawValue(startHour, startMinute, startSecond, amOrPm);
        var endRawValue = GetTimeRawValue(endHour, endMinute, endSecond, amOrPm);
        return this.Between(startRawValue, endRawValue);
    }

    public INaturalCronTimeSpecificationSelector FromTime(int hour, int minute, int? second = null, NaturalCronAmOrPm? amOrPm = null)
    {
        var rawValue = GetTimeRawValue(hour, minute, second, amOrPm);
        return this.From(rawValue);
    }

    public INaturalCronTimeSpecificationSelector UptoTime(int hour, int minute, int? second = null, NaturalCronAmOrPm? amOrPm = null)
    {
        var rawValue = GetTimeRawValue(hour, minute, second, amOrPm);
        return this.Upto(rawValue);
    }
    
#if NET6_0_OR_GREATER
    public INaturalCronTimeSpecificationSelector At(TimeOnly time, bool includeSeconds = false, NaturalCronAmOrPm? amOrPm = null)
    {
        var (hour, minute, second) = ExtractTimeComponents(time, includeSeconds, amOrPm);

        return this.AtTime(hour, minute, second, amOrPm);
    }

    public INaturalCronTimeSpecificationSelector From(TimeOnly value, bool includeSeconds = false, NaturalCronAmOrPm? amOrPm = null)
    {
        var (hour, minute, second) = ExtractTimeComponents(value, includeSeconds, amOrPm);
        return this.FromTime(hour, minute, second, amOrPm);
    }

    public INaturalCronTimeSpecificationSelector Upto(TimeOnly value, bool includeSeconds = false, NaturalCronAmOrPm? amOrPm = null)
    {
        var (hour, minute, second) = ExtractTimeComponents(value, includeSeconds, amOrPm);
        return this.UptoTime(hour, minute, second, amOrPm);
    }

    public INaturalCronTimeSpecificationSelector Between(TimeOnly start, TimeOnly end, bool includeSeconds = false, NaturalCronAmOrPm? amOrPm = null)
    {
        var (startHour, startMinute, startSecond) = ExtractTimeComponents(start, includeSeconds, amOrPm);
        var (endHour, endMinute, endSecond) = ExtractTimeComponents(end, includeSeconds, amOrPm);
        
        if (includeSeconds && startSecond.HasValue && endSecond.HasValue)
        {
            return this.BetweenTime(startHour, startMinute, startSecond.Value, endHour, endMinute, endSecond.Value, amOrPm);
        }
        
        return this.BetweenTime(startHour, startMinute, endHour, endMinute, amOrPm);
    }
#endif

    public INaturalCronTimeSpecificationSelector At(NaturalCronMonth month, int day)
    {
        var dayRawValue = GetDayRawValue(day);
        var monthRawValue = GetMonthName(month);
        var rawValue = $"{dayRawValue} {monthRawValue}";
        return this.At(rawValue);
    }

    public INaturalCronTimeSpecificationSelector Upto(NaturalCronMonth month, int day)
    {
        var dayRawValue = GetDayRawValue(day);
        var monthRawValue = GetMonthName(month);
        var rawValue = $"{dayRawValue} {monthRawValue}";
        return this.Upto(rawValue);
    }

    public INaturalCronTimeSpecificationSelector From(NaturalCronMonth month, int day)
    {
        var dayRawValue = GetDayRawValue(day);
        var monthRawValue = GetMonthName(month);
        var rawValue = $"{dayRawValue} {monthRawValue}";
        return this.From(rawValue);
    }

    public INaturalCronTimeSpecificationSelector Between(NaturalCronMonth startMonth, int startDay, NaturalCronMonth endMonth, int endDay)
    {
        var startDayRawValue = GetDayRawValue(startDay);
        var startMonthRawValue = GetMonthName(startMonth);
        var startRawValue = $"{startDayRawValue} {startMonthRawValue}";
        
        var endDayRawValue = GetDayRawValue(endDay);
        var endMonthRawValue = GetMonthName(endMonth);
        var endRawValue = $"{endDayRawValue} {endMonthRawValue}";
        
        return this.Between(startRawValue, endRawValue);
    }

    public INaturalCronTimeSpecificationSelector WithTimeZone(string ianaName)
    {
        this.ianaTimeZoneId = ianaName;
        return this;
    }

    public INaturalCronTimeSpecificationSelector On(DayOfWeek dayOfWeek)
    {
        NaturalCronDayOfWeek naturalCronDayOfWeek = (NaturalCronDayOfWeek)dayOfWeek + 1;
        return this.On(naturalCronDayOfWeek);
    }
    
    public INaturalCronTimeSpecificationSelector From(DayOfWeek value)
    {
        NaturalCronDayOfWeek naturalCronDayOfWeek = (NaturalCronDayOfWeek)value + 1;
        return this.From(naturalCronDayOfWeek);
    }

    public INaturalCronTimeSpecificationSelector Upto(DayOfWeek value)
    {
        NaturalCronDayOfWeek naturalCronDayOfWeek = (NaturalCronDayOfWeek)value + 1;
        return this.Upto(naturalCronDayOfWeek);
    }

    public INaturalCronTimeSpecificationSelector On(NaturalCronDayOfWeek dayOfWeek)
    {
        var rawValue = GetWeekName(dayOfWeek);
        return this.On(rawValue);
    }

    public INaturalCronTimeSpecificationSelector In(NaturalCronMonth month)
    {
        var rawValue = GetMonthName(month);
        return this.In(rawValue);
    }
    
    public INaturalCronTimeSpecificationSelector AtDay(int day)
    {
        var rawValue = GetDayRawValue(day);
        return this.At(rawValue);
    }

    public INaturalCronTimeSpecificationSelector Day()
    {
        this.everyTimeUnit = NaturalCronTimeUnit.Day;
        return this;
    }

    public INaturalCronTimeSpecificationSelector Week()
    {
        this.everyTimeUnit = NaturalCronTimeUnit.Week;
        return this;
    }

    public INaturalCronTimeSpecificationSelector Month()
    {
        this.everyTimeUnit = NaturalCronTimeUnit.Month;
        return this;
    }

    public INaturalCronTimeSpecificationSelector Year()
    {
        this.everyTimeUnit = NaturalCronTimeUnit.Year;
        return this;
    }

    public INaturalCronTimeSpecificationWithAnchoredSelector Days()
    {
        this.everyTimeUnit = NaturalCronTimeUnit.Day;
        return this;
    }

    public INaturalCronTimeSpecificationWithAnchoredSelector Weeks()
    {
        this.everyTimeUnit = NaturalCronTimeUnit.Week;
        return this;
    }

    public INaturalCronTimeSpecificationWithAnchoredSelector Months()
    {
        this.everyTimeUnit = NaturalCronTimeUnit.Month;
        return this;
    }

    public INaturalCronTimeSpecificationWithAnchoredSelector Years()
    {
        this.everyTimeUnit = NaturalCronTimeUnit.Year;
        return this;
    }

    public INaturalCronTimeSpecificationSelector AnchoredOn(string rawValue)
    {
        this.anchoredRawExpr = "AnchoredOn " + rawValue;
        return this;
    }

    public INaturalCronTimeSpecificationSelector AnchoredIn(string rawValue)
    {
        this.anchoredRawExpr = "AnchoredIn " + rawValue;
        return this;
    }

    public INaturalCronTimeSpecificationSelector AnchoredAt(string rawValue)
    {
        this.anchoredRawExpr = "AnchoredAt " + rawValue;
        return this;
    }

    public INaturalCronTimeSpecificationSelector AnchoredOn(DayOfWeek dayOfWeek)
    {
        NaturalCronDayOfWeek naturalCronDayOfWeek = (NaturalCronDayOfWeek)dayOfWeek + 1;
        return this.AnchoredOn(naturalCronDayOfWeek);
    }

    public INaturalCronTimeSpecificationSelector AnchoredIn(NaturalCronMonth month)
    {
        var rawValue = GetMonthName(month);
        return this.AnchoredIn(rawValue);
    }

    public INaturalCronTimeSpecificationSelector AnchoredOn(NaturalCronDayOfWeek dayOfWeek)
    {
        var rawValue = GetWeekName(dayOfWeek);
        return this.AnchoredOn(rawValue);
    }

    public INaturalCronTimeSpecificationSelector AnchoredOn(int value)
    {
        var rawValue = value.ToString();
        if (this.everyTimeUnit == NaturalCronTimeUnit.Day)
        {
            rawValue = GetDayRawValue(value);
        }
        
        return this.AnchoredOn(rawValue);
    }

    public INaturalCronTimeSpecificationSelector AnchoredIn(int value)
    {
        var rawValue = value.ToString();
        if (this.everyTimeUnit == NaturalCronTimeUnit.Day)
        {
            rawValue = GetDayRawValue(value);
        }
        
        return this.AnchoredIn(rawValue);
    }

    public INaturalCronTimeSpecificationSelector AnchoredAt(int value)
    {
        var rawValue = value.ToString();
        if (this.everyTimeUnit == NaturalCronTimeUnit.Day)
        {
            rawValue = GetDayRawValue(value);
        }
        
        return this.AnchoredAt(rawValue);
    }
    
    public string BuildExpr()
    {
        var result = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(everyRawExpr))
        {
            result.Append(everyRawExpr);

            if (everyValue.HasValue)
                result.Append(' ').Append(everyValue.Value);

            if (everyTimeUnit.HasValue)
            {
                result.Append(' ').Append(everyTimeUnit.Value.ToString().ToLower());
                if (everyValue.HasValue && everyValue.Value > 1)
                    result.Append('s');
            }

            if (!string.IsNullOrWhiteSpace(anchoredRawExpr))
                result.Append(' ').Append(anchoredRawExpr);
        }

        if (exprBuilder.Length > 0)
        {
            if (result.Length > 0)
                result.Append(' ');
            result.Append(exprBuilder);
        }

        if (!string.IsNullOrWhiteSpace(ianaTimeZoneId))
        {
            if (result.Length > 0)
                result.Append(' ');
            result.Append(prefix).Append("TimeZone ").Append(ianaTimeZoneId);
        }

        return result.ToString();
    }
    
    public NaturalCronExpr Build()
    {
        var expr = this.BuildExpr();
        return NaturalCronExpr.Parse(expr);
    }
    
    private static string GetDayRawValue(int day)
    {
        if (day % 100 is 11 or 12 or 13)
            return $"{day}th";

        string suffix = (day % 10) switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th"
        };

        return $"{day}{suffix}";
    }

    
    private static string GetTimeRawValue(int hour, int minute, int? second, NaturalCronAmOrPm? amOrPm)
    {
        // Format hour, minute, and second with zero-padding: 2 digits each
        var rawValue = $"{hour:D2}:{minute:D2}";
    
        if (second.HasValue)
        {
            rawValue += $":{second.Value:D2}";
        }

        if (amOrPm.HasValue)
        {
            rawValue += amOrPm.Value switch
            {
                NaturalCronAmOrPm.Am => "am",
                NaturalCronAmOrPm.Pm => "pm",
                _ => throw new ArgumentOutOfRangeException(nameof(amOrPm), amOrPm, null)
            };
        }

        return rawValue;
    }
    
    private string GetMonthName(NaturalCronMonth month) =>
        useMonthFullName ? MonthFullNames[month] : MonthNameAbbreviations[month];

    private string GetWeekName(NaturalCronDayOfWeek day) =>
        useWeekFullName ? WeekFullNames[day] : WeekNameAbbreviations[day];
    
    private INaturalCronTimeSpecificationSelector AppendRangeToken(string keyword, params (string start, string end)[] ranges)
    {
        string formatted;
        if (ranges.Length == 1)
        {
            formatted = $"{ranges[0].start} and {ranges[0].end}";
        }
        else
        {
            formatted = $"[{string.Join(", ", ranges.Select(r => $"{r.start} and {r.end}"))}]";
        }

        AppendWithSpace($"{prefix}{keyword} {formatted}");
        return this;
    }
    
    private void AppendWithSpace(string text)
    {
        if (exprBuilder.Length > 0)
            exprBuilder.Append(' ');
        exprBuilder.Append(text);
    }

    private static string FormatValues(string[] values) =>
        values.Length == 1 ? values[0] : $"[{string.Join(", ", values)}]";
    
    private INaturalCronTimeSpecificationSelector AppendToken(string keyword, params string[] values)
    {
        var formatted = FormatValues(values);
        AppendWithSpace($"{prefix}{keyword} {formatted}");
        return this;
    }

#if NET6_0_OR_GREATER
    private static (int hour, int minute, int? second) ExtractTimeComponents(TimeOnly time, bool includeSeconds, NaturalCronAmOrPm? amOrPm)
    {
        int hour = time.Hour;
        int minute = time.Minute;
        int? second = includeSeconds ? time.Second : null;
        
        if (amOrPm.HasValue)
        {
            var hour24 = time.Hour;
            if (hour24 < 12 && amOrPm.Value == NaturalCronAmOrPm.Pm)
            {
                hour24 += 12;
            }
            else if (hour24 > 12 && amOrPm.Value == NaturalCronAmOrPm.Am)
            {
                hour24 -= 12;
            }
            
            hour = hour24;
        }
        return (hour, minute, second);
    }
#endif
}