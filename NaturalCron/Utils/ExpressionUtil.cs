namespace NaturalCron.Utils;

internal static class ExpressionUtil
{
    internal static IDictionary<NaturalCronTimeUnit, int> MinValuePerTimeUnit = new Dictionary<NaturalCronTimeUnit, int>()
    {
        {
            NaturalCronTimeUnit.Second, 0
        },
        {
            NaturalCronTimeUnit.Minute, 0
        },
        {
            NaturalCronTimeUnit.Hour, 0
        },
        {
            NaturalCronTimeUnit.Day, 1
        },
        {
            NaturalCronTimeUnit.Week, 1
        },
        {
            NaturalCronTimeUnit.Month, 1
        },
        {
            NaturalCronTimeUnit.Year, 1
        },
    };

    internal static IDictionary<NaturalCronTimeUnit, int> MaxValuePerTimeUnit = new Dictionary<NaturalCronTimeUnit, int>()
    {
        {
            NaturalCronTimeUnit.Second, 59
        },
        {
            NaturalCronTimeUnit.Minute, 59
        },
        {
            NaturalCronTimeUnit.Hour, 23
        },
        {
            NaturalCronTimeUnit.Day, 31
        },
        {
            NaturalCronTimeUnit.Week, 7
        },
        {
            NaturalCronTimeUnit.Month, 12
        },
        {
            NaturalCronTimeUnit.Year, 9999
        },
    };

    internal static IDictionary<string, int> WeekdayMap = new Dictionary<string, int>()
    {
        {
            "SUN", 1
        },
        {
            "MON", 2
        },
        {
            "TUE", 3
        },
        {
            "WED", 4
        },
        {
            "THU", 5
        },
        {
            "FRI", 6
        },
        {
            "SAT", 7
        },
        
        {
            "SUNDAY", 1
        },
        {
            "MONDAY", 2
        },
        {
            "TUESDAY", 3
        },
        {
            "WEDNESDAY", 4
        },
        {
            "THURSDAY", 5
        },
        {
            "FRIDAY", 6
        },
        {
            "SATURDAY", 7    
        },
    };

    internal static IDictionary<string, int> MonthMap = new Dictionary<string, int>()
    {
        {
            "JAN", 1
        },
        {
            "FEB", 2
        },
        {
            "MAR", 3
        },
        {
            "APR", 4
        },
        {
            "MAY", 5
        },
        {
            "JUN", 6
        },
        {
            "JUL", 7
        },
        {
            "AUG", 8
        },
        {
            "SEP", 9
        },
        {
            "OCT", 10
        },
        {
            "NOV", 11
        },
        {
            "DEC", 12
        },
        {
            "JANUARY", 1
        },
        {
            "FEBRUARY", 2
        },
        {
            "MARCH", 3
        },
        {
            "APRIL", 4
        },
       
        {
            "JUNE", 6
        },
        
        {
            "JULY", 7
        },
        {
            "AUGUST", 8
        },
        {
            "SEPTEMBER", 9
        },
        {
            "OCTOBER", 10
        },
        {
            "NOVEMBER", 11
        },
        {
            "DECEMBER", 12
        },
    };

    internal static IDictionary<string, int> NthWeekDaysMap = KeywordsConstants.NthWeekOrdinals
    .SelectMany(
        ordinal => KeywordsConstants.WeekdayWords.Select((weekday, idx) =>
            new { Key = (ordinal + weekday).ToUpper(), Value = (idx % 7) + 1 }
        )
    )
    .ToDictionary(x => x.Key, x => x.Value);

    internal static int? TryGetValueForMatch(NaturalCronTimeUnit timeUnit, DateTime dateTime, string expression)
    {
        var year = dateTime.Year;
        var month = dateTime.Month;
        return TryGetValueForMatch(timeUnit, year, month, expression);
    }
    
    private static readonly NaturalCronCache TryGetValueForMatchCache = new (TimeSpan.FromMinutes(10), 10000);
    internal class TryGetValueForMatchResult
    {
        public int? Value { get; set; }
        public bool IsValid { get; set; }
    }
    
    internal static int? TryGetValueForMatch(NaturalCronTimeUnit timeUnit, int year, int month, string expression)
    {
        var (value, isValid) = DoTryGetValueForMatchCache(timeUnit, year, month, expression);
        return isValid ? value : null;
    }
    
    public static bool ValidateValueForMatch(NaturalCronTimeUnit timeUnit, int year, int month, string expression)
    {
        var (_, isValid) = DoTryGetValueForMatchCache(timeUnit, year, month, expression);
        return isValid;
    }
    
    private static (int? value, bool isValid) DoTryGetValueForMatchCache(NaturalCronTimeUnit timeUnit, int year, int month, string expression)
    {
        var yearForCache = timeUnit == NaturalCronTimeUnit.Day ? year : 0;
        var monthForCache = timeUnit == NaturalCronTimeUnit.Day ? month : 0;

        var key = $"{timeUnit}_!{yearForCache}_!{monthForCache}_!{expression}";

        var cached = TryGetValueForMatchCache.Get(key) as TryGetValueForMatchResult;
        if (cached != null)
        {
            return (cached.Value, cached.IsValid);
        }

        var (value, isValid) = DoTryGetValueForMatch(timeUnit, year, month, expression);

        TryGetValueForMatchCache.Set(key, new TryGetValueForMatchResult { Value = value, IsValid = isValid });

        return (value, isValid);
    }

    
    private static (int? value, bool isValid) DoTryGetValueForMatch(NaturalCronTimeUnit timeUnit, int year, int month, string expression)
    {
        var intValue = TryParseToInt(timeUnit, expression);
        if (intValue != null)
        {
            return (intValue, true);
        }
        
        if (KeywordsConstants.LastDayOfTheMonth.ToUpper().Any(x => expression.ToUpper().ContainsWholeWord(x)) || 
            KeywordsConstants.Last.ToUpper().Any(x => expression.ToUpper().ContainsWholeWord(x)))
        {
            var lastUnitValue = timeUnit == NaturalCronTimeUnit.Day ? 
                DateTime.DaysInMonth(year, month) : 
                MaxValuePerTimeUnit[timeUnit];
            lastUnitValue = ApplyMinusOrPlusForMatch(lastUnitValue, expression, GetMinValueByTimeUnit(timeUnit), lastUnitValue);
            return (lastUnitValue, true);
        }
        
        if (KeywordsConstants.FirstDayOfTheMonth.ToUpper().Any(x => expression.ToUpper().ContainsWholeWord(x)) || 
            KeywordsConstants.First.ToUpper().Any(x => expression.ToUpper().ContainsWholeWord(x)))
        {
            var firstUnitValue = GetMinValueByTimeUnit(timeUnit);
            firstUnitValue = ApplyMinusOrPlusForMatch(firstUnitValue, expression, firstUnitValue, MaxValuePerTimeUnit[timeUnit]);
            return (firstUnitValue, true);
        }


        if (timeUnit != NaturalCronTimeUnit.Day)
        {
            return (null, false);
        }
        
        if (KeywordsConstants.LastNthWeekDays.ToUpper().Any(x => expression.ToUpper().ContainsWholeWord(x)))
        {
            var lastDayOfMonth = DateTime.DaysInMonth(year, month);
            var dateTimeWithLastDayOfMonth = new DateTime(year, month, lastDayOfMonth);
        
            var weekDay = GetWeekDayFromNthExpression(expression);
        
            while ((int)dateTimeWithLastDayOfMonth.DayOfWeek + 1 != weekDay)
            {
                dateTimeWithLastDayOfMonth = dateTimeWithLastDayOfMonth.AddDays(-1);
            }
        
            var day = dateTimeWithLastDayOfMonth.Day;
            var dayValue = ApplyMinusOrPlusForMatch(day, expression, GetMinValueByTimeUnit(timeUnit), GetMaxValueByTimeUnit(timeUnit, year, month));
            return (dayValue, true);
        }

        if (KeywordsConstants.NthWeekDays.ToUpper().Any(x => expression.ToUpper().ContainsWholeWord(x)) )
        {
            var dateTimeWithFirstDayOfMonth = new DateTime(year, month, 1);
            var nthWeekDay = GetWeekDayFromNthExpression(expression);

            // Find the first day of the given week day.
            while ((int)dateTimeWithFirstDayOfMonth.DayOfWeek + 1 != nthWeekDay)
            {
                dateTimeWithFirstDayOfMonth = dateTimeWithFirstDayOfMonth.AddDays(1);
            }

            var nthExpression = expression.Split('-')[0].ToUpper().Trim();
            var position =
                nthExpression.StartsWith("FIRST") || nthExpression.StartsWith("1ST") ? 0 :
                nthExpression.StartsWith("SECOND") || nthExpression.StartsWith("2ND") ? 1 :
                nthExpression.StartsWith("THIRD") || nthExpression.StartsWith("3RD") ? 2 :
                nthExpression.StartsWith("FOURTH") || nthExpression.StartsWith("4TH") ? 3 :
                nthExpression.StartsWith("FIFTH") || nthExpression.StartsWith("5TH") ? 4 :
                -1;

            if (position == -1)
            {
                return (null, false);
            }

            var daysToAdd = position * 7;
            var result = dateTimeWithFirstDayOfMonth.AddDays(daysToAdd);

            // Check if the result is in the same month. Eg not all months has 5 fridays.
            if (result.Month != dateTimeWithFirstDayOfMonth.Month)
            {
                return (null, true);
            }

            var day = result.Day;
            var dayValue = ApplyMinusOrPlusForMatch(day, expression, GetMinValueByTimeUnit(timeUnit), GetMaxValueByTimeUnit(timeUnit, year, month));
            return (dayValue, true);
        }

        // Relative Weekdays
        if (KeywordsConstants.RelativeWeekdays.ToUpper().Any(x => expression.ToUpper().ContainsWholeWord(x)))
        {
            if (expression.ToUpper().ContainsWholeWord("LastWeekDay".ToUpper()))
            {
                var lastDayOfMonth = DateTime.DaysInMonth(year, month);
                var dateTimeWithLastDayOfMonth = new DateTime(year, month, lastDayOfMonth);

                var result = dateTimeWithLastDayOfMonth.Day;
                if (dateTimeWithLastDayOfMonth.DayOfWeek == DayOfWeek.Saturday)
                {
                    result = dateTimeWithLastDayOfMonth.Day - 1;
                }
                    
                if (dateTimeWithLastDayOfMonth.DayOfWeek == DayOfWeek.Sunday)
                {
                    result = dateTimeWithLastDayOfMonth.Day - 2;
                }
                    
                result = ApplyMinusOrPlusForMatch(result, expression, GetMinValueByTimeUnit(timeUnit), GetMaxValueByTimeUnit(timeUnit, year, month));
                    
                return (result, true);
            }
                
            if (expression.ToUpper().ContainsWholeWord("FirstWeekDay".ToUpper()))
            {
                var dateTimeWithFirstDayOfMonth = new DateTime(year, month, 1);
                var result = dateTimeWithFirstDayOfMonth.Day;
                if (dateTimeWithFirstDayOfMonth.DayOfWeek == DayOfWeek.Saturday)
                {
                    result = dateTimeWithFirstDayOfMonth.Day + 2;
                }
                    
                if (dateTimeWithFirstDayOfMonth.DayOfWeek == DayOfWeek.Sunday)
                {
                    result = dateTimeWithFirstDayOfMonth.Day + 1;
                }
                
                result = ApplyMinusOrPlusForMatch(result, expression, GetMinValueByTimeUnit(timeUnit), GetMaxValueByTimeUnit(timeUnit, year, month));

                return (result, true);
            }
                
            if (expression.ToUpper().ContainsWholeWord("ClosestWeekdayTo".ToUpper()))
            {
                var split = expression.Trim().Split(' ');
                if (split.Length != 2)
                {
                    return (null, false);
                }
                
                var day = int.Parse(split[1].Trim());
                return (GetClosestWeekday(year, month, day).Day, true);
            }
        }

        return (null, false);
    }
    
    private static DateTime GetClosestWeekday(int year, int month, int day)
    {
        if (day < 1) 
        {
            day = 1;
        }
        
        else if (day > DateTime.DaysInMonth(year, month))
        {
            day = DateTime.DaysInMonth(year, month);
        }
        
        var date = new DateTime(year, month, day);
        var daysInMonth = DateTime.DaysInMonth(year, month);

        switch (date.DayOfWeek)
        {
            case DayOfWeek.Saturday:
                if (day == 1)
                    return date.AddDays(2); // Move to Monday
                else
                    return date.AddDays(-1); // Move to Friday

            case DayOfWeek.Sunday:
                if (day == daysInMonth)
                    return date.AddDays(-2); // Move to Friday
                else
                    return date.AddDays(1); // Move to Monday

            default:
                return date; // Already a weekday
        }
    }
    
    private static int ApplyMinusOrPlusForMatch
        (int currentValue, string expression, int minValue, int maxValue)
    {
        if (!expression.Contains('+') && !expression.Contains('-')) 
        {
            return currentValue;
        }

        var result = currentValue;
        if (expression.Contains('+')) 
        {
            var plusDays = int.Parse(expression.Split('+')[1].Trim());
            result = currentValue + plusDays;
        }
        
        if (expression.Contains('-')) 
        {
            var minusDays = int.Parse(expression.Split('-')[1].Trim());
            result = currentValue - minusDays;
        }
        
        // If exceeds min or max value
        if (result < minValue)
        {
            return minValue;
        }
        
        if (result > maxValue)
        {
            return maxValue;
        }
        
        return result;
    }

    private static int GetWeekDayFromNthExpression(string expression)
    {
        var nthWeekDay = NthWeekDaysMap.First(x => expression.ToUpper().ContainsWholeWord(x.Key)).Value;
        return nthWeekDay;
    }

    internal static int? TryParseToInt(NaturalCronTimeUnit timeUnit, string value)
    {
        if (int.TryParse(value, out var result))
        {
            return result;
        }

        if (timeUnit == NaturalCronTimeUnit.Week)
        {
            if (WeekdayMap.ContainsKey(value.ToUpper()))
            {
                return WeekdayMap[value.ToUpper()];
            }
        }
        
        if (timeUnit == NaturalCronTimeUnit.Month) 
        {
            if (MonthMap.ContainsKey(value.ToUpper()))
            {
                return MonthMap[value.ToUpper()];
            }
        }
        
        return null;
    }

    internal static int ParseToInt(NaturalCronTimeUnit timeUnit, string value)
    {
        var result = TryParseToInt(timeUnit, value);
        if (result == null)
        {
            throw new FormatException($"Expected {value} to be a number");
        }
        
        return result.Value;
    }

    internal static NaturalCronTimeUnit? TryParseToTimeUnit(string value)
    {
        if (KeywordsConstants.SecondsTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return NaturalCronTimeUnit.Second;
        }
        
        if (KeywordsConstants.MinutesTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return NaturalCronTimeUnit.Minute;
        }
        
        if (KeywordsConstants.HoursTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return NaturalCronTimeUnit.Hour;
        }
        
        if (KeywordsConstants.DaysTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return NaturalCronTimeUnit.Day;
        }
        
        if (KeywordsConstants.WeeksTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return NaturalCronTimeUnit.Week;
        }
        
        if (KeywordsConstants.MonthTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return NaturalCronTimeUnit.Month;
        }
        
        if (KeywordsConstants.YearsTimeUnitWords.ToUpper().Contains(value.ToUpper()))
        {
            return NaturalCronTimeUnit.Year;
        }

        return null;
    }

    internal static int GetMinValueByTimeUnit(NaturalCronTimeUnit timeUnit)
    {
        return MinValuePerTimeUnit[timeUnit];
    }
    
    internal static int GetMaxValueByTimeUnit(NaturalCronTimeUnit timeUnit, DateTime dateTime)
    {
        return GetMaxValueByTimeUnit(timeUnit, dateTime.Year, dateTime.Month);
    }
    
    internal static int GetMaxValueByTimeUnit(NaturalCronTimeUnit timeUnit, int year, int month)
    {
        if (timeUnit == NaturalCronTimeUnit.Day)
        {
            return DateTime.DaysInMonth(year, month);
        }

        return MaxValuePerTimeUnit[timeUnit];
    }
}