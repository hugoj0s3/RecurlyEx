namespace NaturalCron.Builder.Selectors;

public interface INaturalCronEverySelector
{
    INaturalEveryTimeUnitSelector Every();
    INaturalEveryTimeUnitWithValueSelector Every(int value);
    
    INaturalCronTimeSpecificationSelector Yearly();
    INaturalCronTimeSpecificationSelector Monthly();
    INaturalCronTimeSpecificationSelector Weekly();
    INaturalCronTimeSpecificationSelector Daily();
    INaturalCronTimeSpecificationSelector Hourly();
    INaturalCronTimeSpecificationSelector Minutely();
    INaturalCronTimeSpecificationSelector Secondly();
}