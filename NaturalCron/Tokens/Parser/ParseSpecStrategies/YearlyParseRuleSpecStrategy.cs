using NaturalCron.Rules;
using NaturalCron.Tokens.Parser.Dtos;
using NaturalCron.Utils;

namespace NaturalCron.Tokens.Parser.ParseSpecStrategies;

internal class YearlyParseRuleSpecStrategy : ParseRuleSpecStrategy
{
    public override RuleSpecType Type { get; }
    public override (IList<NaturalCronRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec)
    {
        var tokens = TokenParserUtil.RemoveWhitespaceAndIgnoredTokens(groupedRuleSpec.Tokens);
        var anchoredValue = TokenParserUtil.GetAnchoredValue(tokens);
        var yearRule = new NaturalCronEveryXRule()
        {
            TimeUnit = NaturalCronTimeUnit.Year, 
            AnchoredValue = anchoredValue,
            Value = 1,
            FullExpression = TokenParserUtil.JoinTokens(tokens),
        };

        return (yearRule.AsList<NaturalCronRule>(), new List<string>());
    }
}