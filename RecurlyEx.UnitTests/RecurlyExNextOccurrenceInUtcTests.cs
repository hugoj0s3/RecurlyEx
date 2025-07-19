using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using RecurlyEx;
using FluentAssertions;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace RecurlyEx.UnitTests;

[TestCaseOrderer("RecurlyEx.UnitTests.NumberCaseOrderer", "RecurlyEx.UnitTests")]
public class RecurlyExNextOccurrenceInUtcTests
{
    private readonly ITestOutputHelper testOutputHelper;

    public RecurlyExNextOccurrenceInUtcTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Theory]
    [MemberData(nameof(LoadNextOccurrenceTestCases))]
    public void GetNextOccurrencesInUtc_ValidExpressions_Success(
        string description, 
        string expression, 
        string baseTimeUtcStr,
        string[] expectedDateTimeStrs)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        foreach (var ruleExpression in recurlyEx.Rules.Select(x => x.FullExpression))
        {
            ruleExpression.Should().NotBeNullOrEmpty();
        }

        var baseTimeUtc = DateTime.Parse(baseTimeUtcStr);

        var expectedDateTimeUtc = expectedDateTimeStrs.Where(x => !string.IsNullOrEmpty(x)).Select(x => DateTime.Parse(x)).ToList();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        
        IList<DateTime> nextOccurrences = new List<DateTime>();
        var task = Task.Run(() => nextOccurrences = recurlyEx.TryGetNextOccurrencesInUtc(baseTimeUtc, expectedDateTimeStrs.Length), cts.Token);
        task.Wait(cts.Token);

        nextOccurrences.Should().HaveCount(expectedDateTimeUtc.Count);
        nextOccurrences.Should().Equal(expectedDateTimeUtc);
        
        stopwatch.Stop();
        testOutputHelper.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
        stopwatch.Elapsed.TotalMilliseconds.Should().BeLessThan(2000);
    }
    
    public static IEnumerable<object[]> LoadNextOccurrenceTestCases()
    {
        return TestDataHelper.LoadNextOccurrenceTestCases();
    }
}
