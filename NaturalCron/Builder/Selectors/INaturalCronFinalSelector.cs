namespace NaturalCron.Builder.Selectors;

/// <summary>
/// Final stage interface for the NaturalCron fluent builder API.
/// Provides access to range operations (Between, From, Upto), timezone configuration,
/// and build operations to complete the expression construction.
/// </summary>
public interface INaturalCronFinalSelector : 
    INaturalCronOnInAtSelector, 
    INaturalCronBuildSelector, 
    INaturalCronBetweenRangeSelector, 
    INaturalCronUptoAndFromSelector
{
    /// <summary>
    /// Defines the time zone for the expression.
    /// </summary>
    /// <param name="ianaTimeZoneId">IANA time zone identifier (e.g., "America/New_York")</param>
    /// <returns></returns>
    INaturalCronFinalSelector WithTimeZone(string ianaTimeZoneId);
}