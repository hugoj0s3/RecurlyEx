using NaturalCron.Rules;
using NaturalCron.Tokens.Parser.Dtos;
using NaturalCron.Utils;

namespace NaturalCron.Tokens.Parser.ParseSpecStrategies;

internal class MinutelyParseRuleSpecStrategy : ParseRuleSpecStrategy
{
    public override RuleSpecType Type => RuleSpecType.Minutely;
    public override (IList<NaturalCronRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec)
    {

        var tokens = TokenParserUtil.RemoveWhitespaceAndIgnoredTokens(groupedRuleSpec.Tokens);
        var anchoredValue = TokenParserUtil.GetAnchoredValue(tokens);
        var minuteRule = new NaturalCronEveryXRule()
        {
            TimeUnit = NaturalCronTimeUnit.Minute, 
            Value = 1,
            AnchoredValue = anchoredValue,
            FullExpression = TokenParserUtil.JoinTokens(tokens)
        };

        return (minuteRule.AsList<NaturalCronRule>(), new List<string>());
    }
}