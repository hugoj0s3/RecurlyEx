using RecurlyEx.Rules;
using RecurlyEx.Tokens.Parser.Dtos;
using RecurlyEx.Utils;

namespace RecurlyEx.Tokens.Parser.ParseSpecStrategies;

internal class EveryXParseRuleSpecStrategy : ParseRuleSpecStrategy
{

    public override RuleSpecType Type => RuleSpecType.EveryX;
    public override (IList<RecurlyExRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec)
    {
        var tokens = TokenParserUtil.RemoveWhitespaceAndIgnoredTokens(groupedRuleSpec.Tokens);
        if (tokens.Count < 2)
        {
            return (new List<RecurlyExRule>(), "Invalid @every expression".AsList());
        }

        var everyTimeUnit = tokens.Count > 2
            ? ExpressionUtil.TryParseToTimeUnit(tokens[2].Value) ?? ExpressionUtil.TryParseToTimeUnit(tokens[1].Value)
            : ExpressionUtil.TryParseToTimeUnit(tokens[1].Value);
        
        if (everyTimeUnit == null)
        {
            var containsWeekdayWord = KeywordsConstants.WeekdayWords.ToUpper().Contains(tokens[1].Value.ToUpper());
            if (containsWeekdayWord)
            {
                var recurlyExRuleEveryX = new RecurlyExEveryXRule()
                {
                    Value = 1, 
                    AnchoredValue = tokens[1].Value.ToUpper(), 
                    TimeUnit =  RecurlyExTimeUnit.Week,
                    FullExpression = TokenParserUtil.JoinTokens(tokens, " ", RecurlyExTokenType.WhiteSpace).NormalizeExpression()
                };
                return (recurlyExRuleEveryX.AsList<RecurlyExRule>(), new List<string>());
            }
            
            return (new List<RecurlyExRule>(), "Invalid @every expression".AsList());
        }

        // @every 1 day or @every day 
        var value = 1;
        if (tokens.Count > 2 && tokens[2].Type == RecurlyExTokenType.TimeUnit)
        {
            if (!int.TryParse(tokens[1].Value, out value))
            {
                return (new List<RecurlyExRule>(), "Invalid @every expression".AsList());
            }
        }

        var anchoredValue = TokenParserUtil.GetAnchoredValue(tokens);
        var everyXExpression = new RecurlyExEveryXRule()
        {
            Value = value, 
            AnchoredValue = anchoredValue,
            TimeUnit = everyTimeUnit.Value,
            FullExpression = TokenParserUtil.JoinTokens(tokens, " ", RecurlyExTokenType.WhiteSpace).NormalizeExpression()
        };

        return (everyXExpression.AsList<RecurlyExRule>(), new List<string>());
    }
}