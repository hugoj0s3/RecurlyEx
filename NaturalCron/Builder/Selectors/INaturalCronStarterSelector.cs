namespace NaturalCron.Builder.Selectors;

/// <summary>
/// Entry point interface for the NaturalCron fluent builder API.
/// Provides access to all initial builder operations including configuration options,
/// time-based selectors (Every), and direct selectors (On, In, At).
/// </summary>
public interface INaturalCronStarterSelector : 
    INaturalCronOnInAtSelector, 
    INaturalCronConfigSelector, 
    INaturalCronEverySelector
{
}