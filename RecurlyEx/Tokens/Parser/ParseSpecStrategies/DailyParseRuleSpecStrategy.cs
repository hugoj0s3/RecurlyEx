using RecurlyEx.Rules;
using RecurlyEx.Tokens.Parser.Dtos;
using RecurlyEx.Utils;

namespace RecurlyEx.Tokens.Parser.ParseSpecStrategies;

internal class DailyParseRuleSpecStrategy : ParseRuleSpecStrategy
{
    public override RuleSpecType Type => RuleSpecType.Daily;
    public override (IList<RecurlyExRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec)
    {
        var tokens = TokenParserUtil.RemoveWhitespaceAndIgnoredTokens(groupedRuleSpec.Tokens);
        var anchoredValue = TokenParserUtil.GetAnchoredValue(tokens);
        var dayRule = new RecurlyExEveryXRule()
        {
            TimeUnit = RecurlyExTimeUnit.Day,
            AnchoredValue = anchoredValue,
            Value = 1,
            FullExpression = TokenParserUtil.JoinTokens(tokens)
        };

        return (dayRule.AsList<RecurlyExRule>(), new List<string>());
    }
}