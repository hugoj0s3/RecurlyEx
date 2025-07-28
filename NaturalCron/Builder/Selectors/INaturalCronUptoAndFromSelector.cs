namespace NaturalCron.Builder.Selectors;

public interface INaturalCronUptoAndFromSelector
{
    /// <summary>
    /// Define From expression with raw value
    /// </summary>
    /// <param name="rawValue"></param>
    /// <returns></returns>
    INaturalCronFinalSelector From(string rawValue);
    
    /// <summary>
    /// Define Upto expression with raw value
    /// </summary>
    /// <param name="rawValue"></param>
    /// <returns></returns>
    INaturalCronFinalSelector Upto(string rawValue);
    
    /// <summary>
    /// Define From expression
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    INaturalCronFinalSelector From(DayOfWeek value);
    
    /// <summary>
    /// Define Upto expression
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    INaturalCronFinalSelector Upto(DayOfWeek value);
    
    /// <summary>
    /// Define From expression for week
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    INaturalCronFinalSelector From(NaturalCronDayOfWeek value);
    
    /// <summary>
    /// Define Upto expression for week
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    INaturalCronFinalSelector Upto(NaturalCronDayOfWeek value);
    
    /// <summary>
    /// Define From expression for month
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    INaturalCronFinalSelector From(NaturalCronMonth value);
    
    /// <summary>
    /// Define Upto expression for month
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    INaturalCronFinalSelector Upto(NaturalCronMonth value);
    
    /// <summary>
    /// Define From expression for specific day of month
    /// </summary>
    /// <param name="day">Day of the month (1-31)</param>
    /// <returns></returns>
    INaturalCronFinalSelector FromDay(int day);
    
    /// <summary>
    /// Define Upto expression for specific day of month
    /// </summary>
    /// <param name="day">Day of the month (1-31)</param>
    /// <returns></returns>
    INaturalCronFinalSelector UptoDay(int day);
#if NET6_0_OR_GREATER
    /// <summary>
    /// Define From expression for time using TimeOnly
    /// </summary>
    /// <param name="value">Time value</param>
    /// <param name="includeSeconds">Whether to include seconds in the expression</param>
    /// <param name="amOrPm">If null, use 24-hour time format</param>
    /// <returns></returns>
    INaturalCronFinalSelector From(TimeOnly value, bool includeSeconds = false, NaturalCronAmOrPm? amOrPm = null);
    
    /// <summary>
    /// Define Upto expression for time using TimeOnly
    /// </summary>
    /// <param name="value">Time value</param>
    /// <param name="includeSeconds">Whether to include seconds in the expression</param>
    /// <param name="amOrPm">If null, use 24-hour time format</param>
    /// <returns></returns>
    INaturalCronFinalSelector Upto(TimeOnly value, bool includeSeconds = false, NaturalCronAmOrPm? amOrPm = null);
#endif
    
    /// <summary>
    /// Define Upto expression for specific month and day combination
    /// </summary>
    /// <param name="month">Target month</param>
    /// <param name="day">Day of the month (1-31)</param>
    /// <returns></returns>
    INaturalCronFinalSelector Upto(NaturalCronMonth month, int day);
    
    /// <summary>
    /// Define From expression for specific month and day combination
    /// </summary>
    /// <param name="month">Target month</param>
    /// <param name="day">Day of the month (1-31)</param>
    /// <returns></returns>
    INaturalCronFinalSelector From(NaturalCronMonth month, int day);
    
    /// <summary>
    /// Define From expression for specific time
    /// </summary>
    /// <param name="hour">Hour (0-23 for 24-hour format, 1-12 for AM/PM format)</param>
    /// <param name="minute">Minute (0-59)</param>
    /// <param name="second">Optional second (0-59)</param>
    /// <param name="amOrPm">If null, use 24-hour time format</param>
    /// <returns></returns>
    INaturalCronFinalSelector FromTime(int hour, int minute, int? second = null, NaturalCronAmOrPm? amOrPm = null);
    
    /// <summary>
    /// Define Upto expression for specific time
    /// </summary>
    /// <param name="hour">Hour (0-23 for 24-hour format, 1-12 for AM/PM format)</param>
    /// <param name="minute">Minute (0-59)</param>
    /// <param name="second">Optional second (0-59)</param>
    /// <param name="amOrPm">If null, use 24-hour time format</param>
    /// <returns></returns>
    INaturalCronFinalSelector UptoTime(int hour, int minute, int? second = null, NaturalCronAmOrPm? amOrPm = null);
}