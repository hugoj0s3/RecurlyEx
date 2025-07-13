using RecurlyEx.Utils;

namespace RecurlyEx.Rules;

public class RecurlyExAtInOnMultiplesRule : RecurlyExMatchableRule
{
    internal RecurlyExAtInOnMultiplesRule()
    {
    }
    
    private static readonly List<RecurlyExTimeUnit> TimeUnitsExceptTimeZone = Enum.GetValues(typeof(RecurlyExTimeUnit))
        .Cast<RecurlyExTimeUnit>()
        .Where(x => x != RecurlyExTimeUnit.TimeZone)
        .ToList();
    
    private static readonly List<RecurlyExTimeUnit> TimeUnitsExceptTimeZoneAsc = TimeUnitsExceptTimeZone
        .OrderBy(x => (int)x)
        .ToList();
    
    private static readonly List<RecurlyExTimeUnit> TimeUnitsExceptTimeZoneDesc = TimeUnitsExceptTimeZone
        .OrderByDescending(x => (int)x)
        .ToList();

    public override RecurlyExRuleSpec Spec => RecurlyExRuleSpec.AtInOn;

    internal RecurlyExAtInOnRule[] SecondRules { get; set; } = Array.Empty<RecurlyExAtInOnRule>();
    internal RecurlyExAtInOnRule[] MinuteRules { get; set; } = Array.Empty<RecurlyExAtInOnRule>();
    internal RecurlyExAtInOnRule[] HourRules { get; set; } = Array.Empty<RecurlyExAtInOnRule>();
    internal RecurlyExAtInOnRule[] DayRules { get; set; } = Array.Empty<RecurlyExAtInOnRule>();
    internal RecurlyExAtInOnRule[] WeekRules { get; set; } = Array.Empty<RecurlyExAtInOnRule>();
    internal RecurlyExAtInOnRule[] MonthRules { get; set; } = Array.Empty<RecurlyExAtInOnRule>();
    internal RecurlyExAtInOnRule[] YearRules { get; set; } = Array.Empty<RecurlyExAtInOnRule>();
    
    internal RecurlyExAtInOnRule[] GetAllRules()
    {
        return SecondRules
            .Concat(MinuteRules)
            .Concat(HourRules)
            .Concat(DayRules)
            .Concat(WeekRules)
            .Concat(MonthRules)
            .Concat(YearRules)
            .ToArray();
    }
    
    private int? lastYearOrder = null;
    private int? lastMonthOrder = null;

    internal bool HasTimeUnit(RecurlyExTimeUnit timeUnit)
    {
        switch (timeUnit)
        {
            case RecurlyExTimeUnit.Second:
                return SecondRules.Any();
            case RecurlyExTimeUnit.Minute:
                return MinuteRules.Any();
            case RecurlyExTimeUnit.Hour:
                return HourRules.Any();
            case RecurlyExTimeUnit.Day:
                return DayRules.Any();
            case RecurlyExTimeUnit.Week:
                return WeekRules.Any();
            case RecurlyExTimeUnit.Month:
                return MonthRules.Any();
            case RecurlyExTimeUnit.Year:
                return YearRules.Any();
            default:
                return false;
        }
    }

    protected override bool DoMatch(DateTime dateTime)
    {
        var length = GetLength();
        Reorder(dateTime.Year, dateTime.Month);

        for (var i = 0; i < length; i++)
        {
            if (Match(i, dateTime))
            {
                return true;
            }
        }

        return false;
    }

    internal int GetLength()
    {
        var length = new[]
            {
                SecondRules, MinuteRules, HourRules, DayRules, WeekRules, MonthRules, YearRules
            }
            .Max(x => x.Length);
        return length;
    }

    public bool Match(int index, DateTime dateTime)
    {
        Reorder(dateTime.Year, dateTime.Month);

        if (!Match(index, RecurlyExTimeUnit.Second, dateTime))
            return false;
        if (!Match(index, RecurlyExTimeUnit.Minute, dateTime))
            return false;
        if (!Match(index, RecurlyExTimeUnit.Hour, dateTime))
            return false;
        if (!Match(index, RecurlyExTimeUnit.Day, dateTime))
            return false;
        if (!Match(index, RecurlyExTimeUnit.Week, dateTime))
            return false;
        if (!Match(index, RecurlyExTimeUnit.Month, dateTime))
            return false;
        if (!Match(index, RecurlyExTimeUnit.Year, dateTime))
            return false;

        return true;
    }

    protected override (int, RecurlyExTimeUnit) DoGetTimeToAdvanceToNextOccurence(DateTime dateTime)
    {
        Reorder(dateTime.Year, dateTime.Month);
        var targetIndex = -1;
        foreach (var recurlyExTimeUnit in TimeUnitsExceptTimeZoneDesc)
        {
            for (var i = 0; i < GetLength(); i++)
            {
                var rules = GetRules(recurlyExTimeUnit);
                if (rules.Any())
                {
                    var valueForMatch = ExpressionUtil.TryGetValueForMatch(recurlyExTimeUnit, dateTime, rules[i].InnerExpression);
                    var partValue = DateTimeUtil.GetPartValue(recurlyExTimeUnit, dateTime);
                    if (valueForMatch.HasValue && partValue > valueForMatch.Value)
                    {
                        targetIndex = i;
                        break;
                    }
                }
            }

            if (targetIndex != -1)
            {
                break;
            }
        }

        if (targetIndex != -1)
        {
            var targetTimeUnit = TimeUnitsExceptTimeZoneAsc.FirstOrDefault(x => !Match(targetIndex, x, dateTime));
            var targetRule = GetRules(targetTimeUnit)[targetIndex];
            return targetRule.GetTimeToAdvanceToNextOccurence(dateTime);
        }

        return (1, this.TimeUnit);
    }

    private bool Match(int index, RecurlyExTimeUnit timeUnit, DateTime dateTime)
    {
        switch (timeUnit)
        {
            case RecurlyExTimeUnit.Second:
                return !SecondRules.Any() || SecondRules[index].Match(dateTime);
            case RecurlyExTimeUnit.Minute:
                return !MinuteRules.Any() || MinuteRules[index].Match(dateTime);
            case RecurlyExTimeUnit.Hour:
                return !HourRules.Any() || HourRules[index].Match(dateTime);
            case RecurlyExTimeUnit.Day:
                return !DayRules.Any() || DayRules[index].Match(dateTime);
            case RecurlyExTimeUnit.Week:
                return !WeekRules.Any() || WeekRules[index].Match(dateTime);
            case RecurlyExTimeUnit.Month:
                return !MonthRules.Any() || MonthRules[index].Match(dateTime);
            case RecurlyExTimeUnit.Year:
                return !YearRules.Any() || YearRules[index].Match(dateTime);
            default:
                throw new ArgumentOutOfRangeException(nameof(timeUnit), timeUnit, null);
        }
    }

    internal RecurlyExAtInOnRule[] GetRules(RecurlyExTimeUnit timeUnit)
    {
        if (timeUnit == RecurlyExTimeUnit.Second)
        {
            return SecondRules;
        }

        if (timeUnit == RecurlyExTimeUnit.Minute)
        {
            return MinuteRules;
        }

        if (timeUnit == RecurlyExTimeUnit.Hour)
        {
            return HourRules;
        }

        if (timeUnit == RecurlyExTimeUnit.Day)
        {
            return DayRules;
        }

        if (timeUnit == RecurlyExTimeUnit.Week)
        {
            return WeekRules;
        }

        if (timeUnit == RecurlyExTimeUnit.Month)
        {
            return MonthRules;
        }

        if (timeUnit == RecurlyExTimeUnit.Year)
        {
            return YearRules;
        }

        throw new ArgumentOutOfRangeException(nameof(timeUnit), timeUnit, null);
    }

    internal void Reorder(int year, int month)
    {
        if (lastYearOrder == year && 
            lastMonthOrder == month)
        {
            return;
        }
        
        var length = GetLength();

        if (length > 20)
        {
            QuickSort(year, month, 0, length - 1);
        }
        else
        {
            BubbleSort(year, month, length);
        }

        lastYearOrder = year;
        lastMonthOrder = month;
    }

    private void BubbleSort(int year, int month, int length)
    {
        for (int i = 0; i < length - 1; i++)
        {
            for (int j = 0; j < length - i - 1; j++)
            {
                if (CompareTo(year, month, j, j + 1) > 0)
                {
                    Swap(j, j + 1);
                }
            }
        }
    }

    private void QuickSort(int year, int month, int left, int right)
    {
        if (left < right)
        {
            int pivotIndex = Partition(year, month, left, right);
            QuickSort(year, month, left, pivotIndex - 1);
            QuickSort(year, month, pivotIndex + 1, right);
        }
    }

    private int Partition(int year, int month, int left, int right)
    {
        int pivotIndex = right;
        int storeIndex = left;
        for (int i = left; i < right; i++)
        {
            if (CompareTo(year, month, i, pivotIndex) < 0)
            {
                Swap(i, storeIndex);
                storeIndex++;
            }
        }

        Swap(storeIndex, pivotIndex);
        return storeIndex;
    }

    private int CompareTo(int year, int month, int indexA, int indexB)
    {
        var yearCompareTo = CompareTo(RecurlyExTimeUnit.Year, year, month, indexA, indexB);
        if (yearCompareTo != 0)
        {
            return yearCompareTo;
        }

        var monthCompareTo = CompareTo(RecurlyExTimeUnit.Month, year, month, indexA, indexB);
        if (monthCompareTo != 0)
        {
            return monthCompareTo;
        }

        var weekCompareTo = CompareTo(RecurlyExTimeUnit.Week, year, month, indexA, indexB);
        if (weekCompareTo != 0)
        {
            return weekCompareTo;
        }

        var dayCompareTo = CompareTo(RecurlyExTimeUnit.Day, year, month, indexA, indexB);
        if (dayCompareTo != 0)
        {
            return dayCompareTo;
        }

        var hourCompareTo = CompareTo(RecurlyExTimeUnit.Hour, year, month, indexA, indexB);
        if (hourCompareTo != 0)
        {
            return hourCompareTo;
        }

        var minuteCompareTo = CompareTo(RecurlyExTimeUnit.Minute, year, month, indexA, indexB);
        if (minuteCompareTo != 0)
        {
            return minuteCompareTo;
        }

        var secondCompareTo = CompareTo(RecurlyExTimeUnit.Second, year, month, indexA, indexB);
        if (secondCompareTo != 0)
        {
            return secondCompareTo;
        }

        return 0;
    }

    private int CompareTo(RecurlyExTimeUnit timeUnit, int year, int month, int indexA, int indexB)
    {
        var array = GetRules(timeUnit);
        if (array.Any())
        {
            var aValue = ExpressionUtil.TryGetValueForMatch(timeUnit, year, month, array[indexA].InnerExpression);
            var bValue = ExpressionUtil.TryGetValueForMatch(timeUnit, year, month, array[indexB].InnerExpression);
            
            if (!aValue.HasValue && !bValue.HasValue)
            {
                return 0;
            }

            if (aValue.HasValue && bValue.HasValue)
            {
                return aValue.Value.CompareTo(bValue.Value);
            }

            if (aValue.HasValue && !bValue.HasValue)
            {
                return 1;
            }

            if (!aValue.HasValue && bValue.HasValue)
            {
                return -1;
            }
        }

        return 0;
    }

    private void Swap(int indexA, int indexB)
    {
        var secondRules = GetRules(RecurlyExTimeUnit.Second);
        var minuteRules = GetRules(RecurlyExTimeUnit.Minute);
        var hourRules = GetRules(RecurlyExTimeUnit.Hour);
        var dayRules = GetRules(RecurlyExTimeUnit.Day);
        var weekRules = GetRules(RecurlyExTimeUnit.Week);
        var monthRules = GetRules(RecurlyExTimeUnit.Month);
        var yearRules = GetRules(RecurlyExTimeUnit.Year);

        SwapIfHasAny(secondRules, indexA, indexB);
        SwapIfHasAny(minuteRules, indexA, indexB);
        SwapIfHasAny(hourRules, indexA, indexB);
        SwapIfHasAny(dayRules, indexA, indexB);
        SwapIfHasAny(weekRules, indexA, indexB);
        SwapIfHasAny(monthRules, indexA, indexB);
        SwapIfHasAny(yearRules, indexA, indexB);
    }

    private void SwapIfHasAny(RecurlyExAtInOnRule[] array, int indexA, int indexB)
    {
        if (array.Any())
        {
            var temp = array[indexA];
            array[indexA] = array[indexB];
            array[indexB] = temp;
        }
    }
}