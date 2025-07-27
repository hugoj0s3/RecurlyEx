namespace NaturalCron.Builder.Selectors;

public interface INaturalEveryTimeUnitSelector
{
    INaturalCronFinalSelector Day();
    INaturalCronFinalSelector Week();
    INaturalCronFinalSelector Month();
    INaturalCronFinalSelector Year();
}