using RecurlyEx;
using RecurlyEx.Rules;
using FluentAssertions;

namespace RecurlyEx.UnitTests;

public class RecurlyExInBetweenRulesTests
{
    [Theory]
    [InlineData("@between 10:00 and 11:45")]
    public void Parse(string expression) {
        var (recurlyEx, errors) = RecurlyEx.InternalTryParse(expression);
        recurlyEx.Should().NotBeNull();
        var betweenRule = recurlyEx.MinuteRules().FirstOrDefault() as RecurlyExBetweenRule;
        
        betweenRule.Should().NotBeNull();
        betweenRule.StartEndValues[RecurlyExTimeUnit.Hour].StartExpr.Should().Be("10");
        betweenRule.StartEndValues[RecurlyExTimeUnit.Hour].EndExpr.Should().Be("11");
        betweenRule.StartEndValues[RecurlyExTimeUnit.Minute].StartExpr.Should().Be("00");
        betweenRule.StartEndValues[RecurlyExTimeUnit.Minute].EndExpr.Should().Be("45");
    }
}