namespace NaturalCron.Builder.Selectors;

/// <summary>
/// Time unit selector interface for multi-value "Every" expressions in the NaturalCron fluent builder API.
/// Provides methods to specify the time unit for "Every" expressions with a specific numeric value
/// (e.g., Every(2).Days() creates "Every 2 Days"). Supports anchoring for precise scheduling.
/// </summary>
public interface INaturalEveryTimeUnitWithValueSelector
{
    /// <summary>
    /// Specifies seconds as the time unit for the "Every" expression with the provided value.
    /// Creates expressions like "Every 5 Seconds".
    /// </summary>
    /// <returns>Final selector with anchoring support for additional configuration</returns>
    INaturalCronFinalWithAnchoredSelector Seconds();
    
    /// <summary>
    /// Specifies minutes as the time unit for the "Every" expression with the provided value.
    /// Creates expressions like "Every 15 Minutes".
    /// </summary>
    /// <returns>Final selector with anchoring support for additional configuration</returns>
    INaturalCronFinalWithAnchoredSelector Minutes();
    
    /// <summary>
    /// Specifies hours as the time unit for the "Every" expression with the provided value.
    /// Creates expressions like "Every 4 Hours".
    /// </summary>
    /// <returns>Final selector with anchoring support for additional configuration</returns>
    INaturalCronFinalWithAnchoredSelector Hours();
    
    /// <summary>
    /// Specifies days as the time unit for the "Every" expression with the provided value.
    /// Creates expressions like "Every 3 Days".
    /// </summary>
    /// <returns>Final selector with anchoring support for additional configuration</returns>
    INaturalCronFinalWithAnchoredSelector Days();
    
    /// <summary>
    /// Specifies weeks as the time unit for the "Every" expression with the provided value.
    /// Creates expressions like "Every 2 Weeks".
    /// </summary>
    /// <returns>Final selector with anchoring support for additional configuration</returns>
    INaturalCronFinalWithAnchoredSelector Weeks();
    
    /// <summary>
    /// Specifies months as the time unit for the "Every" expression with the provided value.
    /// Creates expressions like "Every 6 Months".
    /// </summary>
    /// <returns>Final selector with anchoring support for additional configuration</returns>
    INaturalCronFinalWithAnchoredSelector Months();
    
    /// <summary>
    /// Specifies years as the time unit for the "Every" expression with the provided value.
    /// Creates expressions like "Every 2 Years".
    /// </summary>
    /// <returns>Final selector with anchoring support for additional configuration</returns>
    INaturalCronFinalWithAnchoredSelector Years();
}