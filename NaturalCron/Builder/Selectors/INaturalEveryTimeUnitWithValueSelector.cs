namespace NaturalCron.Builder.Selectors;

public interface INaturalEveryTimeUnitWithValueSelector
{
    INaturalCronFinalWithAnchoredSelector Days();
    INaturalCronFinalWithAnchoredSelector Weeks();
    INaturalCronFinalWithAnchoredSelector Months();
    INaturalCronFinalWithAnchoredSelector Years();
}