namespace NaturalCron.Builder.Selectors;

/// <summary>
/// Time unit selector interface for single-value "Every" expressions in the NaturalCron fluent builder API.
/// Provides methods to specify the time unit for "Every" expressions with an implicit value of 1
/// (e.g., Every().Day() creates "Every 1 Day").
/// </summary>
public interface INaturalEveryTimeUnitSelector
{
    /// <summary>
    /// Specifies second as the time unit for the "Every" expression.
    /// Creates expressions like "Every 1 Second".
    /// </summary>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Second();
    
    /// <summary>
    /// Specifies minute as the time unit for the "Every" expression.
    /// Creates expressions like "Every 1 Minute".
    /// </summary>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Minute();
    
    /// <summary>
    /// Specifies hour as the time unit for the "Every" expression.
    /// Creates expressions like "Every 1 Hour".
    /// </summary>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Hour();
    
    /// <summary>
    /// Specifies day as the time unit for the "Every" expression.
    /// Creates expressions like "Every 1 Day".
    /// </summary>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Day();
    
    /// <summary>
    /// Specifies week as the time unit for the "Every" expression.
    /// Creates expressions like "Every 1 Week".
    /// </summary>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Week();
    
    /// <summary>
    /// Specifies month as the time unit for the "Every" expression.
    /// Creates expressions like "Every 1 Month".
    /// </summary>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Month();
    
    /// <summary>
    /// Specifies year as the time unit for the "Every" expression.
    /// Creates expressions like "Every 1 Year".
    /// </summary>
    /// <returns>Final selector for additional configuration</returns>
    INaturalCronFinalSelector Year();
}