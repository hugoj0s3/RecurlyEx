using RecurlyEx;
using FluentAssertions;

namespace RecurlyEx.UnitTests;

public class RecurlyExValidationTests
{
    [Theory]
    [InlineData("@every 2 days @on 5th", "You cannot use an At/On/In rules at the same time unit as an 'every X' rule except for week.")]
    [InlineData("@every day @on monday", "You cannot combine an 'every day' rule with an At/On/In rule for weeks.")]
    [InlineData("@every week AnchoredOn friday @on monday ", "You cannot combine an 'every week' + anchored rule with an At/On/In rule for weeks.")]
    [InlineData("@on monday @on tuesday", "Duplicate At/On/In rules for the same time unit are not allowed.")]
    [InlineData("@on monday @between 1st and 5th @between 10th and 15th", "Between rules is not allowed without Every/Secondly/Minutely/Hourly/Daily/Weekly/Monthly/Yearly rule.")]
    [InlineData("@every 0 days", "Every value cannot be less than or equal to 0.")]
    public void TryParse_InvalidExpressions_ReturnsExpectedError(string expression, string expectedError)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        recurlyEx.Should().BeNull();
        errors.Should().Contain(expectedError);
    }
    
    [Theory]
    [InlineData("@every week @foo bar")]
    public void TryParse_InvalidExpressions_General(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        recurlyEx.Should().BeNull();
        errors.Should().NotBeEmpty();
    }
}