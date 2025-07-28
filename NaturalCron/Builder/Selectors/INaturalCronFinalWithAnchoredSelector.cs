namespace NaturalCron.Builder.Selectors;

/// <summary>
/// Extended final stage interface for the NaturalCron fluent builder API with anchoring support.
/// Provides all final operations plus anchoring methods to establish reference points
/// for recurring expressions (e.g., "Every 2 Weeks AnchoredOn Monday").
/// </summary>
public interface INaturalCronFinalWithAnchoredSelector : INaturalCronFinalSelector
{
    /// <summary>
    /// Defines the anchor point using a raw string value.
    /// </summary>
    /// <param name="rawValue">Raw string value for the anchor point</param>
    /// <returns></returns>
    INaturalCronFinalSelector AnchoredOn(string rawValue);
    
    /// <summary>
    /// Defines the anchor point using a raw string value for temporal context.
    /// </summary>
    /// <param name="rawValue">Raw string value for the anchor point</param>
    /// <returns></returns>
    INaturalCronFinalSelector AnchoredIn(string rawValue);
    
    /// <summary>
    /// Defines the anchor point using a raw string value for time context.
    /// </summary>
    /// <param name="rawValue">Raw string value for the anchor point</param>
    /// <returns></returns>
    INaturalCronFinalSelector AnchoredAt(string rawValue);

    /// <summary>
    /// Defines the anchor point using a specific day of the week.
    /// </summary>
    /// <param name="dayOfWeek">Day of the week for anchoring</param>
    /// <returns></returns>
    INaturalCronFinalSelector AnchoredOn(DayOfWeek dayOfWeek);
    
    /// <summary>
    /// Defines the anchor point using a specific month.
    /// </summary>
    /// <param name="month">Month for anchoring</param>
    /// <returns></returns>
    INaturalCronFinalSelector AnchoredIn(NaturalCronMonth month);
    
    /// <summary>
    /// Defines the anchor point using a specific day of the week from the NaturalCron enum.
    /// </summary>
    /// <param name="dayOfWeek">Day of the week for anchoring</param>
    /// <returns></returns>
    INaturalCronFinalSelector AnchoredOn(NaturalCronDayOfWeek dayOfWeek);

    /// <summary>
    /// Defines the anchor point using a numeric value (day, hour, etc.).
    /// </summary>
    /// <param name="value">Numeric value for the anchor point</param>
    /// <returns></returns>
    INaturalCronFinalSelector AnchoredOn(int value);
    
    /// <summary>
    /// Defines the anchor point using a numeric value for temporal context.
    /// </summary>
    /// <param name="value">Numeric value for the anchor point</param>
    /// <returns></returns>
    INaturalCronFinalSelector AnchoredIn(int value);
    
    /// <summary>
    /// Defines the anchor point using a numeric value for time context.
    /// </summary>
    /// <param name="value">Numeric value for the anchor point</param>
    /// <returns></returns>
    INaturalCronFinalSelector AnchoredAt(int value);
}