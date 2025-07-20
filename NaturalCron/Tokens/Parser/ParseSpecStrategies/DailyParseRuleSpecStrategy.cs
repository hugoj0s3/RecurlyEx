using NaturalCron.Rules;
using NaturalCron.Tokens.Parser.Dtos;
using NaturalCron.Utils;

namespace NaturalCron.Tokens.Parser.ParseSpecStrategies;

internal class DailyParseRuleSpecStrategy : ParseRuleSpecStrategy
{
    public override RuleSpecType Type => RuleSpecType.Daily;
    public override (IList<NaturalCronRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec)
    {
        var tokens = TokenParserUtil.RemoveWhitespaceAndIgnoredTokens(groupedRuleSpec.Tokens);
        var anchoredValue = TokenParserUtil.GetAnchoredValue(tokens);
        var dayRule = new NaturalCronEveryXRule()
        {
            TimeUnit = NaturalCronTimeUnit.Day,
            AnchoredValue = anchoredValue,
            Value = 1,
            FullExpression = TokenParserUtil.JoinTokens(tokens)
        };

        return (dayRule.AsList<NaturalCronRule>(), new List<string>());
    }
}