using NaturalCron.Rules;
using NaturalCron.Tokens.Parser.Dtos;
using NaturalCron.Utils;

namespace NaturalCron.Tokens.Parser.ParseSpecStrategies;

internal class BetweenParseRuleSpecStrategy : ParseRuleSpecStrategy
{
    public override RuleSpecType Type => RuleSpecType.Between;
    
    public override (IList<NaturalCronRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec)
    {
        var errors = new List<string>();
        var tokens = TokenParserUtil.TrimWhitespaceAndIgnoredTokens(groupedRuleSpec.Tokens);
        List<NaturalCronRule> rules;
        if (tokens.Any(x => x.Type == NaturalCronTokenType.OpenBrackets))
        {
            rules = ParseMultiplesBetween(tokens, errors)?.AsList().Cast<NaturalCronRule>().ToList() ?? [];
        }
        else
        {
            rules = ParseSingleBetween(tokens, errors)?.AsList().Cast<NaturalCronRule>().ToList() ?? [];
        }
        
        if (errors.Count > 0)
        {
            return (new List<NaturalCronRule>(), errors);
        }
            
        return (rules, errors);
    }

    private NaturalCronBetweenMultiplesRule? ParseMultiplesBetween(IList<NaturalCronToken> tokens, IList<string> errors)
    {
        var firstToken = tokens.First();
        var ranges = ExtractBracketedRanges(tokens); // implement this helper
        if (ranges.Count == 0)
        {
            errors.Add("Invalid between expression");
            return null;
        }

        var betweenRules = new List<NaturalCronBetweenRule>();
        List<NaturalCronTimeUnit>? expectedUnits = null;
        
        foreach (var rangeTokens in ranges)
        {
            rangeTokens.Insert(0, firstToken); // Add Between token.
            
            var rule = ParseSingleBetween(rangeTokens, errors, firstToken);
            if (rule == null)
            {
                errors.Add("Invalid between expression");
                return null;
            }
            
            var units = rule.StartEndValues.Keys.ToList();

            if (expectedUnits == null)
            {
                expectedUnits = units;
            } else if (!units.SequenceEqual(expectedUnits))
            {
                errors.Add("Invalid between expression");
                return null;
            }

            betweenRules.Add(rule);
        }
        
        if (betweenRules.Count == 0)
        {
            errors.Add("Invalid between expression");
            return null;
        }

        return new NaturalCronBetweenMultiplesRule()
        {
            BetweenRules = betweenRules,
            TimeUnit = betweenRules.Select(x => x.TimeUnit).OrderBy(x => (int)x).First(),
            FullExpression = TokenParserUtil.JoinTokens(tokens).NormalizeExpression(),
        };
    }

    private static NaturalCronBetweenRule? ParseSingleBetween(IList<NaturalCronToken> tokens, IList<string> errors, NaturalCronToken? firstToken = null)
    {
        firstToken ??= tokens.First();
        
        if (tokens.Count < 4)
        {
            errors.Add("Invalid between expression");
            return null;
        }

        var andToken = tokens.Skip(2).FirstOrDefault(x => x.Type == NaturalCronTokenType.And);
        if (andToken == null)
        {
            errors.Add("Invalid between expression. must have a 'and' between start and end");
            return null;
        }

        var andIndex = tokens.IndexOf(andToken);
        var startTokens = tokens.Skip(1).Take(andIndex - 1).ToList();
        var endTokens = tokens.Skip(andIndex + 1).ToList();

        var starts = TokenParserUtil.ParseTimeUnitValues(startTokens);
        var defaultUnit = starts.FirstOrDefault(x => x.TimeUnit != NaturalCronTimeUnitAndUnknown.Unknown)?.TimeUnit ?? 
                          NaturalCronTimeUnitAndUnknown.Unknown;
        
        var ends = TokenParserUtil.ParseTimeUnitValues(endTokens, defaultUnit);
        var betweenRule = new NaturalCronBetweenRule()
        {
            FullExpression = TokenParserUtil.JoinTokens(tokens).NormalizeExpression(),
        };
        
        if (starts.Count != ends.Count)
        {
            errors.Add("Invalid between expression. start and end must have the same number of time units");
            return null;
        }

        for (var i = starts.Count - 1; i >= 0; i--)
        {
            var start = starts[i];
            var end = ends[i];
            
            if (start.TimeUnit == NaturalCronTimeUnitAndUnknown.Unknown)
            {
                var error = string.IsNullOrEmpty(start.Error)
                    ? "Invalid between expression. start and end must have known time units" : start.Error;
                errors.Add(error);
                return null;
            }
            
            if (start.TimeUnit != end.TimeUnit)
            {
                errors.Add("Invalid between expression. start and end must have the same time units");
                return null;
            }
            
            var startEndValue = new NaturalCronBetweenStartEndValue()
            {
                StartExpr = start.Value.NormalizeExpression(), 
                EndExpr = end.Value.NormalizeExpression(),
            };
            
            betweenRule.StartEndValues[(NaturalCronTimeUnit)(int)start.TimeUnit] = startEndValue;
        }
        
        betweenRule.TimeUnit = betweenRule.StartEndValues.OrderBy(x => (int)x.Key).First().Key;
        return betweenRule;
    }
    
    private static List<List<NaturalCronToken>> ExtractBracketedRanges(IList<NaturalCronToken> tokens)
    {
        var ranges = new List<List<NaturalCronToken>>();
        var insideBrackets = false;
        var currentRange = new List<NaturalCronToken>();

        foreach (var token in tokens)
        {
            if (token.Type == NaturalCronTokenType.OpenBrackets)
            {
                insideBrackets = true;
                continue;
            }
            if (token.Type == NaturalCronTokenType.CloseBrackets)
            {
                // Add the last range if any tokens were collected
                if (currentRange.Count > 0)
                    ranges.Add(new List<NaturalCronToken>(currentRange));
                break;
            }
            if (insideBrackets)
            {
                if (token.Type == NaturalCronTokenType.Comma)
                {
                    // End of current range, start a new one
                    ranges.Add(new List<NaturalCronToken>(currentRange));
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