using NaturalCron.Utils;
using NaturalCron;
using NaturalCron.Rules;

namespace NaturalCron.Rules;

public class NaturalCronBetweenRule : NaturalCronMatchableRule
{
    internal NaturalCronBetweenRule()
    {
    }

    internal Dictionary<NaturalCronTimeUnit, NaturalCronBetweenStartEndValue> StartEndValues { get; }
        = new();


    public override NaturalCronRuleSpec Spec => NaturalCronRuleSpec.Between;
    
    private static readonly List<NaturalCronTimeUnit> TimeUnitsExceptTimezoneAndWeek = Enum.GetValues(typeof(NaturalCronTimeUnit))
        .Cast<NaturalCronTimeUnit>()
        .Where(x => x != NaturalCronTimeUnit.TimeZone && x != NaturalCronTimeUnit.Week)
        .OrderByDescending(x => (int)x)
        .ToList();

    protected override bool DoMatch(DateTime dateTime)
    {
        var (dateTimeStart, dateTimeEnd, dateTimeToCompare) = BuildDateTimesToCompare(dateTime);

        if (dateTimeToCompare < dateTimeStart || dateTimeToCompare > dateTimeEnd)
        {
            return false;
        }

        if (StartEndValues.TryGetValue(NaturalCronTimeUnit.Week, out var value))
        {
            var startWeekValue = ExpressionUtil.TryGetValueForMatch(NaturalCronTimeUnit.Week, dateTime, value.StartExpr);
            if (!startWeekValue.HasValue)
            {
                return false;
            }

            var endWeekValue = ExpressionUtil.TryGetValueForMatch(NaturalCronTimeUnit.Week, dateTime, value.EndExpr);
            if (!endWeekValue.HasValue)
            {
                return false;
            }

            var weekPartValue = DateTimeUtil.GetPartValue(NaturalCronTimeUnit.Week, dateTime);
            if (weekPartValue < startWeekValue.Value || weekPartValue > endWeekValue.Value)
            {
                return false;
            }
        }

        return true;
    }

    internal bool Validate()
    {
        foreach (var startEndValue in StartEndValues)
        {
            var isValidStart = ExpressionUtil.ValidateValueForMatch(startEndValue.Key, 1, 1, startEndValue.Value.StartExpr);
            var isValidEnd = ExpressionUtil.ValidateValueForMatch(startEndValue.Key, 1, 1, startEndValue.Value.EndExpr);
            if (!isValidStart || !isValidEnd)
            {
                return false;
            }
        }
        
        if (StartEndValues.ContainsKey(NaturalCronTimeUnit.Week))
        {
            var startWeekValue = ExpressionUtil.TryGetValueForMatch(NaturalCronTimeUnit.Week, DateTime.MinValue, StartEndValues[NaturalCronTimeUnit.Week].StartExpr);
            var endWeekValue = ExpressionUtil.TryGetValueForMatch(NaturalCronTimeUnit.Week, DateTime.MinValue, StartEndValues[NaturalCronTimeUnit.Week].EndExpr);
            if (!startWeekValue.HasValue || !endWeekValue.HasValue)
            {
                return false;
            }
            
            if (startWeekValue.Value > endWeekValue.Value)
            {
                return false;
            }
        }
        
        // Jan
        var dateTime = DateTime.MinValue;
        
        var (dateTimeStart, dateTimeEnd, _) = BuildDateTimesToCompare(dateTime);
        return dateTimeEnd >= dateTimeStart;
    }

    private BuildDateTimesToCompareResult? cachedBuildDateTimesToCompareResult;
    
    private (DateTime dateTimeStart, DateTime dateTimeEnd, DateTime dateTimeToCompare) BuildDateTimesToCompare(DateTime dateTime)
    {
        if (cachedBuildDateTimesToCompareResult != null && cachedBuildDateTimesToCompareResult.DateTime == dateTime)
        {
            return (
                cachedBuildDateTimesToCompareResult.DateTimeStart, 
                cachedBuildDateTimesToCompareResult.DateTimeEnd, 
                cachedBuildDateTimesToCompareResult.DateTimeToCompare);
            
        }
        
        var dateTimeStartArray = new int[] { dateTime.Year, dateTime.Month, 1, 0, 0, 0 };
        var dateTimeEndArray = new int[] { dateTime.Year, dateTime.Month, 1, 0, 0, 0 };
        var dateTimeToCompareArray = new int[] { dateTime.Year, dateTime.Month, 1, 0, 0, 0 };

        for (int i = 0; i < TimeUnitsExceptTimezoneAndWeek.Count; i++)
        {
            var timeUnit = TimeUnitsExceptTimezoneAndWeek[i];
            if (!StartEndValues.TryGetValue(timeUnit, out var startEndValue))
            {
                continue;
            }

            var startValue = ExpressionUtil.TryGetValueForMatch(timeUnit, dateTime, startEndValue.StartExpr);
            var endValue = ExpressionUtil.TryGetValueForMatch(timeUnit, dateTime, startEndValue.EndExpr);
            var partValue = DateTimeUtil.GetPartValue(timeUnit, dateTime);

            if (!startValue.HasValue || !endValue.HasValue)
            {
                break;
            }

            var maxValueByTimeUnitStart = ExpressionUtil.GetMaxValueByTimeUnit(timeUnit, dateTimeStartArray[0], dateTimeStartArray[1]);
            if (startValue.Value > maxValueByTimeUnitStart)
            {
                startValue = maxValueByTimeUnitStart;
            }

            var maxValueByTimeUnitEnd = ExpressionUtil.GetMaxValueByTimeUnit(timeUnit, dateTimeEndArray[0], dateTimeEndArray[1]);
            if (endValue.Value > maxValueByTimeUnitEnd)
            {
                endValue = maxValueByTimeUnitEnd;
            }

            if (startValue.Value < ExpressionUtil.GetMinValueByTimeUnit(timeUnit))
            {
                startValue = ExpressionUtil.GetMinValueByTimeUnit(timeUnit);
            }

            if (endValue.Value < ExpressionUtil.GetMinValueByTimeUnit(timeUnit))
            {
                endValue = ExpressionUtil.GetMinValueByTimeUnit(timeUnit);
            }

            dateTimeStartArray[i] = startValue.Value;
            dateTimeEndArray[i] = endValue.Value;
            dateTimeToCompareArray[i] = partValue;
        }
        
        cachedBuildDateTimesToCompareResult = new BuildDateTimesToCompareResult
        {
            DateTime = dateTime,
            DateTimeStart = new DateTime(dateTimeStartArray[0], dateTimeStartArray[1], dateTimeStartArray[2], dateTimeStartArray[3], dateTimeStartArray[4], dateTimeStartArray[5]),
            DateTimeEnd = new DateTime(dateTimeEndArray[0], dateTimeEndArray[1], dateTimeEndArray[2], dateTimeEndArray[3], dateTimeEndArray[4], dateTimeEndArray[5]),
            DateTimeToCompare = new DateTime(dateTimeToCompareArray[0], dateTimeToCompareArray[1], dateTimeToCompareArray[2], dateTimeToCompareArray[3], dateTimeToCompareArray[4], dateTimeToCompareArray[5])
        };
        
        return (
            cachedBuildDateTimesToCompareResult.DateTimeStart, 
            cachedBuildDateTimesToCompareResult.DateTimeEnd, 
            cachedBuildDateTimesToCompareResult.DateTimeToCompare);
    }

    protected override (int, NaturalCronTimeUnit) DoGetTimeToAdvanceToNextOccurence(DateTime dateTime)
    {
        // Week is a special case.
        if (StartEndValues.ContainsKey(NaturalCronTimeUnit.Week))
        {
            return (1, NaturalCronTimeUnit.Day);
        }
        
        var (dateTimeStart, _, _) = BuildDateTimesToCompare(dateTime);
        var secondsToAdvance = (int)(dateTimeStart - dateTime).TotalSeconds;
        if (secondsToAdvance > 0)
        {
            return (secondsToAdvance, NaturalCronTimeUnit.Second);
        }
        
        return GetMinSafeTimeToAdvance(dateTime);
    }

    private class BuildDateTimesToCompareResult
    {
        public DateTime DateTimeStart { get; set; } = DateTime.MinValue;
        public DateTime DateTimeEnd { get; set; } = DateTime.MaxValue;
        public DateTime DateTimeToCompare { get; set; } = DateTime.MinValue;
        
        public DateTime DateTime { get; set; } = DateTime.MinValue;
    }
}