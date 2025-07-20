using NaturalCron.Rules;
using NaturalCron.Tokens.Parser.Dtos;
using NaturalCron.Utils;

namespace NaturalCron.Tokens.Parser.ParseSpecStrategies;

internal class WeeklyParseRuleSpecStrategy : ParseRuleSpecStrategy
{
    public override RuleSpecType Type => RuleSpecType.Weekly;
    public override (IList<NaturalCronRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec)
    {
        var tokens = TokenParserUtil.RemoveWhitespaceAndIgnoredTokens(groupedRuleSpec.Tokens);
        var anchoredValue = TokenParserUtil.GetAnchoredValue(tokens);
        
        var weekRule = new NaturalCronEveryXRule()
        {
            TimeUnit = NaturalCronTimeUnit.Week, 
            Value = 1,
            AnchoredValue = anchoredValue,
            FullExpression = TokenParserUtil.JoinTokens(tokens)
        };

        return (weekRule.AsList<NaturalCronRule>(), new List<string>());
    }
}