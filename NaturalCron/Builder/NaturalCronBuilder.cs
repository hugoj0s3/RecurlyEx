using NaturalCron.Builder.Selectors;

namespace NaturalCron.Builder;

public static class NaturalCronBuilder
{
    public static INaturalCronStarterSelector Start() => new NaturalCronBuilderImpl();
    public static INaturalCronStarterSelector UseAmpersat() => Start().UseAmpersatPrefix();
    public static INaturalCronStarterSelector UseWeekFullName() => Start().UseWeekFullName();
    public static INaturalCronStarterSelector UseMonthFullName() => Start().UseMonthFullName();
    
    public static INaturalEveryTimeUnitSelector Every() => Start().Every();
    public static INaturalEveryTimeUnitWithValueSelector Every(int value) => Start().Every(value);
    
    public static INaturalCronFinalSelector Yearly() => Start().Yearly();
    public static INaturalCronFinalSelector Monthly() => Start().Monthly();
    public static INaturalCronFinalSelector Weekly() => Start().Weekly();
    public static INaturalCronFinalSelector Daily() => Start().Daily();
    public static INaturalCronFinalSelector Hourly() => Start().Hourly();
    public static INaturalCronFinalSelector Minutely() => Start().Minutely();
    public static INaturalCronFinalSelector Secondly() => Start().Secondly();
    
    public static INaturalCronFinalSelector On(string rawValue) => Start().On(rawValue);
    public static INaturalCronFinalSelector In(string rawValue) => Start().In(rawValue);
    public static INaturalCronFinalSelector At(string rawValue) => Start().At(rawValue);

    public static INaturalCronFinalSelector On(params string[] rawValues) => Start().On(rawValues);
    public static INaturalCronFinalSelector In(params string[] rawValues) => Start().In(rawValues);
    public static INaturalCronFinalSelector At(params string[] rawValues) => Start().At(rawValues);

    // Weeks
    public static INaturalCronFinalSelector On(DayOfWeek dayOfWeek) => Start().On(dayOfWeek);

    public static INaturalCronFinalSelector On(NaturalCronDayOfWeek dayOfWeek) => Start().On(dayOfWeek);

    // Months
    public static INaturalCronFinalSelector In(NaturalCronMonth month) => Start().In(month);

    // Days
    public static INaturalCronFinalSelector AtDay(int day) => Start().AtDay(day);
    public static INaturalCronFinalSelector AtDayPosition(NaturalCronDayPosition position) => Start().AtDayPosition(position);
    public static INaturalCronFinalSelector OnNthWeekday(NaturalCronNthWeekDay nthDay, NaturalCronDayOfWeek dayOfWeek) => Start().OnNthWeekday(nthDay, dayOfWeek);
    public static INaturalCronFinalSelector OnClosestWeekdayTo(int day) => Start().OnClosestWeekdayTo(day);
    
    // Time
    public static INaturalCronFinalSelector AtTime(int hour, int minute, int? second = null, NaturalCronAmOrPm? amOrPm = null) => Start().AtTime(hour, minute, second, amOrPm);
}