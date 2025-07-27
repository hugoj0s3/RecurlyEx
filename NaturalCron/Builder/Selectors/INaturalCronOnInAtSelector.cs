namespace NaturalCron.Builder.Selectors;

public interface INaturalCronOnInAtSelector
{
    INaturalCronTimeSpecificationSelector On(string rawValue);
    INaturalCronTimeSpecificationSelector In(string rawValue);
    INaturalCronTimeSpecificationSelector At(string rawValue);

    INaturalCronTimeSpecificationSelector On(params string[] rawValues);
    INaturalCronTimeSpecificationSelector In(params string[] rawValues);
    INaturalCronTimeSpecificationSelector At(params string[] rawValues);

    // Weeks
    INaturalCronTimeSpecificationSelector On(DayOfWeek dayOfWeek);

    INaturalCronTimeSpecificationSelector On(NaturalCronDayOfWeek dayOfWeek);

    // Months
    INaturalCronTimeSpecificationSelector In(NaturalCronMonth month);

    // Days
    INaturalCronTimeSpecificationSelector AtDay(int day);
    INaturalCronTimeSpecificationSelector AtDayPosition(NaturalCronDayPosition position);
    INaturalCronTimeSpecificationSelector OnNthWeekday(NaturalCronNthWeekDay nthDay, NaturalCronDayOfWeek dayOfWeek);
    INaturalCronTimeSpecificationSelector OnClosestWeekdayTo(int day);
    
    // Time
    INaturalCronTimeSpecificationSelector AtTime(int hour, int minute, int? second = null, NaturalCronAmOrPm? amOrPm = null);
    
#if NET6_0_OR_GREATER
    // Time only
    INaturalCronTimeSpecificationSelector At(TimeOnly time, bool includeSeconds = false, NaturalCronAmOrPm? amOrPm = null);
#endif

    // Month + Day
    INaturalCronTimeSpecificationSelector At(NaturalCronMonth month, int day);
}