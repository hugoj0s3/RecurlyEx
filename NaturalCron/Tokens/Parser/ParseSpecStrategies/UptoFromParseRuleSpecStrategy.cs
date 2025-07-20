using NaturalCron.Rules;
using NaturalCron.Tokens.Parser.Dtos;
using NaturalCron.Utils;

namespace NaturalCron.Tokens.Parser.ParseSpecStrategies;

internal class UptoFromParseRuleSpecStrategy : ParseRuleSpecStrategy
{

    public override RuleSpecType Type => RuleSpecType.UptoOrFrom;
    public override (IList<NaturalCronRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec)
    {
        var tokens = TokenParserUtil.TrimWhitespaceAndIgnoredTokens(groupedRuleSpec.Tokens);
        var firstToken = tokens.First();
        if (tokens.Count < 2)
        {
            return (new List<NaturalCronRule>(), "Invalid upto or from expression".AsList());
        }
        
        var timeUnits = TokenParserUtil.ParseTimeUnitValues(tokens.Skip(1).ToList());
        if (timeUnits.Count < 1)
        { 
           return (new List<NaturalCronRule>(), "Invalid upto or from expression".AsList());
        }
        
        var betweenRule = new NaturalCronBetweenRule()
        {
            FullExpression = TokenParserUtil.JoinTokens(tokens),
        };
        
        var firstTokenLower = firstToken.Value.ToLower();
        var isUpto = firstTokenLower == "@upto" || firstTokenLower == "upto";
        for (var i = timeUnits.Count - 1; i >= 0; i--)
        {
            var timeUnit = timeUnits[i];

            if (timeUnit.TimeUnit == NaturalCronTimeUnitAndUnknown.Unknown)
            {
                var error = string.IsNullOrEmpty(timeUnit.Error)
                    ? "Invalid between expression. start and end must have known time units" : timeUnit.Error;
                return (new List<NaturalCronRule>(), error.AsList());
            }
            
            var startValue = "First";
            var endValue = "Last";
            if (isUpto)
            {
                endValue = timeUnit.Value;
            }
            else
            {
                startValue = timeUnit.Value;
            }

            var startEndValue = new NaturalCronBetweenStartEndValue()
            {
                StartExpr = startValue.NormalizeExpression(),
                EndExpr = endValue.NormalizeExpression(),
            };
            
            betweenRule.StartEndValues[(NaturalCronTimeUnit)(int)timeUnit.TimeUnit] = startEndValue;
        }
        
        betweenRule.TimeUnit = betweenRule.StartEndValues.OrderBy(x => (int)x.Key).First().Key;
        return (betweenRule.AsList().Cast<NaturalCronRule>().ToList(), new List<string>());
    }
}