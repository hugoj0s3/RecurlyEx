namespace NaturalCron.Builder.Selectors;

public interface INaturalCronEverySelector
{
    INaturalEveryTimeUnitSelector Every();
    INaturalEveryTimeUnitWithValueSelector Every(int value);
    
    INaturalCronFinalSelector Yearly();
    INaturalCronFinalSelector Monthly();
    INaturalCronFinalSelector Weekly();
    INaturalCronFinalSelector Daily();
    INaturalCronFinalSelector Hourly();
    INaturalCronFinalSelector Minutely();
    INaturalCronFinalSelector Secondly();
}