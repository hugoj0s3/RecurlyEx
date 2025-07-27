namespace NaturalCron.Builder.Selectors;

public interface INaturalCronStarterSelector : 
    INaturalCronOnInAtSelector, 
    INaturalCronConfigSelector, 
    INaturalCronEverySelector
{
}