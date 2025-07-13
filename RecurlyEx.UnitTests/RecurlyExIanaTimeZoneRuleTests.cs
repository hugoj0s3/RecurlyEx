using RecurlyEx;
using RecurlyEx.Rules;
using FluentAssertions;

namespace RecurlyEx.UnitTests;

public class RecurlyExIanaTimeZoneRuleTests
{
    [Theory]
    [InlineData("@tz America/Los_Angeles")]
    [InlineData("@TimeZone America/Los_Angeles")]
    public void TryParse_IanaTimeZone_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.InternalTryParse(expression);
        errors.Should().Contain("At least one At/On/In or Every/Secondly/Minutely/Hourly/Daily/Weekly/Monthly/Yearly is required.");
        recurlyEx.Should().NotBeNull();
        recurlyEx.TimeZoneRule().Should().NotBeNull();
        var timeZoneRule = recurlyEx.TimeZoneRule() as RecurlyExIanaTimeZoneRule;
        timeZoneRule.Should().NotBeNull();
        timeZoneRule.IanaId.Should().Be("America/Los_Angeles");
    }
}