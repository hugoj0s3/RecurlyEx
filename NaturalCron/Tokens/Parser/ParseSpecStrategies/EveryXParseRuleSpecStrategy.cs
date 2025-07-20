using NaturalCron.Rules;
using NaturalCron.Tokens.Parser.Dtos;
using NaturalCron.Utils;

namespace NaturalCron.Tokens.Parser.ParseSpecStrategies;

internal class EveryXParseRuleSpecStrategy : ParseRuleSpecStrategy
{

    public override RuleSpecType Type => RuleSpecType.EveryX;
    public override (IList<NaturalCronRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec)
    {
        var tokens = TokenParserUtil.RemoveWhitespaceAndIgnoredTokens(groupedRuleSpec.Tokens);
        if (tokens.Count < 2)
        {
            return (new List<NaturalCronRule>(), "Invalid every expression".AsList());
        }

        var everyTimeUnit = tokens.Count > 2
            ? ExpressionUtil.TryParseToTimeUnit(tokens[2].Value) ?? ExpressionUtil.TryParseToTimeUnit(tokens[1].Value)
            : ExpressionUtil.TryParseToTimeUnit(tokens[1].Value);
        
        if (everyTimeUnit == null)
        {
            var containsWeekdayWord = KeywordsConstants.WeekdayWords.ToUpper().Contains(tokens[1].Value.ToUpper());
            if (containsWeekdayWord)
            {
                var naturalCronEveryXRule = new NaturalCronEveryXRule()
                {
                    Value = 1, 
                    AnchoredValue = tokens[1].Value.ToUpper(), 
                    TimeUnit =  NaturalCronTimeUnit.Week,
                    FullExpression = TokenParserUtil.JoinTokens(tokens, " ", NaturalCronTokenType.WhiteSpace).NormalizeExpression()
                };
                return (naturalCronEveryXRule.AsList<NaturalCronRule>(), new List<string>());
            }
            
            return (new List<NaturalCronRule>(), "Invalid every expression".AsList());
        }

        // every 1 day or every day 
        var value = 1;
        if (tokens.Count > 2 && tokens[2].Type == NaturalCronTokenType.TimeUnit)
        {
            if (!int.TryParse(tokens[1].Value, out value))
            {
                return (new List<NaturalCronRule>(), "Invalid every expression".AsList());
            }
        }

        var anchoredValue = TokenParserUtil.GetAnchoredValue(tokens);
        var everyXExpression = new NaturalCronEveryXRule()
        {
            Value = value, 
            AnchoredValue = anchoredValue,
            TimeUnit = everyTimeUnit.Value,
            FullExpression = TokenParserUtil.JoinTokens(tokens, " ", NaturalCronTokenType.WhiteSpace).NormalizeExpression()
        };

        return (everyXExpression.AsList<NaturalCronRule>(), new List<string>());
    }
}