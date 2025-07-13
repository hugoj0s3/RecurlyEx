using TimeZoneConverter;

namespace RecurlyEx.Rules;

public class RecurlyExIanaTimeZoneRule : RecurlyExRule
{
    public override RecurlyExRuleSpec Spec => RecurlyExRuleSpec.TimeZone;

    public string IanaId { get; internal set; } = string.Empty;

    public DateTime ConvertFromUtc(DateTime utcDateTime)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TZConvert.GetTimeZoneInfo(IanaId));
    }
    
    public DateTime ConvertToUtc(DateTime localDateTime)
    {
        return TimeZoneInfo.ConvertTimeToUtc(localDateTime, TZConvert.GetTimeZoneInfo(IanaId));
    }
}