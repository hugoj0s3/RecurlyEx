using NaturalCron.Rules;
using NaturalCron.Tokens.Parser.Dtos;
using NaturalCron.Utils;

namespace NaturalCron.Tokens.Parser.ParseSpecStrategies;

internal class HourlyParseRuleSpecStrategy : ParseRuleSpecStrategy
{
    public override RuleSpecType Type => RuleSpecType.Hourly;
    public override (IList<NaturalCronRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec)
    {
        var tokens = TokenParserUtil.RemoveWhitespaceAndIgnoredTokens(groupedRuleSpec.Tokens);
        var anchoredValue = TokenParserUtil.GetAnchoredValue(tokens);
        var hourRule = new NaturalCronEveryXRule()
        {
            TimeUnit = NaturalCronTimeUnit.Hour, 
            Value = 1, 
            AnchoredValue = anchoredValue,
            FullExpression = TokenParserUtil.JoinTokens(tokens)
        };

        return (hourRule.AsList<NaturalCronRule>(), new List<string>());
    }
}