using RecurlyEx.Rules;
using RecurlyEx.Tokens.Parser.Dtos;
using RecurlyEx.Utils;

namespace RecurlyEx.Tokens.Parser.ParseSpecStrategies;

internal class BetweenParseRuleSpecStrategy : ParseRuleSpecStrategy
{
    public override RuleSpecType Type => RuleSpecType.Between;
    
    public override (IList<RecurlyExRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec)
    {
        var errors = new List<string>();
        var tokens = TokenParserUtil.TrimWhitespaceAndIgnoredTokens(groupedRuleSpec.Tokens);
        List<RecurlyExRule> rules;
        if (tokens.Any(x => x.Type == RecurlyExTokenType.OpenBrackets))
        {
            rules = ParseMultiplesBetween(tokens, errors)?.AsList().Cast<RecurlyExRule>().ToList() ?? [];
        }
        else
        {
            rules = ParseSingleBetween(tokens, errors)?.AsList().Cast<RecurlyExRule>().ToList() ?? [];
        }
        
        if (errors.Count > 0)
        {
            return (new List<RecurlyExRule>(), errors);
        }
            
        return (rules, errors);
    }

    private RecurlyExBetweenMultiplesRule? ParseMultiplesBetween(IList<RecurlyExToken> tokens, IList<string> errors)
    {
        var firstToken = tokens.First();
        var ranges = ExtractBracketedRanges(tokens); // implement this helper
        if (ranges.Count == 0)
        {
            errors.Add("Invalid @between expression");
            return null;
        }

        var betweenRules = new List<RecurlyExBetweenRule>();
        List<RecurlyExTimeUnit>? expectedUnits = null;
        
        foreach (var rangeTokens in ranges)
        {
            rangeTokens.Insert(0, firstToken); // Add Between token.
            
            var rule = ParseSingleBetween(rangeTokens, errors, firstToken);
            if (rule == null)
            {
                errors.Add("Invalid @between expression");
                return null;
            }
            
            var units = rule.StartEndValues.Keys.ToList();

            if (expectedUnits == null)
            {
                expectedUnits = units;
            } else if (!units.SequenceEqual(expectedUnits))
            {
                errors.Add("Invalid @between expression");
                return null;
            }

            betweenRules.Add(rule);
        }
        
        if (betweenRules.Count == 0)
        {
            errors.Add("Invalid @between expression");
            return null;
        }

        return new RecurlyExBetweenMultiplesRule()
        {
            BetweenRules = betweenRules,
            TimeUnit = betweenRules.Select(x => x.TimeUnit).OrderBy(x => (int)x).First(),
            FullExpression = TokenParserUtil.JoinTokens(tokens).NormalizeExpression(),
        };
    }

    private static RecurlyExBetweenRule? ParseSingleBetween(IList<RecurlyExToken> tokens, IList<string> errors, RecurlyExToken? firstToken = null)
    {
        firstToken ??= tokens.First();
        
        if (tokens.Count < 4)
        {
            errors.Add("Invalid @between expression");
            return null;
        }

        var andToken = tokens.Skip(2).FirstOrDefault(x => x.Type == RecurlyExTokenType.And);
        if (andToken == null)
        {
            errors.Add("Invalid @between expression. must have a 'and' between start and end");
            return null;
        }

        var andIndex = tokens.IndexOf(andToken);
        var startTokens = tokens.Skip(1).Take(andIndex - 1).ToList();
        var endTokens = tokens.Skip(andIndex + 1).ToList();

        var starts = TokenParserUtil.ParseTimeUnitValues(startTokens);
        var defaultUnit = starts.FirstOrDefault(x => x.TimeUnit != RecurlyExTimeUnitAndUnknown.Unknown)?.TimeUnit ?? 
                          RecurlyExTimeUnitAndUnknown.Unknown;
        
        var ends = TokenParserUtil.ParseTimeUnitValues(endTokens, defaultUnit);
        var betweenRule = new RecurlyExBetweenRule()
        {
            FullExpression = TokenParserUtil.JoinTokens(tokens).NormalizeExpression(),
        };
        
        if (starts.Count != ends.Count)
        {
            errors.Add("Invalid @between expression. start and end must have the same number of time units");
            return null;
        }

        for (var i = starts.Count - 1; i >= 0; i--)
        {
            var start = starts[i];
            var end = ends[i];
            
            if (start.TimeUnit == RecurlyExTimeUnitAndUnknown.Unknown)
            {
                var error = string.IsNullOrEmpty(start.Error)
                    ? "Invalid @between expression. start and end must have known time units" : start.Error;
                errors.Add(error);
                return null;
            }
            
            if (start.TimeUnit != end.TimeUnit)
            {
                errors.Add("Invalid @between expression. start and end must have the same time units");
                return null;
            }
            
            var startEndValue = new RecurlyExBetweenStartEndValue()
            {
                StartExpr = start.Value.NormalizeExpression(), 
                EndExpr = end.Value.NormalizeExpression(),
            };
            
            betweenRule.StartEndValues[(RecurlyExTimeUnit)(int)start.TimeUnit] = startEndValue;
        }
        
        betweenRule.TimeUnit = betweenRule.StartEndValues.OrderBy(x => (int)x.Key).First().Key;
        return betweenRule;
    }
    
    private static List<List<RecurlyExToken>> ExtractBracketedRanges(IList<RecurlyExToken> tokens)
    {
        var ranges = new List<List<RecurlyExToken>>();
        var insideBrackets = false;
        var currentRange = new List<RecurlyExToken>();

        foreach (var token in tokens)
        {
            if (token.Type == RecurlyExTokenType.OpenBrackets)
            {
                insideBrackets = true;
                continue;
            }
            if (token.Type == RecurlyExTokenType.CloseBrackets)
            {
                // Add the last range if any tokens were collected
                if (currentRange.Count > 0)
                    ranges.Add(new List<RecurlyExToken>(currentRange));
                break;
            }
            if (insideBrackets)
            {
                if (token.Type == RecurlyExTokenType.Comma)
                {
                    // End of current range, start a new one
                    ranges.Add(new List<RecurlyExToken>(currentRange));
                    currentRange.Clear();
                }
                else
                {
                    currentRange.Add(token);
                }
            }
        }

        return ranges;
    }
}