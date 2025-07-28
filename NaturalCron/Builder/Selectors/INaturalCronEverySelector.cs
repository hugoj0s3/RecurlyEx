namespace NaturalCron.Builder.Selectors;

/// <summary>
/// Time-based selector interface for the NaturalCron fluent builder API.
/// Provides methods to create recurring expressions using "Every" patterns
/// and convenient shortcuts for common time intervals.
/// </summary>
public interface INaturalCronEverySelector
{
    /// <summary>
    /// Starts a new "Every" expression without value.
    /// Use this when you want to specify the time unit separately (e.g., Every().Day()).
    /// </summary>
    /// <returns>Time unit selector for specifying the interval type</returns>
    INaturalEveryTimeUnitSelector Every();
    
    /// <summary>
    /// Starts a new "Every" expression with a specific numeric value.
    /// Use this for intervals greater than 1 (e.g., Every(2).Days()).
    /// </summary>
    /// <param name="value">The interval value (must be positive)</param>
    /// <returns>Time unit selector with value for specifying the interval type</returns>
    INaturalEveryTimeUnitWithValueSelector Every(int value);
    
    /// <summary>
    /// Creates a yearly expression equivalent to "Every 1 Year".
    /// Convenient shortcut for annual recurring tasks.
    /// </summary>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Yearly();
    
    /// <summary>
    /// Creates a monthly expression equivalent to "Every 1 Month".
    /// Convenient shortcut for monthly recurring tasks.
    /// </summary>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Monthly();
    
    /// <summary>
    /// Creates a weekly expression equivalent to "Every 1 Week".
    /// Convenient shortcut for weekly recurring tasks.
    /// </summary>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Weekly();
    
    /// <summary>
    /// Creates a daily expression equivalent to "Every 1 Day".
    /// Convenient shortcut for daily recurring tasks.
    /// </summary>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Daily();
    
    /// <summary>
    /// Creates an hourly expression equivalent to "Every 1 Hour".
    /// Convenient shortcut for hourly recurring tasks.
    /// </summary>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Hourly();
    
    /// <summary>
    /// Creates a minutely expression equivalent to "Every 1 Minute".
    /// Convenient shortcut for tasks that run every minute.
    /// </summary>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Minutely();
    
    /// <summary>
    /// Creates a secondly expression equivalent to "Every 1 Second".
    /// Convenient shortcut for high-frequency recurring tasks.
    /// </summary>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Secondly();
}