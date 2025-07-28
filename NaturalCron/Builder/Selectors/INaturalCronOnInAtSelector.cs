namespace NaturalCron.Builder.Selectors;

/// <summary>
/// Core selector interface for the NaturalCron fluent builder API.
/// Provides methods to create expressions using "On", "In", and "At" patterns
/// for specifying days, months, times, and complex date/time combinations.
/// </summary>
public interface INaturalCronOnInAtSelector
{
    /// <summary>
    /// Creates an "On" expression using a raw string value.
    /// Useful for custom day specifications or complex expressions.
    /// </summary>
    /// <param name="rawValue">Raw string value for the "On" expression</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector On(string rawValue);
    
    /// <summary>
    /// Creates an "In" expression using a raw string value.
    /// Useful for custom month specifications or complex expressions.
    /// </summary>
    /// <param name="rawValue">Raw string value for the "In" expression</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector In(string rawValue);
    
    /// <summary>
    /// Creates an "At" expression using a raw string value.
    /// Useful for custom time specifications or complex expressions.
    /// </summary>
    /// <param name="rawValue">Raw string value for the "At" expression</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector At(string rawValue);

    /// <summary>
    /// Creates an "On" expression with multiple raw string values.
    /// Useful for specifying multiple days or complex multi-value expressions.
    /// </summary>
    /// <param name="rawValues">Array of raw string values for the "On" expression</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector On(params string[] rawValues);
    
    /// <summary>
    /// Creates an "In" expression with multiple raw string values.
    /// Useful for specifying multiple months or complex multi-value expressions.
    /// </summary>
    /// <param name="rawValues">Array of raw string values for the "In" expression</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector In(params string[] rawValues);
    
    /// <summary>
    /// Creates an "At" expression with multiple raw string values.
    /// Useful for specifying multiple times or complex multi-value expressions.
    /// </summary>
    /// <param name="rawValues">Array of raw string values for the "At" expression</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector At(params string[] rawValues);

    /// <summary>
    /// Creates an "On" expression for a specific day of the week using the standard DayOfWeek enum.
    /// </summary>
    /// <param name="dayOfWeek">Day of the week (Sunday through Saturday)</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector On(DayOfWeek dayOfWeek);

    /// <summary>
    /// Creates an "On" expression for a specific day of the week using the NaturalCron enum.
    /// Provides additional formatting options compared to the standard DayOfWeek enum.
    /// </summary>
    /// <param name="dayOfWeek">Day of the week from NaturalCron enumeration</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector On(NaturalCronDayOfWeek dayOfWeek);

    /// <summary>
    /// Creates an "In" expression for a specific month using the NaturalCron month enumeration.
    /// </summary>
    /// <param name="month">Month from NaturalCron enumeration</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector In(NaturalCronMonth month);

    /// <summary>
    /// Creates an "At" expression for a specific day of the month.
    /// </summary>
    /// <param name="day">Day of the month (1-31)</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector AtDay(int day);
    
    /// <summary>
    /// Creates an "At" expression for a specific day position (first, last, etc.).
    /// </summary>
    /// <param name="position">Day position from NaturalCron enumeration</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector AtDayPosition(NaturalCronDayPosition position);
    
    /// <summary>
    /// Creates an "On" expression for the nth occurrence of a specific weekday in a month.
    /// For example, "On the 2nd Tuesday" of the month.
    /// </summary>
    /// <param name="nthDay">Which occurrence (1st, 2nd, 3rd, etc.)</param>
    /// <param name="dayOfWeek">Day of the week</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector OnNthWeekday(NaturalCronNthWeekDay nthDay, NaturalCronDayOfWeek dayOfWeek);
    
    /// <summary>
    /// Creates an "On" expression for the closest weekday to a specific day of the month.
    /// If the specified day falls on a weekend, it will select the nearest weekday.
    /// </summary>
    /// <param name="day">Target day of the month (1-31)</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector OnClosestWeekdayTo(int day);
    
    /// <summary>
    /// Creates an "At" expression for a specific time using hour, minute, and optional second.
    /// </summary>
    /// <param name="hour">Hour (0-23 for 24-hour format, 1-12 for AM/PM format)</param>
    /// <param name="minute">Minute (0-59)</param>
    /// <param name="second">Optional second (0-59)</param>
    /// <param name="amOrPm">If null, uses 24-hour format; otherwise specifies AM/PM</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector AtTime(int hour, int minute, int? second = null, NaturalCronAmOrPm? amOrPm = null);
    
#if NET6_0_OR_GREATER
    /// <summary>
    /// Creates an "At" expression for a specific time using the TimeOnly struct (.NET 6+).
    /// </summary>
    /// <param name="time">Time value using TimeOnly struct</param>
    /// <param name="includeSeconds">Whether to include seconds in the expression</param>
    /// <param name="amOrPm">If null, uses 24-hour format; otherwise specifies AM/PM</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector At(TimeOnly time, bool includeSeconds = false, NaturalCronAmOrPm? amOrPm = null);
#endif

    /// <summary>
    /// Creates an "At" expression for a specific date using month and day combination.
    /// </summary>
    /// <param name="month">Month from NaturalCron enumeration</param>
    /// <param name="day">Day of the month (1-31)</param>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector At(NaturalCronMonth month, int day);
}