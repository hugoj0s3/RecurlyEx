using System.Text;
using NaturalCron.Rules;
using NaturalCron.Tokens.Parser.Dtos;
using NaturalCron.Tokens.Parser.ParseSpecStrategies;
using NaturalCron.Utils;

namespace NaturalCron.Tokens.Parser;

internal static class TokenParserUtil
{
    internal static IList<GroupedRuleSpec> GroupSpecs(IList<NaturalCronToken> tokens)
    {
        var groupSpecs = new HashSet<GroupedRuleSpec>();
        GroupedRuleSpec? currentGroupSpec = null;
        foreach (var token in tokens)
        {
            var firstTokenLower = token.Value.ToLower();
            if (firstTokenLower.StartsWith("@"))
            {
                firstTokenLower = firstTokenLower.Substring(1);
            }
            
            switch (firstTokenLower)
            {
                case "yearly":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.Yearly
                    };

                    break;
                case "monthly":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.Monthly
                    };
                    break;
                case "weekly":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.Weekly
                    };
                    break;
                case "daily":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.Daily
                    };
                    break;
                case "hourly":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.Hourly
                    };
                    break;
                case "minutely":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.Minutely
                    };
                    break;
                case "secondly":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.Secondly
                    };
                    break;
                case "every":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.EveryX
                    };
                    break;
                case "between":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.Between
                    };
                    break;
                case "tz":
                case "timezone":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.TimeZone
                    };
                    break;
                case "at":
                case "on":
                case "in":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.AtInOn
                    };
                    break;
                case "upto":
                case "from":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.UptoOrFrom
                    };
                    break;
            }

            if (currentGroupSpec == null)
            {
                continue;
            }

            currentGroupSpec.Tokens.Add(token);
            groupSpecs.Add(currentGroupSpec);
        }

        return groupSpecs.ToList();
    }

    internal static List<NaturalCronToken> Tokenizer(string expression)
    {
        expression += "\0";
        StringBuilder accumulator = new StringBuilder();
        List<NaturalCronToken> tokens = new List<NaturalCronToken>();

        int currentLine = 1;
        int currentColumn = 1;
        char? holdingChar = null;
        for (int i = 0; i < expression.Length; i++)
        {
            var c = expression[i];

            bool shouldBreak = false;
            if (KeywordsConstants.Breaks.Contains(c))
            {
                shouldBreak = true;
            }
            else if (i > 0 && char.IsDigit(c) && !char.IsDigit(expression[i - 1]) )
            {
                holdingChar = c;
                shouldBreak = true;
            }
            else if (i > 0 && !char.IsDigit(c) && char.IsDigit(expression[i - 1]) && !IsNthWeekOrdinal(i, expression))
            {
                holdingChar = c;
                shouldBreak = true;
            }

            if (!shouldBreak)
            {
                accumulator.Append(c);
            }

            if (shouldBreak)
            {
                if (accumulator.Length > 0)
                {
                    NaturalCronTokenType tokenType = RecognizeTokenType(accumulator);
                    var token = new NaturalCronToken()
                    {
                        Type = tokenType,
                        Value = accumulator.ToString(),
                        IsValid = tokenType != NaturalCronTokenType.Unknown,
                        Error = tokenType == NaturalCronTokenType.Unknown ? "Invalid token" : string.Empty,
                        Line = currentLine,
                        StartCol = currentColumn - accumulator.Length,
                        EndCol = currentColumn
                    };
                    tokens.Add(token);
                    accumulator.Clear();
                }

                if (KeywordsConstants.Breaks.Contains(c))
                {
                    var breakType = RecognizeBreak(c);
                    // Ignores double spaces
                    if (breakType == NaturalCronTokenType.WhiteSpace && tokens.Count > 0 &&
                        tokens.Last().Type == NaturalCronTokenType.WhiteSpace)
                    {
                        continue;
                    }

                    var token = new NaturalCronToken()
                    {
                        Type = breakType,
                        Value = c.ToString(),
                        IsValid = breakType != NaturalCronTokenType.Unknown,
                        Error = breakType == NaturalCronTokenType.Unknown ? "Invalid token" : string.Empty,
                        Line = currentLine,
                        StartCol = currentColumn - accumulator.Length,
                        EndCol = currentColumn
                    };

                    if (breakType == NaturalCronTokenType.CloseBrackets)
                    {
                        var hasOpenBrackets = tokens.Any(x => x.Type == NaturalCronTokenType.OpenBrackets);
                        if (!hasOpenBrackets)
                        {
                            token.IsValid = false;
                            token.Error = "Missing open brackets";
                        }
                    }

                    tokens.Add(token);
                }
            }

            if (c == '\n')
            {
                currentLine++;
                currentColumn = 1;
            }
            else
            {
                currentColumn++;
            }

            if (holdingChar != null)
            {
                accumulator.Append(holdingChar.Value);
                holdingChar = null;
            }
        }

        return tokens;
    }
    
    private static bool IsNthWeekOrdinal(int i, string expression)
    {
        int start = i - 1;
        // Move start back to the beginning of the digit sequence
        while (start > 0 && char.IsDigit(expression[start - 1]))
            start--;

        int lookahead = i;
        while (lookahead < expression.Length && char.IsLetter(expression[lookahead]))
            lookahead++;

        var candidate = expression.Substring(start, lookahead - start);

        return KeywordsConstants.NthWeekDays.Any(kw => string.Equals(kw, candidate, StringComparison.OrdinalIgnoreCase));
    }

    private static NaturalCronTokenType RecognizeBreak(char c)
    {
        switch (c)
        {
            case '[':
            {
                return NaturalCronTokenType.OpenBrackets;
            }
            case ']':
            {
                return NaturalCronTokenType.CloseBrackets;
            }
            case ' ':
            {
                return NaturalCronTokenType.WhiteSpace;
            }
            case '-':
            {
                return NaturalCronTokenType.DashOrMinus;
            }
            case '+':
                return NaturalCronTokenType.Plus;
            case ':':
            {
                return NaturalCronTokenType.Colon;
            }
            case ',':
            {
                return NaturalCronTokenType.Comma;
            }
            case '\0':
            {
                return NaturalCronTokenType.EndOfExpression;
            }
            default:
            {
                if (KeywordsConstants.IgnoredChars.Contains(c))
                {
                    return NaturalCronTokenType.IgnoredToken;
                }

                return NaturalCronTokenType.Unknown;
            }
        }
    }

    internal static (IList<NaturalCronRule> rules, IList<string> errors) ParseGroupedSpec(GroupedRuleSpec groupedRuleSpec)
    {
        var tokens = groupedRuleSpec.Tokens;
        if (tokens.First().Type != NaturalCronTokenType.RuleSpec)
        {
            throw new ArgumentException("First token must be Rule Spec");
        }

        var strategy = ParseSpecStrategyFactory.Create(groupedRuleSpec.Type);
        if (strategy == null)
        {
            throw new ArgumentException($"Strategy not found for type {groupedRuleSpec.Type}");
        }

        var result = strategy.Parse(groupedRuleSpec);
        return result;
    }

    internal static bool IsMonthWord(string value) =>
        KeywordsConstants.MonthWords.ToUpper().Any(x => value.ToUpper().ContainsWholeWord(x));

    internal static bool IsWeekdaySpecificWord(string value) =>
        KeywordsConstants.WeekdayWords.ToUpper().Any(x => value.ToUpper().ContainsWholeWord(x));

    internal static bool ContainsNthWeekdayWord(string value) =>
        KeywordsConstants.NthWeekDays.ToUpper().Any(x => value.ToUpper().ContainsWholeWord(x));

    internal static bool ContainsRelativeWeekdays(string value) =>
        KeywordsConstants.RelativeWeekdays.ToUpper().Any(x => value.ToUpper().ContainsWholeWord(x));

    internal static NaturalCronTimeUnitAndUnknown GetTimeUnit(string value)
    {
        if (KeywordsConstants.SecondsTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return NaturalCronTimeUnitAndUnknown.Second;
        }

        if (KeywordsConstants.MinutesTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return NaturalCronTimeUnitAndUnknown.Minute;
        }

        if (KeywordsConstants.HoursTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return NaturalCronTimeUnitAndUnknown.Hour;
        }

        if (KeywordsConstants.DaysTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return NaturalCronTimeUnitAndUnknown.Day;
        }

        if (KeywordsConstants.WeeksTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return NaturalCronTimeUnitAndUnknown.Week;
        }

        if (KeywordsConstants.MonthTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return NaturalCronTimeUnitAndUnknown.Month;
        }

        if (KeywordsConstants.YearsTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return NaturalCronTimeUnitAndUnknown.Year;
        }

        return NaturalCronTimeUnitAndUnknown.Unknown;
    }

    internal static bool ContainsLastDayOfTheMonth(string value)
    {
        return KeywordsConstants.LastDayOfTheMonth.ToUpper().Any(x => value.ToUpper().Contains(x));
    }

    internal static bool ContainsFirstDayOfTheMonth(string value)
    {
        return KeywordsConstants.FirstDayOfTheMonth.ToUpper().Any(x => value.ToUpper().Contains(x));
    }

    internal static bool ContainsMax(string value)
    {
        return KeywordsConstants.Last.ToUpper().Any(x => value.ToUpper().Contains(x));
    }

    private static NaturalCronTokenType RecognizeTokenType(StringBuilder accumulator)
    {
        var tokenValue = accumulator.ToString().ToUpper();
        if (tokenValue.IsWholeNumber())
        {
            return NaturalCronTokenType.WholeNumber;
        }

        if (KeywordsConstants.RuleSpec.ToUpper().Contains(tokenValue))
        {
            return NaturalCronTokenType.RuleSpec;
        }

        if (KeywordsConstants.Anchored.ToUpper().Contains(tokenValue))
        {
            return NaturalCronTokenType.Anchored;
        }

        if (KeywordsConstants.TimeUnitWords.ToUpper().Contains(tokenValue))
        {
            return NaturalCronTokenType.TimeUnit;
        }

        if (KeywordsConstants.MonthWords.ToUpper().Contains(tokenValue))
        {
            return NaturalCronTokenType.MonthWord;
        }

        if (KeywordsConstants.WeekdayWords.ToUpper().Contains(tokenValue))
        {
            return NaturalCronTokenType.WeekdayWord;
        }

        if (KeywordsConstants.IanaTimeZones.ToUpper().Contains(tokenValue))
        {
            return NaturalCronTokenType.IanaTimeZoneId;
        }

        if (KeywordsConstants.LastDayOfTheMonth.ToUpper().Contains(tokenValue))
        {
            return NaturalCronTokenType.LastDayOfTheMonth;
        }

        if (KeywordsConstants.FirstDayOfTheMonth.ToUpper().Contains(tokenValue))
        {
            return NaturalCronTokenType.FirstDayOfTheMonth;
        }

        if (KeywordsConstants.RelativeWeekdays.ToUpper().Contains(tokenValue))
        {
            return NaturalCronTokenType.RelativeWeekdays;
        }

        if (KeywordsConstants.NthWeekDays.ToUpper().Contains(tokenValue))
        {
            return NaturalCronTokenType.NthWeekDays;
        }

        if (KeywordsConstants.Last.ToUpper().Contains(tokenValue))
        {
            return NaturalCronTokenType.Last;
        }

        if (KeywordsConstants.First.ToUpper().Contains(tokenValue))
        {
            return NaturalCronTokenType.First;
        }

        if (KeywordsConstants.PmOrAm.ToUpper().Contains(tokenValue))
        {
            return NaturalCronTokenType.AmPm;
        }

        if (KeywordsConstants.And.ToUpper().Contains(tokenValue))
        {
            return NaturalCronTokenType.And;
        }

        return NaturalCronTokenType.Unknown;
    }

    internal static IList<NaturalCronToken> SkipTokens(IList<NaturalCronToken> tokens, params NaturalCronTokenType[] skipTypes)
    {
        return tokens.Where(x => !skipTypes.Contains(x.Type)).ToList();
    }

    internal static IList<NaturalCronToken> TrimTokens(IList<NaturalCronToken> tokens, params NaturalCronTokenType[] skipTypes)
    {
        // Remove leading whitespace tokens
        while (tokens.Count > 0 && skipTypes.Contains(tokens[0].Type))
        {
            tokens.RemoveAt(0);
        }

        // Remove trailing whitespace tokens
        while (tokens.Count > 0 && skipTypes.Contains(tokens[tokens.Count - 1].Type))
        {
            tokens.RemoveAt(tokens.Count - 1);
        }

        return tokens;
    }

    internal static IList<NaturalCronToken> TrimWhitespaceAndIgnoredTokens(IList<NaturalCronToken> tokens)
    {
        return TrimTokens(tokens, NaturalCronTokenType.WhiteSpace, NaturalCronTokenType.IgnoredToken);
    }

    internal static IList<NaturalCronToken> RemoveWhitespaceAndIgnoredTokens(IList<NaturalCronToken> tokens)
    {
        return SkipTokens(tokens, NaturalCronTokenType.WhiteSpace, NaturalCronTokenType.IgnoredToken);
    }

    internal static string JoinTokens(IList<NaturalCronToken> tokens, params NaturalCronTokenType[] skipTypes)
    {
        return JoinTokens(tokens, string.Empty, skipTypes);
    }

    internal static string JoinTokens(IList<NaturalCronToken> tokens, string separator, params NaturalCronTokenType[] skipTypes)
    {
        return string.Join(separator, tokens
            .Where(x => !skipTypes.Contains(x.Type))
            .Select(x => x.Value));
    }

    internal static IList<IList<NaturalCronToken>> SplitTokens(IList<NaturalCronToken> tokens, params NaturalCronTokenType[] skipTypes)
    {
        IList<IList<NaturalCronToken>> result = new List<IList<NaturalCronToken>>();
        IList<NaturalCronToken> currentList = new List<NaturalCronToken>();
        foreach (var token in tokens)
        {
            if (skipTypes.Contains(token.Type))
            {
                if (currentList.Count > 0)
                {
                    result.Add(currentList);
                    currentList = new List<NaturalCronToken>();
                }
            }
            else
            {
                currentList.Add(token);
            }
        }

        if (currentList.Count > 0)
        {
            result.Add(currentList);
        }

        return result;
    }

    // Parse tokens into time units.
    // e.g 10:30:10 -> [{10, hour}, {30, minute}, {10, second}]
    // e.g 10:30 -> [{10, hour}, {30, minute}]
    // e.g 2025-JAN-25 or 2025-01-25 -> [{2025, year}, {1, month}, {25, day}]
    // e.g JAN-25 or 01-25 -> [{1, month}, {25, day}]
    // e.g JAN -> [{1, month}]
    // e.g 10hr or hr 10 -> [{10, hour}]
    // e.g MON -> [{2, weekday}]
    internal static IList<TimeUnitAndValueDto> ParseTimeUnitValues(
        IList<NaturalCronToken> tokens,
        NaturalCronTimeUnitAndUnknown defaultTimeUnit = NaturalCronTimeUnitAndUnknown.Unknown)
    {
        if (tokens.Any(t => t.Type == NaturalCronTokenType.Colon) &&
            !tokens.Any(t => t.Type == NaturalCronTokenType.DashOrMinus) &&
            !tokens.Any(t => ContainsAnySpecialTimeToken(t)) &&
            !tokens.Any(t => t.IsDayOrdinal()))
        {
            if (HasWhitespaceBetweenTokens(tokens, NaturalCronTokenType.Colon))
            {
                return new List<TimeUnitAndValueDto>()
                {
                    new()
                    {
                        TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                        Value = string.Empty,
                        IsValid = false,
                        Error = "Invalid time format: use HH:MM:SS or HH:MM (no spaces between numbers and colons)."
                    }
                };
            }

            var splitTokens = SplitTokens(tokens, NaturalCronTokenType.WhiteSpace);
            if (splitTokens.Count == 1)
            {
                return ParseColonBasedTimeUnitValues(tokens);
            }

            if (splitTokens.Count == 2)
            {
                var colonBasedTokens = splitTokens[0].Any(x => x.Type == NaturalCronTokenType.Colon) ? splitTokens[0] : splitTokens[1];
                var singleBasedTokens = splitTokens[0].Any(x => x.Type == NaturalCronTokenType.Colon) ? splitTokens[1] : splitTokens[0];
                return ParseColonBasedTimeUnitValues(colonBasedTokens)
                    .Concat(ParseSingleTimeUnitValue(singleBasedTokens, NaturalCronTimeUnitAndUnknown.Day).AsList())
                    .ToList();
            }

            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "Invalid format time format. for time uses HH:MM:SS or HH:MM for date uses YYYY-MM-DD or MM-DD"
                }
            };
        }

        if (tokens.Any(t => t.Type == NaturalCronTokenType.DashOrMinus)
            && !tokens.Any(t => t.Type == NaturalCronTokenType.Colon)
            && !tokens.Any(t => ContainsAnySpecialTimeToken(t))
            && !tokens.Any(t => t.IsDayOrdinal()))
        {
            if (HasWhitespaceBetweenTokens(tokens, NaturalCronTokenType.DashOrMinus))
            {
                return new List<TimeUnitAndValueDto>()
                {
                    new()
                    {
                        TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                        Value = string.Empty,
                        IsValid = false,
                        Error =
                            "Invalid date format: use YYYY-MM-DD (e.g., 2025-07-04), MM-DD (e.g., 07-04), MMM-DD (e.g., Jul-04), etc. (no spaces between numbers and dashes)."
                    }
                };
            }

            var splitTokens = SplitTokens(tokens, NaturalCronTokenType.WhiteSpace);
            if (splitTokens.Count == 1)
            {
                return ParseDashBasedTimeUnitValues(tokens);
            }

            if (splitTokens.Count == 2)
            {
                var dashBasedTokens = splitTokens[0].Any(x => x.Type == NaturalCronTokenType.DashOrMinus)
                    ? splitTokens[0]
                    : splitTokens[1];
                var singleBasedTokens = splitTokens[0].Any(x => x.Type == NaturalCronTokenType.DashOrMinus)
                    ? splitTokens[1]
                    : splitTokens[0];
                return ParseDashBasedTimeUnitValues(dashBasedTokens)
                    .Concat(ParseSingleTimeUnitValue(singleBasedTokens, NaturalCronTimeUnitAndUnknown.Hour).AsList())
                    .ToList();
            }

            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "Invalid format time format. for time uses HH:MM:SS or HH:MM for date uses YYYY-MM-DD or MM-DD"
                }
            };
        }

        if (tokens.Any(t => t.Type == NaturalCronTokenType.DashOrMinus) &&
            tokens.Any(t => t.Type == NaturalCronTokenType.Colon) &&
            !tokens.Any(t => ContainsAnySpecialTimeToken(t)) &&
            !tokens.Any(t => t.IsDayOrdinal()))
        {
            if (HasWhitespaceBetweenTokens(tokens, NaturalCronTokenType.DashOrMinus))
            {
                return new List<TimeUnitAndValueDto>()
                {
                    new()
                    {
                        TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                        Value = string.Empty,
                        IsValid = false,
                        Error =
                            "Invalid date format: use YYYY-MM-DD (e.g., 2025-07-04), MM-DD (e.g., 07-04), MMM-DD (e.g., Jul-04), etc. (no spaces between numbers and dashes)."
                    }
                };
            }

            if (HasWhitespaceBetweenTokens(tokens, NaturalCronTokenType.Colon))
            {
                return new List<TimeUnitAndValueDto>()
                {
                    new()
                    {
                        TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                        Value = string.Empty,
                        IsValid = false,
                        Error = "Invalid time format: use HH:MM:SS or HH:MM (no spaces between numbers and colons)."
                    }
                };
            }


            var splitTokens = SplitTokens(tokens, NaturalCronTokenType.WhiteSpace);
            if (splitTokens.Count != 2)
            {
                return new List<TimeUnitAndValueDto>()
                {
                    new()
                    {
                        TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                        Value = string.Empty,
                        IsValid = false,
                        Error = "Invalid format time format. for time uses HH:MM:SS or HH:MM for date uses YYYY-MM-DD or MM-DD"
                    }
                };
            }

            var dashBasedTokens = splitTokens[0].Any(x => x.Type == NaturalCronTokenType.DashOrMinus) ? splitTokens[0] : splitTokens[1];
            var colonBasedTokens = splitTokens[0].Any(x => x.Type == NaturalCronTokenType.Colon) ? splitTokens[0] : splitTokens[1];

            return ParseDashBasedTimeUnitValues(dashBasedTokens)
                .Concat(ParseColonBasedTimeUnitValues(colonBasedTokens)).ToList();
        }

        if (tokens.Any(t => t.IsDayOrdinal()) &&
            !tokens.Any(t => t.Type == NaturalCronTokenType.Colon) &&
            !tokens.Any(t => t.Type == NaturalCronTokenType.DashOrMinus) &&
            !tokens.Any(t => ContainsAnySpecialTimeToken(t)))
        {
            var splitTokens = SplitTokens(tokens, NaturalCronTokenType.WhiteSpace);
            var list = new List<TimeUnitAndValueDto>();
            if (splitTokens.Count > 1)
            {
                if (splitTokens.Count == 2)
                {
                    list.Add(ParseSingleTimeUnitValue(splitTokens[0]));
                    list.Add(ParseSingleTimeUnitValue(splitTokens[1], NaturalCronTimeUnitAndUnknown.Month));
                }
                else if (splitTokens.Count == 3)
                {
                    list.Add(ParseSingleTimeUnitValue(splitTokens[0]));
                    list.Add(ParseSingleTimeUnitValue(splitTokens[1], NaturalCronTimeUnitAndUnknown.Month));
                    list.Add(ParseSingleTimeUnitValue(splitTokens[2], NaturalCronTimeUnitAndUnknown.Year));
                }
                else
                {
                    list.Add(new TimeUnitAndValueDto
                    {
                        TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                        Value = string.Empty,
                        IsValid = false,
                        Error = "Invalid ordinal date format."
                    });
                }

                return list;
            }
        }

        if (tokens.Any(t => t.IsDayOrdinal()) &&
            tokens.Any(t => t.Type == NaturalCronTokenType.Colon) &&
            !tokens.Any(t => t.Type == NaturalCronTokenType.DashOrMinus) &&
            !tokens.Any(t => ContainsAnySpecialTimeToken(t)))
        {
            if (HasWhitespaceBetweenTokens(tokens, NaturalCronTokenType.Colon))
            {
                return new List<TimeUnitAndValueDto>()
                {
                    new()
                    {
                        TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                        Value = string.Empty,
                        IsValid = false,
                        Error = "Invalid time format: use HH:MM:SS or HH:MM (no spaces between numbers and colons)."
                    }
                };
            }

            var splitTokens = SplitTokens(tokens, NaturalCronTokenType.WhiteSpace);
            var dateTokens = new List<IList<NaturalCronToken>>();
            var timeTokens = new List<NaturalCronToken>();

            foreach (var splitTokenItem in splitTokens)
            {
                if (splitTokenItem.Any(t => t.Type == NaturalCronTokenType.Colon)
                    || splitTokenItem.Any(t => t.Type == NaturalCronTokenType.AmPm))
                {
                    timeTokens.AddRange(splitTokenItem);
                }
                else
                {
                    dateTokens.Add(splitTokenItem);
                }
            }

            var list = new List<TimeUnitAndValueDto>();
            if (dateTokens.Count == 2)
            {
                list.Add(ParseSingleTimeUnitValue(dateTokens[0]));
                list.Add(ParseSingleTimeUnitValue(dateTokens[1], NaturalCronTimeUnitAndUnknown.Month));
            }
            else if (dateTokens.Count == 3)
            {
                list.Add(ParseSingleTimeUnitValue(dateTokens[0]));
                list.Add(ParseSingleTimeUnitValue(dateTokens[1], NaturalCronTimeUnitAndUnknown.Month));
                list.Add(ParseSingleTimeUnitValue(dateTokens[2], NaturalCronTimeUnitAndUnknown.Year));
            }
            else
            {
                // Fallback for invalid input
                list.Add(new TimeUnitAndValueDto
                {
                    TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "Invalid ordinal date+time format."
                });
            }
            list.AddRange(ParseColonBasedTimeUnitValues(timeTokens));
            return list;
        }

        return ParseSingleTimeUnitValue(tokens, defaultTimeUnit).AsList();
    }

    private static bool ContainsAnySpecialTimeToken(NaturalCronToken t)
    {
        return t.Type == NaturalCronTokenType.Last ||
               t.Type == NaturalCronTokenType.First ||
               t.Type == NaturalCronTokenType.WeekdayWord ||
               t.Type == NaturalCronTokenType.NthWeekDays ||
               t.Type == NaturalCronTokenType.RelativeWeekdays ||
               t.Type == NaturalCronTokenType.LastDayOfTheMonth ||
               t.Type == NaturalCronTokenType.FirstDayOfTheMonth;
    }

    private static bool HasWhitespaceBetweenTokens(
        IList<NaturalCronToken> tokens,
        NaturalCronTokenType separatorType)
    {
        var nonWhiteSpaceAndSeparator = new List<NaturalCronTokenType>
        {
            NaturalCronTokenType.WhiteSpace, separatorType
        };

        for (int i = 0; i < tokens.Count - 2; i++)
        {
            if (!nonWhiteSpaceAndSeparator.Contains(tokens[i].Type) &&
                tokens[i + 1].Type == NaturalCronTokenType.WhiteSpace &&
                tokens[i + 2].Type == separatorType)
            {
                return true;
            }

            if (tokens[i].Type == separatorType &&
                tokens[i + 1].Type == NaturalCronTokenType.WhiteSpace &&
                !nonWhiteSpaceAndSeparator.Contains(tokens[i + 2].Type))
            {
                return true;
            }
        }
        return false;
    }

    internal static TimeUnitAndValueDto ParseSingleTimeUnitValue(
        IList<NaturalCronToken> tokens,
        NaturalCronTimeUnitAndUnknown defaultTimeUnit = NaturalCronTimeUnitAndUnknown.Unknown)
    {
        var timeUnitToken = tokens.FirstOrDefault(x => x.Type == NaturalCronTokenType.TimeUnit);

        var tokensWithoutTimeUnit =
            tokens.Where(x => x.Type != NaturalCronTokenType.TimeUnit && x.Type != NaturalCronTokenType.EndOfExpression).ToList();

        var invalidTokens = false;
        var dupesMathOperation = false;

        var value = JoinTokens(tokens,
            NaturalCronTokenType.EndOfExpression,
            NaturalCronTokenType.TimeUnit);

        if (TokenParserUtil.IsWeekdaySpecificWord(value))
        {
            return new()
            {
                TimeUnit = NaturalCronTimeUnitAndUnknown.Week, Value = value.Trim(), IsValid = true, Error = string.Empty
            };
        }

        if (TokenParserUtil.IsMonthWord(value))
        {
            return
                new()
                {
                    TimeUnit = NaturalCronTimeUnitAndUnknown.Month, Value = value.Trim(), IsValid = true, Error = string.Empty
                };
        }

        if (ContainsLastDayOfTheMonth(value) ||
            ContainsNthWeekdayWord(value) ||
            ContainsRelativeWeekdays(value) ||
            ContainsFirstDayOfTheMonth(value))
        {
            invalidTokens = tokensWithoutTimeUnit.Any(x =>
                x.Type != NaturalCronTokenType.DashOrMinus &&
                x.Type != NaturalCronTokenType.Plus &&
                x.Type != NaturalCronTokenType.WhiteSpace &&
                x.Type != NaturalCronTokenType.WholeNumber &&
                x.Type != NaturalCronTokenType.RelativeWeekdays &&
                x.Type != NaturalCronTokenType.LastDayOfTheMonth &&
                x.Type != NaturalCronTokenType.FirstDayOfTheMonth &&
                x.Type != NaturalCronTokenType.NthWeekDays &&
                x.Type != NaturalCronTokenType.First);

            dupesMathOperation = tokensWithoutTimeUnit.Count(x =>
                x.Type == NaturalCronTokenType.DashOrMinus &&
                x.Type == NaturalCronTokenType.Plus) > 1;

            if (invalidTokens || dupesMathOperation)
            {
                return new()
                {
                    TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown, Value = value.Trim(), IsValid = false, Error = "invalid expression"
                };
            }

            return new()
            {
                TimeUnit = NaturalCronTimeUnitAndUnknown.Day, Value = value.Trim(), IsValid = true, Error = string.Empty
            };
        }

        if (timeUnitToken == null)
        {
            return new()
            {
                TimeUnit = defaultTimeUnit,
                Value = value.Trim(),
                IsValid = defaultTimeUnit != NaturalCronTimeUnitAndUnknown.Unknown,
                Error = defaultTimeUnit == NaturalCronTimeUnitAndUnknown.Unknown ? "invalid time unit" : string.Empty
            };
        }

        var timeUnit = GetTimeUnit(timeUnitToken.Value);
        if (timeUnit == NaturalCronTimeUnitAndUnknown.Unknown)
        {
            timeUnit = defaultTimeUnit;
        }

        invalidTokens = tokensWithoutTimeUnit.Any(x =>
            x.Type != NaturalCronTokenType.WhiteSpace &&
            x.Type != NaturalCronTokenType.WholeNumber &&
            x.Type != NaturalCronTokenType.Last &&
            x.Type != NaturalCronTokenType.First &&
            x.Type != NaturalCronTokenType.LastDayOfTheMonth &&
            x.Type != NaturalCronTokenType.FirstDayOfTheMonth &&
            x.Type != NaturalCronTokenType.DashOrMinus &&
            x.Type != NaturalCronTokenType.Plus);

        dupesMathOperation = tokensWithoutTimeUnit.Count(x =>
            x.Type == NaturalCronTokenType.DashOrMinus ||
            x.Type == NaturalCronTokenType.Plus) > 1;

        if (invalidTokens || dupesMathOperation)
        {
            return new()
            {
                TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown, Value = value.Trim(), IsValid = false, Error = "invalid expression"
            };
        }

        return new()
        {
            TimeUnit = timeUnit,
            Value = value.Trim(),
            IsValid = timeUnit != NaturalCronTimeUnitAndUnknown.Unknown,
            Error = timeUnit == NaturalCronTimeUnitAndUnknown.Unknown ? "invalid time unit" : string.Empty
        };
    }

    internal static IList<TimeUnitAndValueDto> ParseDashBasedTimeUnitValues(IList<NaturalCronToken> tokens)
    {
        var splitTokens = SplitTokens(tokens, NaturalCronTokenType.DashOrMinus);
        string year = string.Empty;
        string month = string.Empty;
        string day = string.Empty;

        var dashCount = tokens.Count(x => x.Type == NaturalCronTokenType.DashOrMinus);
        if (dashCount > 2)
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = NaturalCronTimeUnitAndUnknown.Year,
                    Value = year.Trim(),
                    IsValid = true,
                    Error = $"Invalid format yyyy-MM or yyyy-MM-dd or MM-dd"
                }
            };
        }

        if (tokens.Where(x => x.Type == NaturalCronTokenType.TimeUnit)
            .Any(x => KeywordsConstants.SecondsTimeUnitWords
                .ToUpper()
                .Contains(x.Value.ToUpper())))
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "can not use sec time units with dashes"
                }
            };
        }

        if (tokens.Where(x => x.Type == NaturalCronTokenType.TimeUnit)
            .Any(x => KeywordsConstants.MinutesTimeUnitWords
                .ToUpper()
                .Contains(x.Value.ToUpper())))
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "can not use min time units with dashes"
                }
            };
        }

        if (tokens.Where(x => x.Type == NaturalCronTokenType.TimeUnit)
            .Any(x => KeywordsConstants.HoursTimeUnitWords
                .ToUpper()
                .Contains(x.Value.ToUpper())))
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "can not use hr time units with dashes"
                }
            };
        }

        var tokens1 = splitTokens[0].Where(x => x.Type == NaturalCronTokenType.WholeNumber ||
                                                x.Type == NaturalCronTokenType.Last ||
                                                x.Type == NaturalCronTokenType.MonthWord).ToList();

        var tokens2 = splitTokens[1].Where(x => x.Type == NaturalCronTokenType.WholeNumber ||
                                                x.Type == NaturalCronTokenType.MonthWord ||
                                                x.Type == NaturalCronTokenType.Last).ToList();
        var tokens3 = splitTokens.Count > 2
            ? splitTokens[2].Where(x => x.Type == NaturalCronTokenType.WholeNumber ||
                                        x.Type == NaturalCronTokenType.LastDayOfTheMonth ||
                                        x.Type == NaturalCronTokenType.Last).ToList()
            : new List<NaturalCronToken>();
        if (tokens3.Count > 0)
        {
            year = JoinTokens(tokens1, NaturalCronTokenType.WhiteSpace, NaturalCronTokenType.EndOfExpression);
            month = JoinTokens(tokens2, NaturalCronTokenType.WhiteSpace, NaturalCronTokenType.EndOfExpression);
            day = JoinTokens(tokens3, NaturalCronTokenType.WhiteSpace, NaturalCronTokenType.EndOfExpression);
        }
        else
        {
            var yearOrMonth = JoinTokens(tokens1, NaturalCronTokenType.WhiteSpace, NaturalCronTokenType.EndOfExpression);
            var dayOrMonth = JoinTokens(tokens2, NaturalCronTokenType.WhiteSpace, NaturalCronTokenType.EndOfExpression);
            if (yearOrMonth.Length == 4 && yearOrMonth.IsWholeNumber())
            {
                year = yearOrMonth;
                month = dayOrMonth;
            }
            else
            {
                month = yearOrMonth;
                day = dayOrMonth;
            }
        }

        if (!string.IsNullOrEmpty(year) && (year.Length != 4 || !year.IsWholeNumber()))
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                    IsValid = false,
                    Error = $"Invalid format yyyy-MM or yyyy-MM-dd or MM-dd"
                }
            };
        }

        if (!string.IsNullOrEmpty(month) && month.Length != 2 &&
            !TokenParserUtil.IsMonthWord(month) &&
            !TokenParserUtil.ContainsMax(day))
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                    IsValid = false,
                    Error = $"Invalid format yyyy-MM or yyyy-MM-dd or MM-dd"
                }
            };
        }

        if (!string.IsNullOrEmpty(day) &&
            day.Length != 2 &&
            !TokenParserUtil.ContainsLastDayOfTheMonth(day) &&
            !TokenParserUtil.ContainsMax(day))
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                    IsValid = false,
                    Error = $"Invalid format yyyy-MM or yyyy-MM-dd or MM-dd"
                }
            };
        }

        var result = new List<TimeUnitAndValueDto>();
        if (!string.IsNullOrEmpty(year))
        {
            result.Add(new TimeUnitAndValueDto()
            {
                TimeUnit = NaturalCronTimeUnitAndUnknown.Year, Value = year.Trim(), IsValid = true
            });
        }

        if (!string.IsNullOrEmpty(month))
        {
            result.Add(new TimeUnitAndValueDto()
            {
                TimeUnit = NaturalCronTimeUnitAndUnknown.Month, Value = month.Trim(), IsValid = true
            });
        }

        if (!string.IsNullOrEmpty(day))
        {
            result.Add(new TimeUnitAndValueDto()
            {
                TimeUnit = NaturalCronTimeUnitAndUnknown.Day, Value = day.Trim(), IsValid = true
            });
        }

        return result;
    }

    internal static IList<TimeUnitAndValueDto> ParseColonBasedTimeUnitValues(IList<NaturalCronToken> tokens)
    {
        var countColons = tokens.Count(x => x.Type == NaturalCronTokenType.Colon);
        if (countColons > 2)
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "Invalid format it should be HH:MM:SS or HH:MM"
                }
            };
        }

        if (tokens.Where(x => x.Type == NaturalCronTokenType.TimeUnit)
            .Any(x => KeywordsConstants.DaysTimeUnitWords
                .ToUpper()
                .Contains(x.Value.ToUpper())))
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "can not use days time units with colon"
                }
            };
        }

        if (tokens.Where(x => x.Type == NaturalCronTokenType.TimeUnit)
            .Any(x => KeywordsConstants.WeeksTimeUnitWords
                .ToUpper()
                .Contains(x.Value.ToUpper())))
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "can not use week time units with colon"
                }
            };
        }

        if (tokens.Where(x => x.Type == NaturalCronTokenType.TimeUnit)
            .Any(x => KeywordsConstants.MonthTimeUnitWords
                .ToUpper()
                .Contains(x.Value.ToUpper())))
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "can not use month time units with colon"
                }
            };
        }

        if (tokens.Where(x => x.Type == NaturalCronTokenType.TimeUnit)
            .Any(x => KeywordsConstants.YearsTimeUnitWords
                .ToUpper()
                .Contains(x.Value.ToUpper())))
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "can not use year time units with colon"
                }
            };
        }


        var splitTokens = SplitTokens(tokens, NaturalCronTokenType.Colon);
        var hourTokens = splitTokens[0].Where(x => x.Type == NaturalCronTokenType.WholeNumber ||
                                                   x.Type == NaturalCronTokenType.Last).ToList();

        var hourValue = JoinTokens(hourTokens, NaturalCronTokenType.EndOfExpression);

        var hasAmOrPm = tokens.Any(x => x.Type == NaturalCronTokenType.AmPm);
        if (hasAmOrPm && hourValue.IsWholeNumber())
        {
            var amOrPmTokens = tokens.Where(x => x.Type == NaturalCronTokenType.AmPm).ToList();
            var pmOrAm = amOrPmTokens.LastOrDefault();
            if (amOrPmTokens.Count > 1 || pmOrAm?.Type != NaturalCronTokenType.AmPm)
            {
                return new List<TimeUnitAndValueDto>()
                {
                    new()
                    {
                        TimeUnit = NaturalCronTimeUnitAndUnknown.Unknown,
                        Value = string.Empty,
                        IsValid = false,
                        Error = "Invalid format it should be HH:MM:SS AM/PM or HH:MM AM/PM"
                    }
                };
            }

            var hourInt = int.Parse(hourValue);
            if (pmOrAm.Value.ToUpper() == "PM" && hourInt != 12)
            {
                hourInt += 12; // Convert PM hours to 24-hour format
            }
            else if (pmOrAm.Value.ToUpper() == "AM" && hourInt == 12)
            {
                hourInt = 0; // Convert 12 AM to 0 hours
            }

            hourValue = hourInt.ToString();
        }

        var minuteTokens = splitTokens[1]
            .Where(x => x.Type == NaturalCronTokenType.WholeNumber ||
                        x.Type == NaturalCronTokenType.Last)
            .ToList();
        var secondTokens = splitTokens.Count > 2 ? splitTokens[2] : new List<NaturalCronToken>();
        secondTokens = secondTokens.Where(x => x.Type == NaturalCronTokenType.WholeNumber ||
                                               x.Type == NaturalCronTokenType.Last).ToList();

        var result = new List<TimeUnitAndValueDto>()
        {
            new()
            {
                TimeUnit = NaturalCronTimeUnitAndUnknown.Hour, Value = hourValue, IsValid = true
            },
            new()
            {
                TimeUnit = NaturalCronTimeUnitAndUnknown.Minute,
                Value = JoinTokens(minuteTokens, NaturalCronTokenType.EndOfExpression),
                IsValid = true
            },
        };

        if (secondTokens.Count > 0)
        {
            result.Add(new TimeUnitAndValueDto()
            {
                TimeUnit = NaturalCronTimeUnitAndUnknown.Second,
                Value = JoinTokens(secondTokens, NaturalCronTokenType.EndOfExpression),
                IsValid = true
            });
        }

        return result;
    }

    internal static string? GetAnchoredValue(IList<NaturalCronToken> tokens)
    {
        var anchoredIndex = -1;
        if (tokens.Any(x => x.Type == NaturalCronTokenType.Anchored))
        {
            anchoredIndex = tokens.IndexOf(tokens.First(x => x.Type == NaturalCronTokenType.Anchored));
        }

        string? anchoredValue = null;
        if (anchoredIndex != -1)
        {
            anchoredValue = JoinTokens(tokens.Skip(anchoredIndex + 1).ToList(), skipTypes:
            [
                NaturalCronTokenType.WhiteSpace,
                NaturalCronTokenType.IgnoredToken,
                NaturalCronTokenType.EndOfExpression,
                NaturalCronTokenType.Comma,
                NaturalCronTokenType.TimeUnit,
            ]);
        }
        return anchoredValue;
    }
}