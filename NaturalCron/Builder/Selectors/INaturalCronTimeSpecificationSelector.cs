namespace NaturalCron.Builder.Selectors;

public interface INaturalCronTimeSpecificationSelector : 
    INaturalCronOnInAtSelector, 
    INaturalCronBuildSelector, 
    INaturalCronBetweenRangeSelector, 
    INaturalCronUptoAndFromSelector
{
    // TimeZones
    INaturalCronTimeSpecificationSelector WithTimeZone(string ianaTimeZoneId);
}