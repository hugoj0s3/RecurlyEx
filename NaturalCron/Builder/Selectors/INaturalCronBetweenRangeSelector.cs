namespace NaturalCron.Builder.Selectors;

public interface INaturalCronBetweenRangeSelector
{
    INaturalCronTimeSpecificationSelector Between(string startRawValue, string endRawValue);
    INaturalCronTimeSpecificationSelector Between(params (string startRawValue, string endRawValue)[] ranges);
    
    INaturalCronTimeSpecificationSelector Between(DayOfWeek start, DayOfWeek end);
    
    INaturalCronTimeSpecificationSelector Between(NaturalCronDayOfWeek start, NaturalCronDayOfWeek end);
    
    INaturalCronTimeSpecificationSelector Between(NaturalCronMonth start, NaturalCronMonth end);

    INaturalCronTimeSpecificationSelector BetweenTime(int startHour, int startMinute, int endHour, int endMinute, NaturalCronAmOrPm? amOrPm = null);

    INaturalCronTimeSpecificationSelector BetweenTime(int startHour, int startMinute, int startSecond, int endHour, int endMinute, int endSecond,
        NaturalCronAmOrPm? amOrPm = null);
    
#if NET6_0_OR_GREATER
    INaturalCronTimeSpecificationSelector Between(TimeOnly start, TimeOnly end, bool includeSeconds = false, NaturalCronAmOrPm? amOrPm = null);
#endif
    
    INaturalCronTimeSpecificationSelector Between(NaturalCronMonth startMonth, int startDay, NaturalCronMonth endMonth, int endDay);
}