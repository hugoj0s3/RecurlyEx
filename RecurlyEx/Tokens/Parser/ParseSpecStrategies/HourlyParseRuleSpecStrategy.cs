using RecurlyEx.Rules;
using RecurlyEx.Tokens.Parser.Dtos;
using RecurlyEx.Utils;

namespace RecurlyEx.Tokens.Parser.ParseSpecStrategies;

internal class HourlyParseRuleSpecStrategy : ParseRuleSpecStrategy
{
    public override RuleSpecType Type => RuleSpecType.Hourly;
    public override (IList<RecurlyExRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec)
    {
        var tokens = TokenParserUtil.RemoveWhitespaceAndIgnoredTokens(groupedRuleSpec.Tokens);
        var anchoredValue = TokenParserUtil.GetAnchoredValue(tokens);
        var hourRule = new RecurlyExEveryXRule()
        {
            TimeUnit = RecurlyExTimeUnit.Hour, 
            Value = 1, 
            AnchoredValue = anchoredValue,
            FullExpression = TokenParserUtil.JoinTokens(tokens)
        };

        return (hourRule.AsList<RecurlyExRule>(), new List<string>());
    }
}