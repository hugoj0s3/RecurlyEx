using RecurlyEx;
using RecurlyEx.Tokens;
using RecurlyEx.Tokens.Parser;
using FluentAssertions;

namespace RecurlyEx.UnitTests;

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
                TokenCounts = new Dictionary<RecurlyExTokenType, string[]>
                {
                    { RecurlyExTokenType.RuleSpec, ["@every", "@on"] },
                    { RecurlyExTokenType.TimeUnit, ["day"] },
                    { RecurlyExTokenType.AmPm, ["am"] },
                    { RecurlyExTokenType.WholeNumber, ["10", "30"] },
                    { RecurlyExTokenType.Colon, [":"] },
                    { RecurlyExTokenType.WhiteSpace, [" ", " ", " "] },
                    { RecurlyExTokenType.EndOfExpression, ["\0"] }
                }
            },
            new()
            {
                Expression = "@every 2 months @at day 10 @on 10:30pm",
                TotalCount = 19,
                TokenCounts = new Dictionary<RecurlyExTokenType, string[]>
                {
                    { RecurlyExTokenType.RuleSpec, ["@every", "@at", "@on"] },
                    { RecurlyExTokenType.TimeUnit, ["months", "day"] },
                    { RecurlyExTokenType.AmPm, ["pm"] },
                    { RecurlyExTokenType.WholeNumber, ["2", "10", "10", "30"] },
                    { RecurlyExTokenType.Colon, [":"] },
                    { RecurlyExTokenType.EndOfExpression, ["\0"] }
                },
            },
            new()
            {
                Expression = "@every 3 years @at JAN-25 @on 10:30am",
                TotalCount = 19,
                TokenCounts = new Dictionary<RecurlyExTokenType, string[]>
                {
                    { RecurlyExTokenType.RuleSpec, ["@every", "@at", "@on"] },
                    { RecurlyExTokenType.TimeUnit, ["years"] },
                    { RecurlyExTokenType.AmPm, ["am"] },
                    { RecurlyExTokenType.WholeNumber, ["3", "25", "10", "30"] },
                    { RecurlyExTokenType.Colon, [":"] },
                    { RecurlyExTokenType.DashOrMinus, ["-"] },
                    { RecurlyExTokenType.MonthWord, ["JAN"] },
                    { RecurlyExTokenType.EndOfExpression, ["\0"] }
                },
            },
            new()
            {
                Expression = "@secondly @between 10:30am and 11:50pm",
                TotalCount = 16,
                TokenCounts = new Dictionary<RecurlyExTokenType, string[]>
                {
                    { RecurlyExTokenType.RuleSpec, ["@secondly", "@between"] },
                    { RecurlyExTokenType.AmPm, ["am", "pm"] },
                    { RecurlyExTokenType.WholeNumber, ["10", "30", "11", "50"] },
                    { RecurlyExTokenType.Colon, [":", ":"] },
                    { RecurlyExTokenType.And, ["and"] }
                },
            },
            new()
            {
                Expression = "@minutely @on days [1, 3, 10, 25]",
                TotalCount = 19,
                TokenCounts = new Dictionary<RecurlyExTokenType, string[]>
                {
                    { RecurlyExTokenType.RuleSpec, ["@minutely", "@on"] },
                    { RecurlyExTokenType.TimeUnit, ["days"] },
                    { RecurlyExTokenType.WholeNumber, ["1", "3", "10", "25"] },
                    { RecurlyExTokenType.Comma, [",", ",", ","] },
                    { RecurlyExTokenType.OpenBrackets, ["["] },
                    { RecurlyExTokenType.CloseBrackets, ["]"] },
                },
            }
        };
        
        return list.Select(x => new object[] { x });
    }

    public class ValidTokenTestData
    {
        public string Expression { get; set; } = string.Empty;
        public int TotalCount { get; set; }
        public IDictionary<RecurlyExTokenType, string[]> TokenCounts { get; set; } = new Dictionary<RecurlyExTokenType, string[]>();
    }
}