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
    
    public static INaturalCronTimeSpecificationSelector Yearly() => Start().Yearly();
    public static INaturalCronTimeSpecificationSelector Monthly() => Start().Monthly();
    public static INaturalCronTimeSpecificationSelector Weekly() => Start().Weekly();
    public static INaturalCronTimeSpecificationSelector Daily() => Start().Daily();
    public static INaturalCronTimeSpecificationSelector Hourly() => Start().Hourly();
    public static INaturalCronTimeSpecificationSelector Minutely() => Start().Minutely();
    public static INaturalCronTimeSpecificationSelector Secondly() => Start().Secondly();
    
    public static INaturalCronTimeSpecificationSelector On(string rawValue) => Start().On(rawValue);
    public static INaturalCronTimeSpecificationSelector In(string rawValue) => Start().In(rawValue);
    public static INaturalCronTimeSpecificationSelector At(string rawValue) => Start().At(rawValue);

    public static INaturalCronTimeSpecificationSelector On(params string[] rawValues) => Start().On(rawValues);
    public static INaturalCronTimeSpecificationSelector In(params string[] rawValues) => Start().In(rawValues);
    public static INaturalCronTimeSpecificationSelector At(params string[] rawValues) => Start().At(rawValues);

    // Weeks
    public static INaturalCronTimeSpecificationSelector On(DayOfWeek dayOfWeek) => Start().On(dayOfWeek);

    public static INaturalCronTimeSpecificationSelector On(NaturalCronDayOfWeek dayOfWeek) => Start().On(dayOfWeek);

    // Months
    public static INaturalCronTimeSpecificationSelector In(NaturalCronMonth month) => Start().In(month);

    // Days
    public static INaturalCronTimeSpecificationSelector AtDay(int day) => Start().AtDay(day);
    public static INaturalCronTimeSpecificationSelector AtDayPosition(NaturalCronDayPosition position) => Start().AtDayPosition(position);
    public static INaturalCronTimeSpecificationSelector OnNthWeekday(NaturalCronNthWeekDay nthDay, NaturalCronDayOfWeek dayOfWeek) => Start().OnNthWeekday(nthDay, dayOfWeek);
    public static INaturalCronTimeSpecificationSelector OnClosestWeekdayTo(int day) => Start().OnClosestWeekdayTo(day);
    
    // Time
    public static INaturalCronTimeSpecificationSelector AtTime(int hour, int minute, int? second = null, NaturalCronAmOrPm? amOrPm = null) => Start().AtTime(hour, minute, second, amOrPm);
}