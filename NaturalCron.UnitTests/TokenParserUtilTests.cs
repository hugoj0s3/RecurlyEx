using System.Collections.Generic;
using System.Linq;
using NaturalCron;
using NaturalCron.Tokens;
using NaturalCron.Tokens.Parser;
using FluentAssertions;

namespace NaturalCron.UnitTests;

public class TokenParserUtilTests
{
    [Theory]
    [MemberData(nameof(GetValidTokenTestData))]
    public void TokenizerValidExpressions_Success(ValidTokenTestData testData)
    {
        var expression = testData.Expression;
        var tokens = TokenParserUtil.Tokenizer(expression);

        tokens.Count.Should().Be(testData.TotalCount);

        foreach (var tokenType in testData.TokenCounts.Keys)
        {
            tokens.Count(x => x.Type == tokenType).Should().Be(testData.TokenCounts[tokenType].Length, $"Token type: {tokenType}");
            tokens.Where(x => x.Type == tokenType).Select(x => x.Value).Should().BeEquivalentTo(testData.TokenCounts[tokenType]);
        }
    }

    public static IEnumerable<object[]> GetValidTokenTestData()
    {
        var list = new List<ValidTokenTestData>
        {
            new()
            {
                Expression = "@every  day @on 10:30am",
                TotalCount = 11,
                TokenCounts = new Dictionary<NaturalCronTokenType, string[]>
                {
                    { NaturalCronTokenType.RuleSpec, ["@every", "@on"] },
                    { NaturalCronTokenType.TimeUnit, ["day"] },
                    { NaturalCronTokenType.AmPm, ["am"] },
                    { NaturalCronTokenType.WholeNumber, ["10", "30"] },
                    { NaturalCronTokenType.Colon, [":"] },
                    { NaturalCronTokenType.WhiteSpace, [" ", " ", " "] },
                    { NaturalCronTokenType.EndOfExpression, ["\0"] }
                }
            },
            new()
            {
                Expression = "@every 2 months @at day 10 @on 10:30pm",
                TotalCount = 19,
                TokenCounts = new Dictionary<NaturalCronTokenType, string[]>
                {
                    { NaturalCronTokenType.RuleSpec, ["@every", "@at", "@on"] },
                    { NaturalCronTokenType.TimeUnit, ["months", "day"] },
                    { NaturalCronTokenType.AmPm, ["pm"] },
                    { NaturalCronTokenType.WholeNumber, ["2", "10", "10", "30"] },
                    { NaturalCronTokenType.Colon, [":"] },
                    { NaturalCronTokenType.EndOfExpression, ["\0"] }
                },
            },
            new()
            {
                Expression = "@every 3 years @at JAN-25 @on 10:30am",
                TotalCount = 19,
                TokenCounts = new Dictionary<NaturalCronTokenType, string[]>
                {
                    { NaturalCronTokenType.RuleSpec, ["@every", "@at", "@on"] },
                    { NaturalCronTokenType.TimeUnit, ["years"] },
                    { NaturalCronTokenType.AmPm, ["am"] },
                    { NaturalCronTokenType.WholeNumber, ["3", "25", "10", "30"] },
                    { NaturalCronTokenType.Colon, [":"] },
                    { NaturalCronTokenType.DashOrMinus, ["-"] },
                    { NaturalCronTokenType.MonthWord, ["JAN"] },
                    { NaturalCronTokenType.EndOfExpression, ["\0"] }
                },
            },
            new()
            {
                Expression = "@secondly @between 10:30am and 11:50pm",
                TotalCount = 16,
                TokenCounts = new Dictionary<NaturalCronTokenType, string[]>
                {
                    { NaturalCronTokenType.RuleSpec, ["@secondly", "@between"] },
                    { NaturalCronTokenType.AmPm, ["am", "pm"] },
                    { NaturalCronTokenType.WholeNumber, ["10", "30", "11", "50"] },
                    { NaturalCronTokenType.Colon, [":", ":"] },
                    { NaturalCronTokenType.And, ["and"] }
                },
            },
            new()
            {
                Expression = "@minutely @on days [1, 3, 10, 25]",
                TotalCount = 19,
                TokenCounts = new Dictionary<NaturalCronTokenType, string[]>
                {
                    { NaturalCronTokenType.RuleSpec, ["@minutely", "@on"] },
                    { NaturalCronTokenType.TimeUnit, ["days"] },
                    { NaturalCronTokenType.WholeNumber, ["1", "3", "10", "25"] },
                    { NaturalCronTokenType.Comma, [",", ",", ","] },
                    { NaturalCronTokenType.OpenBrackets, ["["] },
                    { NaturalCronTokenType.CloseBrackets, ["]"] },
                },
            }
        };
        
        return list.Select(x => new object[] { x });
    }

    public class ValidTokenTestData
    {
        public string Expression { get; set; } = string.Empty;
        public int TotalCount { get; set; }
        public IDictionary<NaturalCronTokenType, string[]> TokenCounts { get; set; } = new Dictionary<NaturalCronTokenType, string[]>();
    }
}