namespace NaturalCron.Builder.Selectors;

public interface INaturalCronFinalWithAnchoredSelector : INaturalCronFinalSelector
{
    INaturalCronFinalSelector AnchoredOn(string rawValue);
    INaturalCronFinalSelector AnchoredIn(string rawValue);
    INaturalCronFinalSelector AnchoredAt(string rawValue);

    INaturalCronFinalSelector AnchoredOn(DayOfWeek dayOfWeek);
    INaturalCronFinalSelector AnchoredIn(NaturalCronMonth month);
    INaturalCronFinalSelector AnchoredOn(NaturalCronDayOfWeek dayOfWeek);

    INaturalCronFinalSelector AnchoredOn(int value);
    INaturalCronFinalSelector AnchoredIn(int value);
    INaturalCronFinalSelector AnchoredAt(int value);
}