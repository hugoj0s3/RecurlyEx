namespace NaturalCron.Builder.Selectors;

public interface INaturalCronUptoAndFromSelector
{
    INaturalCronFinalSelector From(string rawValue);
    INaturalCronFinalSelector Upto(string rawValue);
    
    INaturalCronFinalSelector From(DayOfWeek value);
    INaturalCronFinalSelector Upto(DayOfWeek value);
    
    INaturalCronFinalSelector From(NaturalCronDayOfWeek value);
    INaturalCronFinalSelector Upto(NaturalCronDayOfWeek value);
    
    INaturalCronFinalSelector From(NaturalCronMonth value);
    INaturalCronFinalSelector Upto(NaturalCronMonth value);
    
    // Days
    INaturalCronFinalSelector FromDay(int day);
    INaturalCronFinalSelector UptoDay(int day);
#if NET6_0_OR_GREATER
    // Time only
    INaturalCronFinalSelector From(TimeOnly value, bool includeSeconds = false, NaturalCronAmOrPm? amOrPm = null);
    INaturalCronFinalSelector Upto(TimeOnly value, bool includeSeconds = false, NaturalCronAmOrPm? amOrPm = null);
#endif
    
    
    // Month + Day
    INaturalCronFinalSelector Upto(NaturalCronMonth month, int day);
    INaturalCronFinalSelector From(NaturalCronMonth month, int day);
    
    INaturalCronFinalSelector FromTime(int hour, int minute, int? second = null, NaturalCronAmOrPm? amOrPm = null);
    INaturalCronFinalSelector UptoTime(int hour, int minute, int? second = null, NaturalCronAmOrPm? amOrPm = null);
}