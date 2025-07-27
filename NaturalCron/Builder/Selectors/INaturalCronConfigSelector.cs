namespace NaturalCron.Builder.Selectors;

public interface INaturalCronConfigSelector
{
    INaturalCronStarterSelector UseAmpersatPrefix();

    INaturalCronStarterSelector UseWeekFullName();

    INaturalCronStarterSelector UseMonthFullName();
}