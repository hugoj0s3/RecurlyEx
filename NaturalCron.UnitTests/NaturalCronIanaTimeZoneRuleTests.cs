using NaturalCron;
using NaturalCron.Rules;
using FluentAssertions;

namespace NaturalCron.UnitTests;

public class NaturalCronIanaTimeZoneRuleTests
{
    [Theory]
    [InlineData("@tz America/Los_Angeles")]
    [InlineData("@TimeZone America/Los_Angeles")]
    public void TryParse_IanaTimeZone_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.InternalTryParse(expression);
        errors.Should().Contain("At least one At/On/In or Every/Secondly/Minutely/Hourly/Daily/Weekly/Monthly/Yearly is required.");
        naturalCronExpr.Should().NotBeNull();
        naturalCronExpr.TimeZoneRule().Should().NotBeNull();
        var timeZoneRule = naturalCronExpr.TimeZoneRule() as NaturalCronIanaTimeZoneRule;
        timeZoneRule.Should().NotBeNull();
        timeZoneRule.IanaId.Should().Be("America/Los_Angeles");
    }
}