using NaturalCron.Rules;
using NaturalCron.Tokens.Parser.Dtos;
using NaturalCron.Utils;

namespace NaturalCron.Tokens.Parser.ParseSpecStrategies;

internal class SecondlyParseRuleSpecStrategy : ParseRuleSpecStrategy
{
    public override RuleSpecType Type => RuleSpecType.Secondly;
    public override (IList<NaturalCronRule>, IList<string>) Parse(GroupedRuleSpec groupedRuleSpec)
    {
        var tokens = TokenParserUtil.RemoveWhitespaceAndIgnoredTokens(groupedRuleSpec.Tokens);
        var anchoredValue = TokenParserUtil.GetAnchoredValue(tokens);
        var secondRule = new NaturalCronEveryXRule()
        {
            TimeUnit = NaturalCronTimeUnit.Second, 
            Value = 1,
            AnchoredValue = anchoredValue,
            FullExpression = TokenParserUtil.JoinTokens(tokens)
        };

        return (secondRule.AsList<NaturalCronRule>(), new List<string>());
    }
}