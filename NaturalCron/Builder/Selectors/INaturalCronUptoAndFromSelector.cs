namespace NaturalCron.Builder.Selectors;

public interface INaturalCronUptoAndFromSelector
{
    INaturalCronTimeSpecificationSelector From(string rawValue);
    INaturalCronTimeSpecificationSelector Upto(string rawValue);
    
    INaturalCronTimeSpecificationSelector From(DayOfWeek value);
    INaturalCronTimeSpecificationSelector Upto(DayOfWeek value);
    
    INaturalCronTimeSpecificationSelector From(NaturalCronDayOfWeek value);
    INaturalCronTimeSpecificationSelector Upto(NaturalCronDayOfWeek value);
    
    // Days
    INaturalCronTimeSpecificationSelector FromDay(int day);
    INaturalCronTimeSpecificationSelector UptoDay(int day);
#if NET6_0_OR_GREATER
    // Time only
    INaturalCronTimeSpecificationSelector From(TimeOnly value, bool includeSeconds = false, NaturalCronAmOrPm? amOrPm = null);
    INaturalCronTimeSpecificationSelector Upto(TimeOnly value, bool includeSeconds = false, NaturalCronAmOrPm? amOrPm = null);
#endif
    
    
    // Month + Day
    INaturalCronTimeSpecificationSelector Upto(NaturalCronMonth month, int day);
    INaturalCronTimeSpecificationSelector From(NaturalCronMonth month, int day);
    
    INaturalCronTimeSpecificationSelector FromTime(int hour, int minute, int? second = null, NaturalCronAmOrPm? amOrPm = null);
    INaturalCronTimeSpecificationSelector UptoTime(int hour, int minute, int? second = null, NaturalCronAmOrPm? amOrPm = null);
}