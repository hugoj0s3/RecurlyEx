using RecurlyEx.Rules;
using RecurlyEx.Tokens.Parser.Dtos;
using RecurlyEx.Utils;

namespace RecurlyEx.Tokens.Parser.ParseSpecStrategies;

internal class TimeZoneParseRuleSpecStrategy : ParseRuleSpecStrategy
{
    public override RuleSpecType Type => RuleSpecType.TimeZone;
    public override (IList<RecurlyExRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec)
    {
        var tokens = groupedRuleSpec.Tokens;
        if (tokens.Count < 2)
        {
            return (new List<RecurlyExRule>(), "Invalid @tz expression".AsList());
        }

        var tzValue = TokenParserUtil.JoinTokens(tokens.Skip(1).ToList(), RecurlyExTokenType.WhiteSpace, RecurlyExTokenType.EndOfExpression);
        var tz = new RecurlyExIanaTimeZoneRule()
        {
            TimeUnit = RecurlyExTimeUnit.TimeZone, 
            IanaId = tzValue,
            FullExpression = TokenParserUtil.JoinTokens(tokens)
        };
        
        return (tz.AsList<RecurlyExRule>(), new List<string>());
    }
}