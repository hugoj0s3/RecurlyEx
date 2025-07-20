using NaturalCron.Rules;
using NaturalCron.Tokens.Parser.Dtos;
using NaturalCron.Utils;

namespace NaturalCron.Tokens.Parser.ParseSpecStrategies;

internal class TimeZoneParseRuleSpecStrategy : ParseRuleSpecStrategy
{
    public override RuleSpecType Type => RuleSpecType.TimeZone;
    public override (IList<NaturalCronRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec)
    {
        var tokens = groupedRuleSpec.Tokens;
        if (tokens.Count < 2)
        {
            return (new List<NaturalCronRule>(), "Invalid tz expression".AsList());
        }

        var tzValue = TokenParserUtil.JoinTokens(tokens.Skip(1).ToList(), NaturalCronTokenType.WhiteSpace, NaturalCronTokenType.EndOfExpression);
        var tz = new NaturalCronIanaTimeZoneRule()
        {
            TimeUnit = NaturalCronTimeUnit.TimeZone, 
            IanaId = tzValue,
            FullExpression = TokenParserUtil.JoinTokens(tokens)
        };
        
        return (tz.AsList<NaturalCronRule>(), new List<string>());
    }
}