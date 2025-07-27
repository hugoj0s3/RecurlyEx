namespace NaturalCron.Builder.Selectors;

public interface INaturalCronTimeSpecificationWithAnchoredSelector : INaturalCronTimeSpecificationSelector
{
    INaturalCronTimeSpecificationSelector AnchoredOn(string rawValue);
    INaturalCronTimeSpecificationSelector AnchoredIn(string rawValue);
    INaturalCronTimeSpecificationSelector AnchoredAt(string rawValue);

    INaturalCronTimeSpecificationSelector AnchoredOn(DayOfWeek dayOfWeek);
    INaturalCronTimeSpecificationSelector AnchoredIn(NaturalCronMonth month);
    INaturalCronTimeSpecificationSelector AnchoredOn(NaturalCronDayOfWeek dayOfWeek);

    INaturalCronTimeSpecificationSelector AnchoredOn(int value);
    INaturalCronTimeSpecificationSelector AnchoredIn(int value);
    INaturalCronTimeSpecificationSelector AnchoredAt(int value);
}