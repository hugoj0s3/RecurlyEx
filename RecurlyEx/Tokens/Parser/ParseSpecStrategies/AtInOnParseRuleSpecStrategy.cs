using RecurlyEx.Rules;
using RecurlyEx.Tokens.Parser.Dtos;

namespace RecurlyEx.Tokens.Parser.ParseSpecStrategies;

internal class AtInOnParseRuleSpecStrategy : ParseRuleSpecStrategy
{
    public override RuleSpecType Type => RuleSpecType.AtInOn;

    public override (IList<RecurlyExRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec)
    {
        var tokens = TokenParserUtil.TrimWhitespaceAndIgnoredTokens(groupedRuleSpec.Tokens);
        if (tokens.Any(x => x.Type == RecurlyExTokenType.OpenBrackets))
        {
            return ParseMultiplesAtInOn(tokens);
        }

        return ParseSingleAtInOn(tokens);
    }

    private static (IList<RecurlyExRule> rules, IList<string> errors) ParseSingleAtInOn(IList<RecurlyExToken> tokens)
    {
        var errors = new List<string>();
        var firstToken = tokens.First();
        
        var timeUnits = TokenParserUtil.ParseTimeUnitValues(tokens.Skip(1).ToList());
        var atInOnRules = new List<RecurlyExRule>();
        foreach (var timeUnit in timeUnits)
        {
            if (timeUnit.TimeUnit == RecurlyExTimeUnitAndUnknown.Unknown)
            {
                var error = string.IsNullOrEmpty(timeUnit.Error)
                    ? "Invalid @at/@In/@On expression. start and end must have known time units" : timeUnit.Error;
                errors.Add(error);
                return (atInOnRules, errors);
            }

            var atRule = new RecurlyExAtInOnRule()
            {
                TimeUnit = (RecurlyExTimeUnit)(int)timeUnit.TimeUnit, 
                InnerExpression = timeUnit.Value.NormalizeExpression(),
                FullExpression = TokenParserUtil.JoinTokens(tokens).NormalizeExpression(),
            };
            atInOnRules.Add(atRule);
        }

        return (atInOnRules, errors);
    }

    private static (IList<RecurlyExRule> rules, IList<string> errors) ParseMultiplesAtInOn(IList<RecurlyExToken> tokens)
    {
        var firstToken = tokens.First();
        var atInOnMultipleValues = new Dictionary<RecurlyExTimeUnitAndUnknown, List<string>>();

        var tokensOutsideBrackets = new List<RecurlyExToken>();
        var tokensInsideBrackets = new List<RecurlyExToken>();
        var isAccumulating = false;

        foreach (var token in tokens.Skip(1))
        {
            if (token.Type == RecurlyExTokenType.OpenBrackets)
            {
                isAccumulating = true;
                continue;
            }

            if (!isAccumulating)
            {
                tokensOutsideBrackets.Add(token);
                continue;
            }

            if (token.Type == RecurlyExTokenType.CloseBrackets)
            {
                isAccumulating = false;
                continue;
            }

            tokensInsideBrackets.Add(token);
        }

        var tokensWithValues = TokenParserUtil.SplitTokens(tokensInsideBrackets, RecurlyExTokenType.Comma);
        var defaultTimeUnitToken = tokensOutsideBrackets.FirstOrDefault(t => t.Type == RecurlyExTokenType.TimeUnit);
        var defaultTimeUnit = RecurlyExTimeUnitAndUnknown.Unknown;
        if (defaultTimeUnitToken != null)
        {
            defaultTimeUnit = TokenParserUtil.GetTimeUnit(defaultTimeUnitToken.Value);
        }
        
        var errors = new List<string>();
        foreach (var tokenWithValues in tokensWithValues)
        {
            var timeUnitValues = TokenParserUtil.ParseTimeUnitValues(tokenWithValues, defaultTimeUnit);
            foreach (var timeUnitValue in timeUnitValues)
            {
                if (atInOnMultipleValues.TryGetValue(timeUnitValue.TimeUnit, out var multipleValues))
                {
                    multipleValues.Add(timeUnitValue.Value);
                }
                else
                {
                    multipleValues = new List<string>() { timeUnitValue.Value };
                    atInOnMultipleValues.Add(timeUnitValue.TimeUnit, multipleValues);
                }
                
                if (!timeUnitValue.IsValid)
                {
                    errors.Add(timeUnitValue.Error);
                }
            }
        }
        
        if (errors.Any())
        {
            var error = string.Join(" ", errors);
            return (new List<RecurlyExRule>(), new List<string>() { error });
        }

        // Valid if the atInOnMultipleValues are equals
        var maxValueLength = atInOnMultipleValues.Values.Max(x => x.Count);
        if (atInOnMultipleValues.Values.Any(x => x.Count != maxValueLength))
        {
            var error = "Invalid @at, @on, @in expression. Multiple values must have the same number of time units";
            return (new List<RecurlyExRule>(), new List<string>() { error });
        }

        if (atInOnMultipleValues.ContainsKey(RecurlyExTimeUnitAndUnknown.Unknown))
        {
            return (new List<RecurlyExRule>(), new List<string>()
            {
                "Invalid @at, @on, @in expression. Multiple values must have known time units"
            });
        }
        
        var timeUnits = new[] {
            RecurlyExTimeUnitAndUnknown.Second,
            RecurlyExTimeUnitAndUnknown.Minute,
            RecurlyExTimeUnitAndUnknown.Hour,
            RecurlyExTimeUnitAndUnknown.Day,
            RecurlyExTimeUnitAndUnknown.Week,
            RecurlyExTimeUnitAndUnknown.Month,
            RecurlyExTimeUnitAndUnknown.Year
        };
        
        var rulesByUnit = new Dictionary<RecurlyExTimeUnitAndUnknown, List<RecurlyExAtInOnRule>>();
        foreach (var timeUnit in timeUnits)
        {
            atInOnMultipleValues.TryGetValue(timeUnit, out var values);
            values ??= new List<string>();

            var rules = values
                .Select(value => new RecurlyExAtInOnRule { TimeUnit = (RecurlyExTimeUnit)timeUnit, InnerExpression = value.NormalizeExpression() })
                .ToList();

            rulesByUnit[timeUnit] = rules;
        }

        RecurlyExRule result = new RecurlyExAtInOnMultiplesRule()
        {
            SecondRules = rulesByUnit[RecurlyExTimeUnitAndUnknown.Second].ToArray(),
            MinuteRules = rulesByUnit[RecurlyExTimeUnitAndUnknown.Minute].ToArray(),
            HourRules = rulesByUnit[RecurlyExTimeUnitAndUnknown.Hour].ToArray(),
            DayRules = rulesByUnit[RecurlyExTimeUnitAndUnknown.Day].ToArray(),
            MonthRules = rulesByUnit[RecurlyExTimeUnitAndUnknown.Month].ToArray(),
            WeekRules = rulesByUnit[RecurlyExTimeUnitAndUnknown.Week].ToArray(),
            YearRules = rulesByUnit[RecurlyExTimeUnitAndUnknown.Year].ToArray(),
            TimeUnit = (RecurlyExTimeUnit)(int) rulesByUnit.First(x => x.Value.Any()).Key,
            FullExpression = TokenParserUtil.JoinTokens(tokens),
        };

        return (new List<RecurlyExRule>() { result }, new List<string>());
    }
}