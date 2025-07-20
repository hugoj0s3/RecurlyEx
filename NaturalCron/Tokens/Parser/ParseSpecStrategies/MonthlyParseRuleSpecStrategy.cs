using NaturalCron.Rules;
using NaturalCron.Tokens.Parser.Dtos;
using NaturalCron.Utils;

namespace NaturalCron.Tokens.Parser.ParseSpecStrategies;

internal class MonthlyParseRuleSpecStrategy : ParseRuleSpecStrategy
{
    public override RuleSpecType Type => RuleSpecType.Monthly;
    public override (IList<NaturalCronRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec)
    {
        var tokens = TokenParserUtil.RemoveWhitespaceAndIgnoredTokens(groupedRuleSpec.Tokens);
        var anchoredValue = TokenParserUtil.GetAnchoredValue(tokens);
        var monthRule = new NaturalCronEveryXRule()
        {
            TimeUnit = NaturalCronTimeUnit.Month, 
            Value = 1,
            AnchoredValue = anchoredValue,
            FullExpression = TokenParserUtil.JoinTokens(tokens)
        };

        return (monthRule.AsList<NaturalCronRule>(), new List<string>());
    }
}