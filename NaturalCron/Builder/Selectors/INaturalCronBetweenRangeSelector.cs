namespace NaturalCron.Builder.Selectors;

public interface INaturalCronBetweenRangeSelector
{
    INaturalCronFinalSelector Between(string startRawValue, string endRawValue);
    INaturalCronFinalSelector Between(params (string startRawValue, string endRawValue)[] ranges);
    
    INaturalCronFinalSelector Between(DayOfWeek start, DayOfWeek end);
    
    INaturalCronFinalSelector Between(NaturalCronDayOfWeek start, NaturalCronDayOfWeek end);
    
    INaturalCronFinalSelector Between(NaturalCronMonth start, NaturalCronMonth end);

    INaturalCronFinalSelector BetweenTime(int startHour, int startMinute, int endHour, int endMinute, NaturalCronAmOrPm? amOrPm = null);

    INaturalCronFinalSelector BetweenTime(int startHour, int startMinute, int startSecond, int endHour, int endMinute, int endSecond,
        NaturalCronAmOrPm? amOrPm = null);
    
#if NET6_0_OR_GREATER
    INaturalCronFinalSelector Between(TimeOnly start, TimeOnly end, bool includeSeconds = false, NaturalCronAmOrPm? amOrPm = null);
#endif
    
    INaturalCronFinalSelector Between(NaturalCronMonth startMonth, int startDay, NaturalCronMonth endMonth, int endDay);
}