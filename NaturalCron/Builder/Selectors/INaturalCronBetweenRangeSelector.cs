namespace NaturalCron.Builder.Selectors;

/// <summary>
/// Range selector interface for the NaturalCron fluent builder API.
/// Provides methods to create "Between" expressions for defining ranges
/// across various data types including strings, dates, times, and enums.
/// </summary>
public interface INaturalCronBetweenRangeSelector
{
    /// <summary>
    /// Creates a range between two raw string values.
    /// Useful for custom range specifications or complex expressions.
    /// </summary>
    /// <param name="startRawValue">Starting value of the range</param>
    /// <param name="endRawValue">Ending value of the range</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Between(string startRawValue, string endRawValue);
    
    /// <summary>
    /// Creates multiple ranges using pairs of raw string values.
    /// Allows for complex multi-range expressions like "Between (9-12, 14-17)".
    /// </summary>
    /// <param name="ranges">Array of tuples containing start and end values for each range</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Between(params (string startRawValue, string endRawValue)[] ranges);
    
    /// <summary>
    /// Creates a range between two days of the week using the standard DayOfWeek enum.
    /// For example, "Between Monday and Friday".
    /// </summary>
    /// <param name="start">Starting day of the week</param>
    /// <param name="end">Ending day of the week</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Between(DayOfWeek start, DayOfWeek end);
    
    /// <summary>
    /// Creates a range between two days of the week using the NaturalCron enum.
    /// Provides additional formatting options compared to the standard DayOfWeek enum.
    /// </summary>
    /// <param name="start">Starting day of the week from NaturalCron enumeration</param>
    /// <param name="end">Ending day of the week from NaturalCron enumeration</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Between(NaturalCronDayOfWeek start, NaturalCronDayOfWeek end);
    
    /// <summary>
    /// Creates a range between two months using the NaturalCron month enumeration.
    /// For example, "Between January and March".
    /// </summary>
    /// <param name="start">Starting month from NaturalCron enumeration</param>
    /// <param name="end">Ending month from NaturalCron enumeration</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Between(NaturalCronMonth start, NaturalCronMonth end);

    /// <summary>
    /// Creates a time range using hour and minute values.
    /// For example, "Between 9:00 and 17:00".
    /// </summary>
    /// <param name="startHour">Starting hour (0-23 for 24-hour format, 1-12 for AM/PM format)</param>
    /// <param name="startMinute">Starting minute (0-59)</param>
    /// <param name="endHour">Ending hour (0-23 for 24-hour format, 1-12 for AM/PM format)</param>
    /// <param name="endMinute">Ending minute (0-59)</param>
    /// <param name="amOrPm">If null, uses 24-hour format; otherwise specifies AM/PM for both times</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector BetweenTime(int startHour, int startMinute, int endHour, int endMinute, NaturalCronAmOrPm? amOrPm = null);

    /// <summary>
    /// Creates a time range using hour, minute, and second values.
    /// For example, "Between 9:30:15 and 17:45:30".
    /// </summary>
    /// <param name="startHour">Starting hour (0-23 for 24-hour format, 1-12 for AM/PM format)</param>
    /// <param name="startMinute">Starting minute (0-59)</param>
    /// <param name="startSecond">Starting second (0-59)</param>
    /// <param name="endHour">Ending hour (0-23 for 24-hour format, 1-12 for AM/PM format)</param>
    /// <param name="endMinute">Ending minute (0-59)</param>
    /// <param name="endSecond">Ending second (0-59)</param>
    /// <param name="amOrPm">If null, uses 24-hour format; otherwise specifies AM/PM for both times</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector BetweenTime(int startHour, int startMinute, int startSecond, int endHour, int endMinute, int endSecond,
        NaturalCronAmOrPm? amOrPm = null);
    
#if NET6_0_OR_GREATER
    /// <summary>
    /// Creates a time range using TimeOnly structs (.NET 6+).
    /// For example, "Between 09:00 and 17:00".
    /// </summary>
    /// <param name="start">Starting time using TimeOnly struct</param>
    /// <param name="end">Ending time using TimeOnly struct</param>
    /// <param name="includeSeconds">Whether to include seconds in the expression</param>
    /// <param name="amOrPm">If null, uses 24-hour format; otherwise specifies AM/PM for both times</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Between(TimeOnly start, TimeOnly end, bool includeSeconds = false, NaturalCronAmOrPm? amOrPm = null);
#endif
    
    /// <summary>
    /// Creates a date range using month and day combinations.
    /// For example, "Between January 15 and March 30".
    /// </summary>
    /// <param name="startMonth">Starting month from NaturalCron enumeration</param>
    /// <param name="startDay">Starting day of the month (1-31)</param>
    /// <param name="endMonth">Ending month from NaturalCron enumeration</param>
    /// <param name="endDay">Ending day of the month (1-31)</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Between(NaturalCronMonth startMonth, int startDay, NaturalCronMonth endMonth, int endDay);
}