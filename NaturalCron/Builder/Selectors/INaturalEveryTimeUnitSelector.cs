namespace NaturalCron.Builder.Selectors;

public interface INaturalEveryTimeUnitSelector
{
    INaturalCronTimeSpecificationSelector Day();
    INaturalCronTimeSpecificationSelector Week();
    INaturalCronTimeSpecificationSelector Month();
    INaturalCronTimeSpecificationSelector Year();
}