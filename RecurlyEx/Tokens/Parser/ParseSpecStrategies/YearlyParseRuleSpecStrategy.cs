using RecurlyEx.Rules;
using RecurlyEx.Tokens.Parser.Dtos;
using RecurlyEx.Utils;

namespace RecurlyEx.Tokens.Parser.ParseSpecStrategies;

internal class YearlyParseRuleSpecStrategy : ParseRuleSpecStrategy
{
    public override RuleSpecType Type { get; }
    public override (IList<RecurlyExRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec)
    {
        var tokens = TokenParserUtil.RemoveWhitespaceAndIgnoredTokens(groupedRuleSpec.Tokens);
        var anchoredValue = TokenParserUtil.GetAnchoredValue(tokens);
        var yearRule = new RecurlyExEveryXRule()
        {
            TimeUnit = RecurlyExTimeUnit.Year, 
            AnchoredValue = anchoredValue,
            Value = 1,
            FullExpression = TokenParserUtil.JoinTokens(tokens),
        };

        return (yearRule.AsList<RecurlyExRule>(), new List<string>());
    }
}