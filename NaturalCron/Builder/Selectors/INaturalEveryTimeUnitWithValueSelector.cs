namespace NaturalCron.Builder.Selectors;

public interface INaturalEveryTimeUnitWithValueSelector
{
    INaturalCronTimeSpecificationWithAnchoredSelector Days();
    INaturalCronTimeSpecificationWithAnchoredSelector Weeks();
    INaturalCronTimeSpecificationWithAnchoredSelector Months();
    INaturalCronTimeSpecificationWithAnchoredSelector Years();
}