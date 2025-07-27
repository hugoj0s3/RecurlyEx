namespace NaturalCron.Builder.Selectors;

public interface INaturalCronOnInAtSelector
{
    INaturalCronFinalSelector On(string rawValue);
    INaturalCronFinalSelector In(string rawValue);
    INaturalCronFinalSelector At(string rawValue);

    INaturalCronFinalSelector On(params string[] rawValues);
    INaturalCronFinalSelector In(params string[] rawValues);
    INaturalCronFinalSelector At(params string[] rawValues);

    // Weeks
    INaturalCronFinalSelector On(DayOfWeek dayOfWeek);

    INaturalCronFinalSelector On(NaturalCronDayOfWeek dayOfWeek);

    // Months
    INaturalCronFinalSelector In(NaturalCronMonth month);

    // Days
    INaturalCronFinalSelector AtDay(int day);
    INaturalCronFinalSelector AtDayPosition(NaturalCronDayPosition position);
    INaturalCronFinalSelector OnNthWeekday(NaturalCronNthWeekDay nthDay, NaturalCronDayOfWeek dayOfWeek);
    INaturalCronFinalSelector OnClosestWeekdayTo(int day);
    
    // Time
    INaturalCronFinalSelector AtTime(int hour, int minute, int? second = null, NaturalCronAmOrPm? amOrPm = null);
    
#if NET6_0_OR_GREATER
    // Time only
    INaturalCronFinalSelector At(TimeOnly time, bool includeSeconds = false, NaturalCronAmOrPm? amOrPm = null);
#endif

    // Month + Day
    INaturalCronFinalSelector At(NaturalCronMonth month, int day);
}