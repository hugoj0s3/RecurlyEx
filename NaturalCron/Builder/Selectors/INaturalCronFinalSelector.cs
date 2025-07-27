namespace NaturalCron.Builder.Selectors;

public interface INaturalCronFinalSelector : 
    INaturalCronOnInAtSelector, 
    INaturalCronBuildSelector, 
    INaturalCronBetweenRangeSelector, 
    INaturalCronUptoAndFromSelector
{
    // TimeZones
    INaturalCronFinalSelector WithTimeZone(string ianaTimeZoneId);
}