namespace NaturalCron.Builder.Selectors;

/// <summary>
/// Configuration options interface for the NaturalCron fluent builder API.
/// Provides methods to customize the output format of generated expressions,
/// including prefix options and name formatting preferences.
/// </summary>
public interface INaturalCronConfigSelector
{
    /// <summary>
    /// Configures the builder to use ampersat (@) prefix for the generated expressions.
    /// This prefix can be useful for distinguishing NaturalCron expressions from standard syntax.
    /// </summary>
    /// <returns>Builder instance for method chaining</returns>
    INaturalCronStarterSelector UseAmpersatPrefix();

    /// <summary>
    /// Configures the builder to use full weekday names (e.g., "Monday") instead of abbreviations (e.g., "Mon").
    /// This makes expressions more readable but longer in format.
    /// </summary>
    /// <returns>Builder instance for method chaining</returns>
    INaturalCronStarterSelector UseWeekFullName();

    /// <summary>
    /// Configures the builder to use full month names (e.g., "January") instead of abbreviations (e.g., "Jan").
    /// This makes expressions more readable but longer in format.
    /// </summary>
    /// <returns>Builder instance for method chaining</returns>
    INaturalCronStarterSelector UseMonthFullName();
}