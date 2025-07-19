using System.Diagnostics.CodeAnalysis;
using System.Text;
using RecurlyEx.Tokens;
using RecurlyEx.Rules;
using RecurlyEx.Tokens.Parser;
using RecurlyEx.Utils;
using TimeZoneConverter;

namespace RecurlyEx;

public class RecurlyEx
{
    private static readonly RecurlyExTimeUnit[] TimeUnitsWithoutTimezoneAndWeek =
    [
        RecurlyExTimeUnit.Second,
        RecurlyExTimeUnit.Minute,
        RecurlyExTimeUnit.Hour,
        RecurlyExTimeUnit.Day,
        RecurlyExTimeUnit.Month,
        RecurlyExTimeUnit.Year
    ];
    
    private static readonly RecurlyExTimeUnit[] TimeUnitsWithoutTimezone =
    [
        RecurlyExTimeUnit.Second,
        RecurlyExTimeUnit.Minute,
        RecurlyExTimeUnit.Hour,
        RecurlyExTimeUnit.Day,
        RecurlyExTimeUnit.Month,
        RecurlyExTimeUnit.Year,
        RecurlyExTimeUnit.Week
    ];

    internal RecurlyEx() {}
    public string Expression { get; set; } = string.Empty;
    public IReadOnlyCollection<RecurlyExRule> Rules { get; internal set; } = new List<RecurlyExRule>();

    public IList<RecurlyExRule> SecondRules() =>
        Rules.Where(x => x.TimeUnit == RecurlyExTimeUnit.Second).ToList();

    public IList<RecurlyExRule> MinuteRules() =>
        Rules.Where(x => x.TimeUnit == RecurlyExTimeUnit.Minute).ToList();

    public IList<RecurlyExRule> HourRules() =>
        Rules.Where(x => x.TimeUnit == RecurlyExTimeUnit.Hour).ToList();

    public IList<RecurlyExRule> DayRules() =>
        Rules.Where(x => x.TimeUnit == RecurlyExTimeUnit.Day).ToList();

    public IList<RecurlyExRule> WeekRules() =>
        Rules.Where(x => x.TimeUnit == RecurlyExTimeUnit.Week).ToList();

    public IList<RecurlyExRule> MonthRules() =>
        Rules.Where(x => x.TimeUnit == RecurlyExTimeUnit.Month).ToList();

    public IList<RecurlyExRule> YearRules() =>
        Rules.Where(x => x.TimeUnit == RecurlyExTimeUnit.Year).ToList();

    public RecurlyExIanaTimeZoneRule? TimeZoneRule() =>
        Rules.FirstOrDefault(x => x.TimeUnit == RecurlyExTimeUnit.TimeZone) as RecurlyExIanaTimeZoneRule;
    /// <summary>
    /// Tries to get the next occurrence in UTC. Returns null if no occurrence is found within the specified lookahead period.
    /// </summary>
    /// <param name="utcBaseTime">The base time in UTC from which to find the next occurrence</param>
    /// <param name="utcMaxLookahead">Maximum lookahead time in UTC. If null, defaults to DateTime.MaxValue</param>
    /// <returns>The next occurrence in UTC, or null if no occurrence is found</returns>
    public DateTime? TryGetNextOccurrenceInUtc(DateTime utcBaseTime, DateTime? utcMaxLookahead = null)
    {
        // 1. Convert to local time
        var localBaseTime = TimeZoneRule()?.ConvertFromUtc(utcBaseTime) ?? utcBaseTime;
        DateTime localMaxLookahead;
        if (utcMaxLookahead.HasValue)
        {
            localMaxLookahead = TimeZoneRule()?.ConvertFromUtc(utcMaxLookahead.Value) ?? utcMaxLookahead.Value;
        }
        else 
        {
            localMaxLookahead = DateTime.MaxValue;
        }
        
        // 2. Get local next occurrence
        var result = InternalTryGetLocalNextOccurrence(localBaseTime, localMaxLookahead);
        
        // 2. Convert to UTC
        return !result.HasValue ? null : TimeZoneRule()?.ConvertToUtc(result.Value) ?? result.Value;
    }

    private DateTime? InternalTryGetLocalNextOccurrence(DateTime localBaseTime, DateTime localMaxLookahead)
    {
        // 1. Normalize input
        localBaseTime = new DateTime(
            localBaseTime.Year, localBaseTime.Month, localBaseTime.Day,
            localBaseTime.Hour, localBaseTime.Minute, localBaseTime.Second);
        
        var localNextOccurrence = localBaseTime;

        // 2. Apply "every X" rule anchoring if present
        var everyRule = Rules.OfType<RecurlyExEveryXRule>().FirstOrDefault();
        if (everyRule != null)
        {
            // Anchor to the correct value (e.g., hour 0, minute 0, etc.)
            var matchableRules = this.Rules.OfType<RecurlyExMatchableRule>().ToList();
            var anchoredOccurrence = everyRule.ApplyAnchoredValueIfPresent(localNextOccurrence, localBaseTime, matchableRules);
            
            // If at the base time and all rules match, advance once more
            if (ShouldAddEveryX(localBaseTime, anchoredOccurrence, everyRule.TimeUnit))
            {
                anchoredOccurrence = everyRule.AddEveryXTime(anchoredOccurrence, matchableRules);
            }

            // If we crossed into a new upper unit (e.g., new day), re-anchor and re-advance if needed
            if (everyRule.AnchoredValue != null)
            {
                var upperUnit = everyRule.TimeUnit + 1;
                if (upperUnit < RecurlyExTimeUnit.Year)
                {
                    var upperPartNow = DateTimeUtil.GetPartValue(upperUnit, anchoredOccurrence);
                    var upperPartBase = DateTimeUtil.GetPartValue(upperUnit, localBaseTime);
                    if (upperPartNow > upperPartBase)
                    {
                        anchoredOccurrence = everyRule.ApplyAnchoredValueIfPresent(anchoredOccurrence, localNextOccurrence, matchableRules);

                        // If at the base time and all rules match, advance once more
                        if (ShouldAddEveryX(localBaseTime, anchoredOccurrence, everyRule.TimeUnit))
                        {
                            anchoredOccurrence = everyRule.AddEveryXTime(anchoredOccurrence, matchableRules);
                        }
                    }
                }
            }

            localNextOccurrence = anchoredOccurrence;
        }

        // 3. If still not after base time, advance by the minimum non-everyX unit
        if (localNextOccurrence <= localBaseTime)
        {
            var matchableRules = this.Rules.OfType<RecurlyExMatchableRule>().ToList();
            
            // 1. Filter out all between rules from matchableRules
            var nonBetweenRules = matchableRules
                .Where(x => x is not RecurlyExBetweenRule && x is not RecurlyExBetweenMultiplesRule);

            // 2. Add between rules that do NOT match localNextOccurrence
            var unmatchedBetweenRules = matchableRules
                .Where(x => x is RecurlyExBetweenRule b || x is RecurlyExBetweenMultiplesRule)
                .Where(x => !x.Match(localNextOccurrence))
                .ToList();

            // 3. Combine all candidates
            var allCandidates = nonBetweenRules
                .Concat(unmatchedBetweenRules);

            // 4. Find the minimum TimeUnit
            var minAdvanceUnit = allCandidates
                .OrderBy(x => x.TimeUnit)
                .First()
                .TimeUnit;
            
            if (minAdvanceUnit == RecurlyExTimeUnit.Week)
            {
                minAdvanceUnit = RecurlyExTimeUnit.Day;
            }

            while (localNextOccurrence <= localBaseTime)
            {
                localNextOccurrence = AdvanceNextOccurrence(localNextOccurrence, minAdvanceUnit, 1);
            }
        }

        // 4. Find the next actual match (with all rules)
        (localNextOccurrence, var hasMatch) = FindNextMatchOccurrence(localNextOccurrence, localMaxLookahead);

        // 5. Return UTC if match found
        return hasMatch ? localNextOccurrence : (DateTime?)null;
    }

    /// <summary>
    /// Gets the next occurrence in UTC. Throws an exception if no occurrence is found within the specified lookahead period.
    /// </summary>
    /// <param name="utcBaseTime">The base time in UTC from which to find the next occurrence</param>
    /// <param name="utcMaxLookahead">Maximum lookahead time in UTC. If null, defaults to DateTime.MaxValue</param>
    /// <returns>The next occurrence in UTC</returns>
    /// <exception cref="InvalidOperationException">Thrown when no next occurrence is found</exception>
    public DateTime GetNextOccurrenceInUtc(DateTime utcBaseTime, DateTime? utcMaxLookahead = null)
    {
        var nextOccurrence = TryGetNextOccurrenceInUtc(utcBaseTime, utcMaxLookahead);
        if (nextOccurrence == null)
        {
            throw new InvalidOperationException("No next occurrence found");
        }

        return nextOccurrence.Value;
    }
    
    /// <summary>
    /// Tries to get the next occurrences in UTC. Returns as many occurrences as found, up to the specified count.
    /// Stops when no more occurrences are found or the count is reached.
    /// </summary>
    /// <param name="utcBaseTime">The base time in UTC from which to find the next occurrences</param>
    /// <param name="count">The number of occurrences to retrieve</param>
    /// <param name="utcMaxLookahead">Maximum lookahead time in UTC. If null, defaults to DateTime.MaxValue</param>
    /// <returns>A list of occurrences in UTC (may contain fewer than the requested count)</returns>
    public IList<DateTime> TryGetNextOccurrencesInUtc(DateTime utcBaseTime, int count, DateTime? utcMaxLookahead = null)
    {
        var result = new List<DateTime>();
        for (var i = 0; i < count; i++)
        {
            var nextOccurrence = TryGetNextOccurrenceInUtc(utcBaseTime, utcMaxLookahead);
            if (nextOccurrence == null)
            {
                break;
            }

            result.Add(nextOccurrence.Value);
            utcBaseTime = nextOccurrence.Value;
        }

        return result;
    }
    
    /// <summary>
    /// Gets the next occurrences in UTC. Throws an exception if fewer occurrences than requested are found.
    /// Use TryGetNextOccurrencesInUtc if you are not sure there are enough occurrences available.
    /// </summary>
    /// <param name="utcBaseTime">The base time in UTC from which to find the next occurrences</param>
    /// <param name="count">The number of occurrences to retrieve</param>
    /// <param name="utcMaxLookahead">Maximum lookahead time in UTC. If null, defaults to DateTime.MaxValue</param>
    /// <returns>A list of occurrences in UTC</returns>
    /// <exception cref="InvalidOperationException">Thrown when fewer than the requested number of occurrences are found</exception>
    public IList<DateTime> GetNextOccurrencesInUtc(DateTime utcBaseTime, int count, DateTime? utcMaxLookahead = null)
    {
        var result = TryGetNextOccurrencesInUtc(utcBaseTime, count, utcMaxLookahead);
        if (result.Count < count)
        {
            throw new InvalidOperationException("Not enough occurrences found");
        }

        return result;
    }
    
    /// <summary>
    /// Tries to get the next occurrence using the current thread's timezone. Returns null if no occurrence is found within the specified lookahead period.
    /// </summary>
    /// <param name="baseTime">The base time in the current thread's timezone from which to find the next occurrence</param>
    /// <param name="maxLookahead">Maximum lookahead time in the current thread's timezone. If null, defaults to DateTime.MaxValue</param>
    /// <returns>The next occurrence in the current thread's timezone, or null if no occurrence is found</returns>
    public DateTime? TryGetNextOccurrence(DateTime baseTime, DateTime? maxLookahead = null)
    {
        if (this.TimeZoneRule() == null)
        {
            return InternalTryGetLocalNextOccurrence(baseTime, maxLookahead ?? DateTime.MaxValue);
        }
        
        // Convert local time to UTC and call the UTC method
        baseTime = TimeZoneInfo.ConvertTimeToUtc(baseTime);
        maxLookahead = maxLookahead.HasValue ? TimeZoneInfo.ConvertTimeToUtc(maxLookahead.Value) : DateTime.MaxValue;
        
        var result = TryGetNextOccurrenceInUtc(baseTime, maxLookahead.Value);
        
        // Convert UTC to local time
        return result == null ? null : TimeZoneInfo.ConvertTimeFromUtc(result.Value, TimeZoneInfo.Local);
    }
    
    /// <summary>
    /// Gets the next occurrence using the current thread's timezone. Throws an exception if no occurrence is found within the specified lookahead period.
    /// </summary>
    /// <param name="baseTime">The base time in the current thread's timezone from which to find the next occurrence</param>
    /// <param name="maxLookahead">Maximum lookahead time in the current thread's timezone. If null, defaults to DateTime.MaxValue</param>
    /// <returns>The next occurrence in the current thread's timezone</returns>
    /// <exception cref="InvalidOperationException">Thrown when no next occurrence is found</exception>
    public DateTime GetNextOccurrence(DateTime baseTime, DateTime? maxLookahead = null)
    {
        var nextOccurrence = TryGetNextOccurrence(baseTime, maxLookahead);
        if (nextOccurrence == null)
        {
            throw new InvalidOperationException("No next occurrence found");
        }

        return nextOccurrence.Value;
    }

    /// <summary>
    /// Tries to get the next occurrences using the current thread's timezone. Returns as many occurrences as found, up to the specified count.
    /// Stops when no more occurrences are found or the count is reached.
    /// </summary>
    /// <param name="baseTime">The base time in the current thread's timezone from which to find the next occurrences</param>
    /// <param name="count">The number of occurrences to retrieve</param>
    /// <param name="maxLookahead">Maximum lookahead time in the current thread's timezone. If null, defaults to DateTime.MaxValue</param>
    /// <returns>A list of occurrences in the current thread's timezone (may contain fewer than the requested count)</returns>
    public IList<DateTime> TryGetNextOccurrences(DateTime baseTime, int count, DateTime? maxLookahead = null)
    {
        var result = new List<DateTime>();
        for (var i = 0; i < count; i++)
        {
            var nextOccurrence = TryGetNextOccurrence(baseTime, maxLookahead);
            if (nextOccurrence == null)
            {
                break;
            }

            result.Add(nextOccurrence.Value);
            baseTime = nextOccurrence.Value;
        }

        return result;
    }

    /// <summary>
    /// Gets the next occurrences using the current thread's timezone. Throws an exception if fewer occurrences than requested are found.
    /// Use TryGetNextOccurrences if you are not sure there are enough occurrences available.
    /// </summary>
    /// <param name="baseTime">The base time in the current thread's timezone from which to find the next occurrences</param>
    /// <param name="count">The number of occurrences to retrieve</param>
    /// <param name="maxLookahead">Maximum lookahead time in the current thread's timezone. If null, defaults to DateTime.MaxValue</param>
    /// <returns>A list of occurrences in the current thread's timezone</returns>
    /// <exception cref="InvalidOperationException">Thrown when fewer than the requested number of occurrences are found</exception>
    public IList<DateTime> GetNextOccurrences(DateTime baseTime, int count, DateTime? maxLookahead = null)
    {
        var result = TryGetNextOccurrences(baseTime, count, maxLookahead);
        if (result.Count < count)
        {
            throw new InvalidOperationException("Not enough occurrences found");
        }

        return result;
    }

    public static RecurlyEx Parse(string expression)
    {
        var (recurlyEx, errors) = InternalTryParse(expression);
        if (errors.Any())
        {
            throw new ArgumentException(string.Join("\n", errors));
        }

        return recurlyEx!;
    }

    public static (RecurlyEx?, IList<string> errors) TryParse(string expression)
    {
        var (recurlyEx, errors) = InternalTryParse(expression);

        if (errors.Any())
        {
            return (null, errors);
        }

        return (recurlyEx, new List<string>());
    }

    internal static (RecurlyEx?, IList<string> errors) InternalTryParse(string expression)
    {
        var tokens = TokenParserUtil.Tokenizer(expression);
        if (tokens.Any(x => !x.IsValid))
        {
            return (null, tokens.Where(x => !x.IsValid).Select(x => x.GetErrorWithLinePosition()).ToList());
        }

        var groupSpecs = TokenParserUtil.GroupSpecs(tokens);
        var rules = new List<RecurlyExRule>();
        var parserErrors = new List<string>();
        foreach (var groupedSpec in groupSpecs)
        {
            var result = TokenParserUtil.ParseGroupedSpec(groupedSpec);
            rules.AddRange(result.rules);
            parserErrors.AddRange(result.errors.Select(x => $"{TokenParserUtil.JoinTokens(groupedSpec.Tokens)} - {x}"));
        }

        if (tokens.Any(x => !x.IsValid))
        {
            var tokenErrors = tokens.Where(x => !x.IsValid).Select(x => x.GetErrorWithLinePosition()).ToList();
            parserErrors.AddRange(tokenErrors);
        }

        if (parserErrors.Any())
        {
            return (null, parserErrors);
        }

        RecurlyEx recurlyEx = new()
        {
            Expression = expression, 
            Rules = rules.AsReadOnly(),
        };
        
        var validationErrors = recurlyEx.Validate();

        if (!validationErrors.Any())
        {
           var everyXRule = recurlyEx.Rules.FirstOrDefault(x => x is RecurlyExEveryXRule);
           if (everyXRule != null && everyXRule.TimeUnit >= RecurlyExTimeUnit.Month)
           {
               var matchableRules = recurlyEx.Rules.OfType<RecurlyExMatchableRule>().ToList();
               // If there is no day rule and every is year or month, transform week rules to day rules.
               // It is basically the same. E.g every month on [monday, wednesday] => every month on [1stMon, 1stWed]
               TransformWeekRulesToFirstWeekDayRulesIfNeeded(matchableRules);
           }
        }
        
        return (recurlyEx, validationErrors);
    }
    
    private static void TransformWeekRulesToFirstWeekDayRulesIfNeeded(List<RecurlyExMatchableRule> matchableRules)
    {
        bool hasDayRules = matchableRules.Any(x =>
            (x is RecurlyExAtInOnRule || x is RecurlyExAtInOnMultiplesRule) &&
            (x.TimeUnit == RecurlyExTimeUnit.Day || 
            (x is RecurlyExAtInOnMultiplesRule m && m.HasTimeUnit(RecurlyExTimeUnit.Day)))
        );
        
        if (hasDayRules)
        {
            return;
        }

        var weekRules = matchableRules
            .Where(x => x.TimeUnit == RecurlyExTimeUnit.Week ||
                        (x is RecurlyExAtInOnMultiplesRule m && m.HasTimeUnit(RecurlyExTimeUnit.Week)))
            .ToList();
        
        if (!weekRules.Any())
        {
            return;
        }

        foreach (var weekRule in weekRules)
        {
            if (weekRule is RecurlyExAtInOnMultiplesRule m)
            {
                m.DayRules = m.WeekRules
                    .Select(x =>
                    {
                        var weekdayIdx = ExpressionUtil.TryGetValueForMatch(RecurlyExTimeUnit.Week, DateTime.Now, x.InnerExpression) ?? 1;
                        return new RecurlyExAtInOnRule
                        {
                            TimeUnit = RecurlyExTimeUnit.Day,
                            FullExpression = x.FullExpression,
                            InnerExpression = "1st" + KeywordsConstants.WeekdayWords[weekdayIdx - 1]
                        };
                    })
                    .ToArray();
                m.WeekRules = Array.Empty<RecurlyExAtInOnRule>();
            }
            else if (weekRule is RecurlyExAtInOnRule atInOnRule)
            {
                var weekdayIdx = ExpressionUtil.TryGetValueForMatch(RecurlyExTimeUnit.Week, DateTime.Now, atInOnRule.InnerExpression) ?? 1;
                atInOnRule.InnerExpression = "1st" + KeywordsConstants.WeekdayWords[weekdayIdx - 1];
            }
        }
    }

    private (int timeToAdvance, RecurlyExTimeUnit timeUnit) GetTimeToAdvanceToNextOccurence(DateTime datetime)
    {
        var unmatchedRules = Rules
            .OfType<RecurlyExMatchableRule>()
            .Where(x => !x.Match(datetime))
            .ToList();

        if (!unmatchedRules.Any())
        {
            return (1, RecurlyExTimeUnit.Second);
        }
        
        var minTimeUnit = unmatchedRules.Select(x =>
            {
                
                var (timeToAdvance, timeUnit) = x.GetTimeToAdvanceToNextOccurence(datetime);
               
                // In case of week we just advance to next day. Week doesn't follow the pattern of other time units e.g advance one week
                // it will simple advance to next day of the week.
                if (timeUnit == RecurlyExTimeUnit.Week)
                {
                   return (1, RecurlyExTimeUnit.Day);
                }

                // Not all month have the same number of days.
                // For special cases like last day of month, closest day to, Last[WeekDay] It can advance to the next occurrence skipping the day rules.
                if (timeUnit == RecurlyExTimeUnit.Month || 
                    (timeUnit == RecurlyExTimeUnit.Year && DateTime.IsLeapYear(datetime.Year) && datetime.Month == 2))
                {
                    var simulatedNextOccurrence = AdvanceNextOccurrence(datetime, RecurlyExTimeUnit.Month, timeToAdvance);
                    simulatedNextOccurrence = DateTimeUtil.SetPartValue(RecurlyExTimeUnit.Day, simulatedNextOccurrence, 1);
                    var totalDays = (int)(simulatedNextOccurrence - datetime).TotalDays;
                    return (totalDays, RecurlyExTimeUnit.Day);
                }
                
                return (timeToAdvance, timeUnit);
            })
            .OrderBy(x => DateTimeUtil.GetDurationInSeconds(x.Item2, x.Item1, datetime.Year, datetime.Month))
            .FirstOrDefault();

        return minTimeUnit;
    }

    private bool AllMatch(DateTime dateTime)
    {
        return AllMatch(dateTime, this.Rules.ToList());
    }
    
    private bool ShouldAddEveryX(DateTime baseTime, DateTime anchoredOccurrence, RecurlyExTimeUnit everyXTimeUnit)
    {
        var result = anchoredOccurrence == baseTime && AllMatch(anchoredOccurrence);
        if (!result)
        {
            return false;
        }
        
        // If we have multiples ensure it matches with the last occurence of it to guarantee the cycle is complete.
        var multiples = Rules.OfType<RecurlyExAtInOnMultiplesRule>()
            .Where(x => x.GetAllRules().Any(y => y.TimeUnit <= everyXTimeUnit))
            .ToList();
        return multiples.All(x => x.Match(x.GetLength() - 1, baseTime));
    }

    private static bool AllMatch(DateTime dateTime, IList<RecurlyExRule> rules)
    {
        return rules.OfType<RecurlyExMatchableRule>().All(x => x.Match(dateTime));
    }

    private IList<string> Validate()
    {
        var errors = new List<string>();
        
        bool hasAtOnInOrEveryXRule = Rules.Any(x => x is RecurlyExAtInOnRule) ||
                                     Rules.Any(x => x is RecurlyExEveryXRule) ||
                                     Rules.Any(x => x is RecurlyExAtInOnMultiplesRule);
        if (!hasAtOnInOrEveryXRule)
        {
            errors.Add("At least one At/On/In or Every/Secondly/Minutely/Hourly/Daily/Weekly/Monthly/Yearly is required.");
            return errors;
        }

        // 2. Only one EveryX rule allowed
        var everyXRules = Rules.Where(x => x is RecurlyExEveryXRule).ToList();
        if (everyXRules.Count > 1)
        {
            errors.Add("Duplicate Every/Secondly/Minutely/Hourly/Daily/Weekly/Monthly/Yearly rule is not allowed.");
            return errors;
        }

        // 3. No At/On/In or AtInOnMultiples rule at same unit as EveryX.
        //    No Every day combined with At/On/In week rule
        //    No Every week with anchored combined with At/On/In multiples rule
        var everyXRule = everyXRules.FirstOrDefault() as RecurlyExEveryXRule;
        if (everyXRule != null)
        {
            var everyTimeUnit = everyXRule.TimeUnit;
            if (everyTimeUnit != RecurlyExTimeUnit.Week)
            {
                if (Rules.Any(x =>
                        (x.TimeUnit == everyTimeUnit && x is RecurlyExAtInOnRule) ||
                        (x is RecurlyExAtInOnMultiplesRule m && m.HasTimeUnit(everyTimeUnit))))
                {
                    errors.Add("You cannot use an At/On/In rules at the same time unit as an 'every X' rule except for week.");
                    return errors;
                }
            }

            if (everyTimeUnit == RecurlyExTimeUnit.Week)
            {
                if (Rules.Any(x =>
                        (x.TimeUnit == RecurlyExTimeUnit.Week && x is RecurlyExAtInOnRule) ||
                        (x is RecurlyExAtInOnMultiplesRule m && m.HasTimeUnit(RecurlyExTimeUnit.Week))) && 
                    !string.IsNullOrEmpty(everyXRule.AnchoredValue))
                {
                    errors.Add("You cannot combine an 'every week' + anchored rule with an At/On/In rule for weeks.");
                    return errors;
                }
            }
        }
        
        // 4. No duplicate At/On/In rules at the same time unit (Between is allowed)
        foreach (var timeUnit in TimeUnitsWithoutTimezone)
        {
            var atOnInRules = Rules.Where(x => x is RecurlyExAtInOnMultiplesRule || x is RecurlyExAtInOnRule);
            var count = atOnInRules
                .Count(x => x.TimeUnit == timeUnit || x is RecurlyExAtInOnMultiplesRule m && m.HasTimeUnit(timeUnit));

            if (count > 1)
            {
                errors.Add($"Duplicate At/On/In rules for the same time unit are not allowed.");
                return errors;
            }
        }

        // 5. Check if there are any invalid between rules
        var allBetweenRules = Rules
            .SelectMany(x =>
            {
                if (x is RecurlyExBetweenRule b)
                {
                    return new[]
                    {
                        b
                    };
                }

                if (x is RecurlyExBetweenMultiplesRule bM)
                {
                    return bM.BetweenRules;
                }

                return Array.Empty<RecurlyExBetweenRule>();
            }).ToList();

        if (allBetweenRules.Any() && everyXRule == null)
        {
            errors.Add("Between rules is not allowed without Every/Secondly/Minutely/Hourly/Daily/Weekly/Monthly/Yearly rule.");
            return errors;
        }

        foreach (var betweenRule in allBetweenRules)
        {
            if (!betweenRule.Validate())
            {
                var rangeError = string.Join(", ",
                    betweenRule.StartEndValues.Select(x => $"{betweenRule.FullExpression}"));
                errors.Add($"Between rule ranges are invalid for {rangeError}");
            }
        }

        var allAtOnInRules = Rules.Where(x => x is RecurlyExAtInOnMultiplesRule || x is RecurlyExAtInOnRule)
            .SelectMany(x =>
            {
                if (x is RecurlyExAtInOnMultiplesRule b)
                {
                    return b.GetAllRules();
                }

                if (x is RecurlyExAtInOnRule bM)
                {
                    return new[]
                    {
                        bM
                    };
                }

                return Array.Empty<RecurlyExAtInOnRule>();
            })
            .ToList();
        foreach (var atOnInRule in allAtOnInRules)
        {
            var isValid = ExpressionUtil.ValidateValueForMatch(atOnInRule.TimeUnit, 1, 1, atOnInRule.InnerExpression);
            if (!isValid)
            {
                errors.Add($"At/On/In rule '{atOnInRule.FullExpression}' is not valid.");
            }
        }

        var timezoneRule = Rules.FirstOrDefault(x => x is RecurlyExIanaTimeZoneRule) as RecurlyExIanaTimeZoneRule;
        if (timezoneRule != null)
        {
            if (!TZConvert.KnownIanaTimeZoneNames.ToArray().ToUpper().Contains(timezoneRule.IanaId.ToUpper()))
            {
                errors.Add($"Timezone rule '{timezoneRule.IanaId}' is not valid.");
            }
        }
        
        if (everyXRule != null && everyXRule.Value <= 0)
        {
            errors.Add("Every value cannot be less than or equal to 0.");
        }

        return errors;
    }

    private (DateTime, bool) FindNextMatchOccurrence(DateTime localNextOccurrence, DateTime utcMaxLookahead)
    {
        return FindNextMatchOccurrence(localNextOccurrence, utcMaxLookahead, Rules.ToList());
    }

    private (DateTime, bool) FindNextMatchOccurrence(DateTime localNextOccurrence, DateTime utcMaxLookahead, IList<RecurlyExRule> rules)
    {
        bool hasMatch = false;
        while (localNextOccurrence < utcMaxLookahead && !hasMatch)
        {
            if (AllMatch(localNextOccurrence, rules))
            {
                hasMatch = true;
                break;
            }

            var timeUnitToAdvance = GetTimeToAdvanceToNextOccurence(localNextOccurrence);
            localNextOccurrence = AdvanceNextOccurrence(localNextOccurrence, timeUnitToAdvance.Item2, timeUnitToAdvance.Item1);
        }

        return (localNextOccurrence, hasMatch);
    }

    private static DateTime AdvanceNextOccurrence(DateTime localNextOccurrence, RecurlyExTimeUnit timeUnitToAdvance, int amount)
    {
        return DateTimeUtil.AdvanceTimeSafety(timeUnitToAdvance, localNextOccurrence, amount);
    }

    private static bool HasTimeUnitRule(IList<RecurlyExMatchableRule> rules, RecurlyExTimeUnit timeUnit)
    {
        foreach (var recurlyExRule in rules)
        {
            if (recurlyExRule.TimeUnit == timeUnit)
            {
                return true;
            }

            if (recurlyExRule is RecurlyExAtInOnMultiplesRule atInOnMultiplesRule && 
                atInOnMultiplesRule.HasTimeUnit(timeUnit))
            {
                return true;
            }
        }

        return false;
    }
}