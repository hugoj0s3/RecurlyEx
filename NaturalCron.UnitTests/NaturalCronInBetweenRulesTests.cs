using System.Linq;
using NaturalCron;
using NaturalCron.Rules;
using FluentAssertions;

namespace NaturalCron.UnitTests;

public class NaturalCronInBetweenRulesTests
{
    [Theory]
    [InlineData("@between 10:00 and 11:45")]
    public void Parse(string expression) {
        var (naturalCronExpr, errors) = NaturalCronExpr.InternalTryParse(expression);
        naturalCronExpr.Should().NotBeNull();
        var betweenRule = naturalCronExpr.MinuteRules().FirstOrDefault() as NaturalCronBetweenRule;
        
        betweenRule.Should().NotBeNull();
        betweenRule.StartEndValues[NaturalCronTimeUnit.Hour].StartExpr.Should().Be("10");
        betweenRule.StartEndValues[NaturalCronTimeUnit.Hour].EndExpr.Should().Be("11");
        betweenRule.StartEndValues[NaturalCronTimeUnit.Minute].StartExpr.Should().Be("00");
        betweenRule.StartEndValues[NaturalCronTimeUnit.Minute].EndExpr.Should().Be("45");
    }
}