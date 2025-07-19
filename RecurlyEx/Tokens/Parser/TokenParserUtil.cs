using System.Text;
using RecurlyEx.Rules;
using RecurlyEx.Tokens.Parser.Dtos;
using RecurlyEx.Tokens.Parser.ParseSpecStrategies;
using RecurlyEx.Utils;

namespace RecurlyEx.Tokens.Parser;

internal static class TokenParserUtil
{
    internal static IList<GroupedRuleSpec> GroupSpecs(IList<RecurlyExToken> tokens)
    {
        var groupSpecs = new HashSet<GroupedRuleSpec>();
        GroupedRuleSpec? currentGroupSpec = null;
        foreach (var token in tokens)
        {
            switch (token.Value.ToLower())
            {
                case "@yearly":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.Yearly
                    };

                    break;
                case "@monthly":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.Monthly
                    };
                    break;
                case "@weekly":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.Weekly
                    };
                    break;
                case "@daily":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.Daily
                    };
                    break;
                case "@hourly":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.Hourly
                    };
                    break;
                case "@minutely":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.Minutely
                    };
                    break;
                case "@secondly":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.Secondly
                    };
                    break;
                case "@every":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.EveryX
                    };
                    break;
                case "@between":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.Between
                    };
                    break;
                case "@tz":
                case "@timezone":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.TimeZone
                    };
                    break;
                case "@at":
                case "@on":
                case "@in":
                    currentGroupSpec = new GroupedRuleSpec()
                    {
                        Type = RuleSpecType.AtInOn
                    };
                    break;
                case "@upto":
                case "@from":
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

    internal static List<RecurlyExToken> Tokenizer(string expression)
    {
        expression += "\0";
        StringBuilder accumulator = new StringBuilder();
        List<RecurlyExToken> tokens = new List<RecurlyExToken>();

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
                    RecurlyExTokenType tokenType = RecognizeTokenType(accumulator);
                    var token = new RecurlyExToken()
                    {
                        Type = tokenType,
                        Value = accumulator.ToString(),
                        IsValid = tokenType != RecurlyExTokenType.Unknown,
                        Error = tokenType == RecurlyExTokenType.Unknown ? "Invalid token" : string.Empty,
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
                    if (breakType == RecurlyExTokenType.WhiteSpace && tokens.Count > 0 &&
                        tokens.Last().Type == RecurlyExTokenType.WhiteSpace)
                    {
                        continue;
                    }

                    var token = new RecurlyExToken()
                    {
                        Type = breakType,
                        Value = c.ToString(),
                        IsValid = breakType != RecurlyExTokenType.Unknown,
                        Error = breakType == RecurlyExTokenType.Unknown ? "Invalid token" : string.Empty,
                        Line = currentLine,
                        StartCol = currentColumn - accumulator.Length,
                        EndCol = currentColumn
                    };

                    if (breakType == RecurlyExTokenType.CloseBrackets)
                    {
                        var hasOpenBrackets = tokens.Any(x => x.Type == RecurlyExTokenType.OpenBrackets);
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

    private static RecurlyExTokenType RecognizeBreak(char c)
    {
        switch (c)
        {
            case '[':
            {
                return RecurlyExTokenType.OpenBrackets;
            }
            case ']':
            {
                return RecurlyExTokenType.CloseBrackets;
            }
            case ' ':
            {
                return RecurlyExTokenType.WhiteSpace;
            }
            case '-':
            {
                return RecurlyExTokenType.DashOrMinus;
            }
            case '+':
                return RecurlyExTokenType.Plus;
            case ':':
            {
                return RecurlyExTokenType.Colon;
            }
            case ',':
            {
                return RecurlyExTokenType.Comma;
            }
            case '\0':
            {
                return RecurlyExTokenType.EndOfExpression;
            }
            default:
            {
                if (KeywordsConstants.IgnoredChars.Contains(c))
                {
                    return RecurlyExTokenType.IgnoredToken;
                }

                return RecurlyExTokenType.Unknown;
            }
        }
    }

    internal static (IList<RecurlyExRule> rules, IList<string> errors) ParseGroupedSpec(GroupedRuleSpec groupedRuleSpec)
    {
        var tokens = groupedRuleSpec.Tokens;
        if (tokens.First().Type != RecurlyExTokenType.RuleSpec)
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

    internal static RecurlyExTimeUnitAndUnknown GetTimeUnit(string value)
    {
        if (KeywordsConstants.SecondsTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return RecurlyExTimeUnitAndUnknown.Second;
        }

        if (KeywordsConstants.MinutesTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return RecurlyExTimeUnitAndUnknown.Minute;
        }

        if (KeywordsConstants.HoursTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return RecurlyExTimeUnitAndUnknown.Hour;
        }

        if (KeywordsConstants.DaysTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return RecurlyExTimeUnitAndUnknown.Day;
        }

        if (KeywordsConstants.WeeksTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return RecurlyExTimeUnitAndUnknown.Week;
        }

        if (KeywordsConstants.MonthTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return RecurlyExTimeUnitAndUnknown.Month;
        }

        if (KeywordsConstants.YearsTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return RecurlyExTimeUnitAndUnknown.Year;
        }

        return RecurlyExTimeUnitAndUnknown.Unknown;
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

    private static RecurlyExTokenType RecognizeTokenType(StringBuilder accumulator)
    {
        var tokenValue = accumulator.ToString().ToUpper();
        if (tokenValue.IsWholeNumber())
        {
            return RecurlyExTokenType.WholeNumber;
        }

        if (KeywordsConstants.RuleSpec.ToUpper().Contains(tokenValue))
        {
            return RecurlyExTokenType.RuleSpec;
        }

        if (KeywordsConstants.Anchored.ToUpper().Contains(tokenValue))
        {
            return RecurlyExTokenType.Anchored;
        }

        if (KeywordsConstants.TimeUnitWords.ToUpper().Contains(tokenValue))
        {
            return RecurlyExTokenType.TimeUnit;
        }

        if (KeywordsConstants.MonthWords.ToUpper().Contains(tokenValue))
        {
            return RecurlyExTokenType.MonthWord;
        }

        if (KeywordsConstants.WeekdayWords.ToUpper().Contains(tokenValue))
        {
            return RecurlyExTokenType.WeekdayWord;
        }

        if (KeywordsConstants.IanaTimeZones.ToUpper().Contains(tokenValue))
        {
            return RecurlyExTokenType.IanaTimeZoneId;
        }

        if (KeywordsConstants.LastDayOfTheMonth.ToUpper().Contains(tokenValue))
        {
            return RecurlyExTokenType.LastDayOfTheMonth;
        }

        if (KeywordsConstants.FirstDayOfTheMonth.ToUpper().Contains(tokenValue))
        {
            return RecurlyExTokenType.FirstDayOfTheMonth;
        }

        if (KeywordsConstants.RelativeWeekdays.ToUpper().Contains(tokenValue))
        {
            return RecurlyExTokenType.RelativeWeekdays;
        }

        if (KeywordsConstants.NthWeekDays.ToUpper().Contains(tokenValue))
        {
            return RecurlyExTokenType.NthWeekDays;
        }

        if (KeywordsConstants.Last.ToUpper().Contains(tokenValue))
        {
            return RecurlyExTokenType.Last;
        }

        if (KeywordsConstants.First.ToUpper().Contains(tokenValue))
        {
            return RecurlyExTokenType.First;
        }

        if (KeywordsConstants.PmOrAm.ToUpper().Contains(tokenValue))
        {
            return RecurlyExTokenType.AmPm;
        }

        if (KeywordsConstants.And.ToUpper().Contains(tokenValue))
        {
            return RecurlyExTokenType.And;
        }

        return RecurlyExTokenType.Unknown;
    }

    internal static IList<RecurlyExToken> SkipTokens(IList<RecurlyExToken> tokens, params RecurlyExTokenType[] skipTypes)
    {
        return tokens.Where(x => !skipTypes.Contains(x.Type)).ToList();
    }

    internal static IList<RecurlyExToken> TrimTokens(IList<RecurlyExToken> tokens, params RecurlyExTokenType[] skipTypes)
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

    internal static IList<RecurlyExToken> TrimWhitespaceAndIgnoredTokens(IList<RecurlyExToken> tokens)
    {
        return TrimTokens(tokens, RecurlyExTokenType.WhiteSpace, RecurlyExTokenType.IgnoredToken);
    }

    internal static IList<RecurlyExToken> RemoveWhitespaceAndIgnoredTokens(IList<RecurlyExToken> tokens)
    {
        return SkipTokens(tokens, RecurlyExTokenType.WhiteSpace, RecurlyExTokenType.IgnoredToken);
    }

    internal static string JoinTokens(IList<RecurlyExToken> tokens, params RecurlyExTokenType[] skipTypes)
    {
        return JoinTokens(tokens, string.Empty, skipTypes);
    }

    internal static string JoinTokens(IList<RecurlyExToken> tokens, string separator, params RecurlyExTokenType[] skipTypes)
    {
        return string.Join(separator, tokens
            .Where(x => !skipTypes.Contains(x.Type))
            .Select(x => x.Value));
    }

    internal static IList<IList<RecurlyExToken>> SplitTokens(IList<RecurlyExToken> tokens, params RecurlyExTokenType[] skipTypes)
    {
        IList<IList<RecurlyExToken>> result = new List<IList<RecurlyExToken>>();
        IList<RecurlyExToken> currentList = new List<RecurlyExToken>();
        foreach (var token in tokens)
        {
            if (skipTypes.Contains(token.Type))
            {
                if (currentList.Count > 0)
                {
                    result.Add(currentList);
                    currentList = new List<RecurlyExToken>();
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
        IList<RecurlyExToken> tokens,
        RecurlyExTimeUnitAndUnknown defaultTimeUnit = RecurlyExTimeUnitAndUnknown.Unknown)
    {
        if (tokens.Any(t => t.Type == RecurlyExTokenType.Colon) &&
            !tokens.Any(t => t.Type == RecurlyExTokenType.DashOrMinus) &&
            !tokens.Any(t => ContainsAnySpecialTimeToken(t)) &&
            !tokens.Any(t => t.IsDayOrdinal()))
        {
            if (HasWhitespaceBetweenTokens(tokens, RecurlyExTokenType.Colon))
            {
                return new List<TimeUnitAndValueDto>()
                {
                    new()
                    {
                        TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
                        Value = string.Empty,
                        IsValid = false,
                        Error = "Invalid time format: use HH:MM:SS or HH:MM (no spaces between numbers and colons)."
                    }
                };
            }

            var splitTokens = SplitTokens(tokens, RecurlyExTokenType.WhiteSpace);
            if (splitTokens.Count == 1)
            {
                return ParseColonBasedTimeUnitValues(tokens);
            }

            if (splitTokens.Count == 2)
            {
                var colonBasedTokens = splitTokens[0].Any(x => x.Type == RecurlyExTokenType.Colon) ? splitTokens[0] : splitTokens[1];
                var singleBasedTokens = splitTokens[0].Any(x => x.Type == RecurlyExTokenType.Colon) ? splitTokens[1] : splitTokens[0];
                return ParseColonBasedTimeUnitValues(colonBasedTokens)
                    .Concat(ParseSingleTimeUnitValue(singleBasedTokens, RecurlyExTimeUnitAndUnknown.Day).AsList())
                    .ToList();
            }

            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "Invalid format time format. for time uses HH:MM:SS or HH:MM for date uses YYYY-MM-DD or MM-DD"
                }
            };
        }

        if (tokens.Any(t => t.Type == RecurlyExTokenType.DashOrMinus)
            && !tokens.Any(t => t.Type == RecurlyExTokenType.Colon)
            && !tokens.Any(t => ContainsAnySpecialTimeToken(t))
            && !tokens.Any(t => t.IsDayOrdinal()))
        {
            if (HasWhitespaceBetweenTokens(tokens, RecurlyExTokenType.DashOrMinus))
            {
                return new List<TimeUnitAndValueDto>()
                {
                    new()
                    {
                        TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
                        Value = string.Empty,
                        IsValid = false,
                        Error =
                            "Invalid date format: use YYYY-MM-DD (e.g., 2025-07-04), MM-DD (e.g., 07-04), MMM-DD (e.g., Jul-04), etc. (no spaces between numbers and dashes)."
                    }
                };
            }

            var splitTokens = SplitTokens(tokens, RecurlyExTokenType.WhiteSpace);
            if (splitTokens.Count == 1)
            {
                return ParseDashBasedTimeUnitValues(tokens);
            }

            if (splitTokens.Count == 2)
            {
                var dashBasedTokens = splitTokens[0].Any(x => x.Type == RecurlyExTokenType.DashOrMinus)
                    ? splitTokens[0]
                    : splitTokens[1];
                var singleBasedTokens = splitTokens[0].Any(x => x.Type == RecurlyExTokenType.DashOrMinus)
                    ? splitTokens[1]
                    : splitTokens[0];
                return ParseDashBasedTimeUnitValues(dashBasedTokens)
                    .Concat(ParseSingleTimeUnitValue(singleBasedTokens, RecurlyExTimeUnitAndUnknown.Hour).AsList())
                    .ToList();
            }

            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "Invalid format time format. for time uses HH:MM:SS or HH:MM for date uses YYYY-MM-DD or MM-DD"
                }
            };
        }

        if (tokens.Any(t => t.Type == RecurlyExTokenType.DashOrMinus) &&
            tokens.Any(t => t.Type == RecurlyExTokenType.Colon) &&
            !tokens.Any(t => ContainsAnySpecialTimeToken(t)) &&
            !tokens.Any(t => t.IsDayOrdinal()))
        {
            if (HasWhitespaceBetweenTokens(tokens, RecurlyExTokenType.DashOrMinus))
            {
                return new List<TimeUnitAndValueDto>()
                {
                    new()
                    {
                        TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
                        Value = string.Empty,
                        IsValid = false,
                        Error =
                            "Invalid date format: use YYYY-MM-DD (e.g., 2025-07-04), MM-DD (e.g., 07-04), MMM-DD (e.g., Jul-04), etc. (no spaces between numbers and dashes)."
                    }
                };
            }

            if (HasWhitespaceBetweenTokens(tokens, RecurlyExTokenType.Colon))
            {
                return new List<TimeUnitAndValueDto>()
                {
                    new()
                    {
                        TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
                        Value = string.Empty,
                        IsValid = false,
                        Error = "Invalid time format: use HH:MM:SS or HH:MM (no spaces between numbers and colons)."
                    }
                };
            }


            var splitTokens = SplitTokens(tokens, RecurlyExTokenType.WhiteSpace);
            if (splitTokens.Count != 2)
            {
                return new List<TimeUnitAndValueDto>()
                {
                    new()
                    {
                        TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
                        Value = string.Empty,
                        IsValid = false,
                        Error = "Invalid format time format. for time uses HH:MM:SS or HH:MM for date uses YYYY-MM-DD or MM-DD"
                    }
                };
            }

            var dashBasedTokens = splitTokens[0].Any(x => x.Type == RecurlyExTokenType.DashOrMinus) ? splitTokens[0] : splitTokens[1];
            var colonBasedTokens = splitTokens[0].Any(x => x.Type == RecurlyExTokenType.Colon) ? splitTokens[0] : splitTokens[1];

            return ParseDashBasedTimeUnitValues(dashBasedTokens)
                .Concat(ParseColonBasedTimeUnitValues(colonBasedTokens)).ToList();
        }

        if (tokens.Any(t => t.IsDayOrdinal()) &&
            !tokens.Any(t => t.Type == RecurlyExTokenType.Colon) &&
            !tokens.Any(t => t.Type == RecurlyExTokenType.DashOrMinus) &&
            !tokens.Any(t => ContainsAnySpecialTimeToken(t)))
        {
            var splitTokens = SplitTokens(tokens, RecurlyExTokenType.WhiteSpace);
            var list = new List<TimeUnitAndValueDto>();
            if (splitTokens.Count > 1)
            {
                if (splitTokens.Count == 2)
                {
                    list.Add(ParseSingleTimeUnitValue(splitTokens[0]));
                    list.Add(ParseSingleTimeUnitValue(splitTokens[1], RecurlyExTimeUnitAndUnknown.Month));
                }
                else if (splitTokens.Count == 3)
                {
                    list.Add(ParseSingleTimeUnitValue(splitTokens[0]));
                    list.Add(ParseSingleTimeUnitValue(splitTokens[1], RecurlyExTimeUnitAndUnknown.Month));
                    list.Add(ParseSingleTimeUnitValue(splitTokens[2], RecurlyExTimeUnitAndUnknown.Year));
                }
                else
                {
                    list.Add(new TimeUnitAndValueDto
                    {
                        TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
                        Value = string.Empty,
                        IsValid = false,
                        Error = "Invalid ordinal date format."
                    });
                }

                return list;
            }
        }

        if (tokens.Any(t => t.IsDayOrdinal()) &&
            tokens.Any(t => t.Type == RecurlyExTokenType.Colon) &&
            !tokens.Any(t => t.Type == RecurlyExTokenType.DashOrMinus) &&
            !tokens.Any(t => ContainsAnySpecialTimeToken(t)))
        {
            if (HasWhitespaceBetweenTokens(tokens, RecurlyExTokenType.Colon))
            {
                return new List<TimeUnitAndValueDto>()
                {
                    new()
                    {
                        TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
                        Value = string.Empty,
                        IsValid = false,
                        Error = "Invalid time format: use HH:MM:SS or HH:MM (no spaces between numbers and colons)."
                    }
                };
            }

            var splitTokens = SplitTokens(tokens, RecurlyExTokenType.WhiteSpace);
            var dateTokens = new List<IList<RecurlyExToken>>();
            var timeTokens = new List<RecurlyExToken>();

            foreach (var splitTokenItem in splitTokens)
            {
                if (splitTokenItem.Any(t => t.Type == RecurlyExTokenType.Colon)
                    || splitTokenItem.Any(t => t.Type == RecurlyExTokenType.AmPm))
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
                list.Add(ParseSingleTimeUnitValue(dateTokens[1], RecurlyExTimeUnitAndUnknown.Month));
            }
            else if (dateTokens.Count == 3)
            {
                list.Add(ParseSingleTimeUnitValue(dateTokens[0]));
                list.Add(ParseSingleTimeUnitValue(dateTokens[1], RecurlyExTimeUnitAndUnknown.Month));
                list.Add(ParseSingleTimeUnitValue(dateTokens[2], RecurlyExTimeUnitAndUnknown.Year));
            }
            else
            {
                // Fallback for invalid input
                list.Add(new TimeUnitAndValueDto
                {
                    TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
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

    private static bool ContainsAnySpecialTimeToken(RecurlyExToken t)
    {
        return t.Type == RecurlyExTokenType.Last ||
               t.Type == RecurlyExTokenType.First ||
               t.Type == RecurlyExTokenType.WeekdayWord ||
               t.Type == RecurlyExTokenType.NthWeekDays ||
               t.Type == RecurlyExTokenType.RelativeWeekdays ||
               t.Type == RecurlyExTokenType.LastDayOfTheMonth ||
               t.Type == RecurlyExTokenType.FirstDayOfTheMonth;
    }

    private static bool HasWhitespaceBetweenTokens(
        IList<RecurlyExToken> tokens,
        RecurlyExTokenType separatorType)
    {
        var nonWhiteSpaceAndSeparator = new List<RecurlyExTokenType>
        {
            RecurlyExTokenType.WhiteSpace, separatorType
        };

        for (int i = 0; i < tokens.Count - 2; i++)
        {
            if (!nonWhiteSpaceAndSeparator.Contains(tokens[i].Type) &&
                tokens[i + 1].Type == RecurlyExTokenType.WhiteSpace &&
                tokens[i + 2].Type == separatorType)
            {
                return true;
            }

            if (tokens[i].Type == separatorType &&
                tokens[i + 1].Type == RecurlyExTokenType.WhiteSpace &&
                !nonWhiteSpaceAndSeparator.Contains(tokens[i + 2].Type))
            {
                return true;
            }
        }
        return false;
    }

    internal static TimeUnitAndValueDto ParseSingleTimeUnitValue(
        IList<RecurlyExToken> tokens,
        RecurlyExTimeUnitAndUnknown defaultTimeUnit = RecurlyExTimeUnitAndUnknown.Unknown)
    {
        var timeUnitToken = tokens.FirstOrDefault(x => x.Type == RecurlyExTokenType.TimeUnit);

        var tokensWithoutTimeUnit =
            tokens.Where(x => x.Type != RecurlyExTokenType.TimeUnit && x.Type != RecurlyExTokenType.EndOfExpression).ToList();

        var invalidTokens = false;
        var dupesMathOperation = false;

        var value = JoinTokens(tokens,
            RecurlyExTokenType.EndOfExpression,
            RecurlyExTokenType.TimeUnit);

        if (TokenParserUtil.IsWeekdaySpecificWord(value))
        {
            return new()
            {
                TimeUnit = RecurlyExTimeUnitAndUnknown.Week, Value = value.Trim(), IsValid = true, Error = string.Empty
            };
        }

        if (TokenParserUtil.IsMonthWord(value))
        {
            return
                new()
                {
                    TimeUnit = RecurlyExTimeUnitAndUnknown.Month, Value = value.Trim(), IsValid = true, Error = string.Empty
                };
        }

        if (ContainsLastDayOfTheMonth(value) ||
            ContainsNthWeekdayWord(value) ||
            ContainsRelativeWeekdays(value) ||
            ContainsFirstDayOfTheMonth(value))
        {
            invalidTokens = tokensWithoutTimeUnit.Any(x =>
                x.Type != RecurlyExTokenType.DashOrMinus &&
                x.Type != RecurlyExTokenType.Plus &&
                x.Type != RecurlyExTokenType.WhiteSpace &&
                x.Type != RecurlyExTokenType.WholeNumber &&
                x.Type != RecurlyExTokenType.RelativeWeekdays &&
                x.Type != RecurlyExTokenType.LastDayOfTheMonth &&
                x.Type != RecurlyExTokenType.FirstDayOfTheMonth &&
                x.Type != RecurlyExTokenType.NthWeekDays &&
                x.Type != RecurlyExTokenType.First);

            dupesMathOperation = tokensWithoutTimeUnit.Count(x =>
                x.Type == RecurlyExTokenType.DashOrMinus &&
                x.Type == RecurlyExTokenType.Plus) > 1;

            if (invalidTokens || dupesMathOperation)
            {
                return new()
                {
                    TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown, Value = value.Trim(), IsValid = false, Error = "invalid expression"
                };
            }

            return new()
            {
                TimeUnit = RecurlyExTimeUnitAndUnknown.Day, Value = value.Trim(), IsValid = true, Error = string.Empty
            };
        }

        if (timeUnitToken == null)
        {
            return new()
            {
                TimeUnit = defaultTimeUnit,
                Value = value.Trim(),
                IsValid = defaultTimeUnit != RecurlyExTimeUnitAndUnknown.Unknown,
                Error = defaultTimeUnit == RecurlyExTimeUnitAndUnknown.Unknown ? "invalid time unit" : string.Empty
            };
        }

        var timeUnit = GetTimeUnit(timeUnitToken.Value);
        if (timeUnit == RecurlyExTimeUnitAndUnknown.Unknown)
        {
            timeUnit = defaultTimeUnit;
        }

        invalidTokens = tokensWithoutTimeUnit.Any(x =>
            x.Type != RecurlyExTokenType.WhiteSpace &&
            x.Type != RecurlyExTokenType.WholeNumber &&
            x.Type != RecurlyExTokenType.Last &&
            x.Type != RecurlyExTokenType.First &&
            x.Type != RecurlyExTokenType.LastDayOfTheMonth &&
            x.Type != RecurlyExTokenType.FirstDayOfTheMonth &&
            x.Type != RecurlyExTokenType.DashOrMinus &&
            x.Type != RecurlyExTokenType.Plus);

        dupesMathOperation = tokensWithoutTimeUnit.Count(x =>
            x.Type == RecurlyExTokenType.DashOrMinus ||
            x.Type == RecurlyExTokenType.Plus) > 1;

        if (invalidTokens || dupesMathOperation)
        {
            return new()
            {
                TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown, Value = value.Trim(), IsValid = false, Error = "invalid expression"
            };
        }

        return new()
        {
            TimeUnit = timeUnit,
            Value = value.Trim(),
            IsValid = timeUnit != RecurlyExTimeUnitAndUnknown.Unknown,
            Error = timeUnit == RecurlyExTimeUnitAndUnknown.Unknown ? "invalid time unit" : string.Empty
        };
    }

    internal static IList<TimeUnitAndValueDto> ParseDashBasedTimeUnitValues(IList<RecurlyExToken> tokens)
    {
        var splitTokens = SplitTokens(tokens, RecurlyExTokenType.DashOrMinus);
        string year = string.Empty;
        string month = string.Empty;
        string day = string.Empty;

        var dashCount = tokens.Count(x => x.Type == RecurlyExTokenType.DashOrMinus);
        if (dashCount > 2)
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = RecurlyExTimeUnitAndUnknown.Year,
                    Value = year.Trim(),
                    IsValid = true,
                    Error = $"Invalid format yyyy-MM or yyyy-MM-dd or MM-dd"
                }
            };
        }

        if (tokens.Where(x => x.Type == RecurlyExTokenType.TimeUnit)
            .Any(x => KeywordsConstants.SecondsTimeUnitWords
                .ToUpper()
                .Contains(x.Value.ToUpper())))
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "can not use sec time units with dashes"
                }
            };
        }

        if (tokens.Where(x => x.Type == RecurlyExTokenType.TimeUnit)
            .Any(x => KeywordsConstants.MinutesTimeUnitWords
                .ToUpper()
                .Contains(x.Value.ToUpper())))
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "can not use min time units with dashes"
                }
            };
        }

        if (tokens.Where(x => x.Type == RecurlyExTokenType.TimeUnit)
            .Any(x => KeywordsConstants.HoursTimeUnitWords
                .ToUpper()
                .Contains(x.Value.ToUpper())))
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "can not use hr time units with dashes"
                }
            };
        }

        var tokens1 = splitTokens[0].Where(x => x.Type == RecurlyExTokenType.WholeNumber ||
                                                x.Type == RecurlyExTokenType.Last ||
                                                x.Type == RecurlyExTokenType.MonthWord).ToList();

        var tokens2 = splitTokens[1].Where(x => x.Type == RecurlyExTokenType.WholeNumber ||
                                                x.Type == RecurlyExTokenType.MonthWord ||
                                                x.Type == RecurlyExTokenType.Last).ToList();
        var tokens3 = splitTokens.Count > 2
            ? splitTokens[2].Where(x => x.Type == RecurlyExTokenType.WholeNumber ||
                                        x.Type == RecurlyExTokenType.LastDayOfTheMonth ||
                                        x.Type == RecurlyExTokenType.Last).ToList()
            : new List<RecurlyExToken>();
        if (tokens3.Count > 0)
        {
            year = JoinTokens(tokens1, RecurlyExTokenType.WhiteSpace, RecurlyExTokenType.EndOfExpression);
            month = JoinTokens(tokens2, RecurlyExTokenType.WhiteSpace, RecurlyExTokenType.EndOfExpression);
            day = JoinTokens(tokens3, RecurlyExTokenType.WhiteSpace, RecurlyExTokenType.EndOfExpression);
        }
        else
        {
            var yearOrMonth = JoinTokens(tokens1, RecurlyExTokenType.WhiteSpace, RecurlyExTokenType.EndOfExpression);
            var dayOrMonth = JoinTokens(tokens2, RecurlyExTokenType.WhiteSpace, RecurlyExTokenType.EndOfExpression);
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
                    TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
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
                    TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
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
                    TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
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
                TimeUnit = RecurlyExTimeUnitAndUnknown.Year, Value = year.Trim(), IsValid = true
            });
        }

        if (!string.IsNullOrEmpty(month))
        {
            result.Add(new TimeUnitAndValueDto()
            {
                TimeUnit = RecurlyExTimeUnitAndUnknown.Month, Value = month.Trim(), IsValid = true
            });
        }

        if (!string.IsNullOrEmpty(day))
        {
            result.Add(new TimeUnitAndValueDto()
            {
                TimeUnit = RecurlyExTimeUnitAndUnknown.Day, Value = day.Trim(), IsValid = true
            });
        }

        return result;
    }

    internal static IList<TimeUnitAndValueDto> ParseColonBasedTimeUnitValues(IList<RecurlyExToken> tokens)
    {
        var countColons = tokens.Count(x => x.Type == RecurlyExTokenType.Colon);
        if (countColons > 2)
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "Invalid format it should be HH:MM:SS or HH:MM"
                }
            };
        }

        if (tokens.Where(x => x.Type == RecurlyExTokenType.TimeUnit)
            .Any(x => KeywordsConstants.DaysTimeUnitWords
                .ToUpper()
                .Contains(x.Value.ToUpper())))
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "can not use days time units with colon"
                }
            };
        }

        if (tokens.Where(x => x.Type == RecurlyExTokenType.TimeUnit)
            .Any(x => KeywordsConstants.WeeksTimeUnitWords
                .ToUpper()
                .Contains(x.Value.ToUpper())))
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "can not use week time units with colon"
                }
            };
        }

        if (tokens.Where(x => x.Type == RecurlyExTokenType.TimeUnit)
            .Any(x => KeywordsConstants.MonthTimeUnitWords
                .ToUpper()
                .Contains(x.Value.ToUpper())))
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "can not use month time units with colon"
                }
            };
        }

        if (tokens.Where(x => x.Type == RecurlyExTokenType.TimeUnit)
            .Any(x => KeywordsConstants.YearsTimeUnitWords
                .ToUpper()
                .Contains(x.Value.ToUpper())))
        {
            return new List<TimeUnitAndValueDto>()
            {
                new()
                {
                    TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
                    Value = string.Empty,
                    IsValid = false,
                    Error = "can not use year time units with colon"
                }
            };
        }


        var splitTokens = SplitTokens(tokens, RecurlyExTokenType.Colon);
        var hourTokens = splitTokens[0].Where(x => x.Type == RecurlyExTokenType.WholeNumber ||
                                                   x.Type == RecurlyExTokenType.Last).ToList();

        var hourValue = JoinTokens(hourTokens, RecurlyExTokenType.EndOfExpression);

        var hasAmOrPm = tokens.Any(x => x.Type == RecurlyExTokenType.AmPm);
        if (hasAmOrPm && hourValue.IsWholeNumber())
        {
            var amOrPmTokens = tokens.Where(x => x.Type == RecurlyExTokenType.AmPm).ToList();
            var pmOrAm = amOrPmTokens.LastOrDefault();
            if (amOrPmTokens.Count > 1 || pmOrAm?.Type != RecurlyExTokenType.AmPm)
            {
                return new List<TimeUnitAndValueDto>()
                {
                    new()
                    {
                        TimeUnit = RecurlyExTimeUnitAndUnknown.Unknown,
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
            .Where(x => x.Type == RecurlyExTokenType.WholeNumber ||
                        x.Type == RecurlyExTokenType.Last)
            .ToList();
        var secondTokens = splitTokens.Count > 2 ? splitTokens[2] : new List<RecurlyExToken>();
        secondTokens = secondTokens.Where(x => x.Type == RecurlyExTokenType.WholeNumber ||
                                               x.Type == RecurlyExTokenType.Last).ToList();

        var result = new List<TimeUnitAndValueDto>()
        {
            new()
            {
                TimeUnit = RecurlyExTimeUnitAndUnknown.Hour, Value = hourValue, IsValid = true
            },
            new()
            {
                TimeUnit = RecurlyExTimeUnitAndUnknown.Minute,
                Value = JoinTokens(minuteTokens, RecurlyExTokenType.EndOfExpression),
                IsValid = true
            },
        };

        if (secondTokens.Count > 0)
        {
            result.Add(new TimeUnitAndValueDto()
            {
                TimeUnit = RecurlyExTimeUnitAndUnknown.Second,
                Value = JoinTokens(secondTokens, RecurlyExTokenType.EndOfExpression),
                IsValid = true
            });
        }

        return result;
    }

    internal static string? GetAnchoredValue(IList<RecurlyExToken> tokens)
    {
        var anchoredIndex = -1;
        if (tokens.Any(x => x.Type == RecurlyExTokenType.Anchored))
        {
            anchoredIndex = tokens.IndexOf(tokens.First(x => x.Type == RecurlyExTokenType.Anchored));
        }

        string? anchoredValue = null;
        if (anchoredIndex != -1)
        {
            anchoredValue = JoinTokens(tokens.Skip(anchoredIndex + 1).ToList(), skipTypes:
            [
                RecurlyExTokenType.WhiteSpace,
                RecurlyExTokenType.IgnoredToken,
                RecurlyExTokenType.EndOfExpression,
                RecurlyExTokenType.Comma,
                RecurlyExTokenType.TimeUnit,
            ]);
        }
        return anchoredValue;
    }
}