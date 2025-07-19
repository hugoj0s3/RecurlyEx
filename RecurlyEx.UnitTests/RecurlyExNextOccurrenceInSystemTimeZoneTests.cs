using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using RecurlyEx;
using FluentAssertions;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace RecurlyEx.UnitTests;

[TestCaseOrderer("RecurlyEx.UnitTests.NumberCaseOrderer", "RecurlyEx.UnitTests")]
public class RecurlyExNextOccurrenceInSystemTimeZoneTests
{
    private readonly ITestOutputHelper testOutputHelper;

    public RecurlyExNextOccurrenceInSystemTimeZoneTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Theory]
    [MemberData(nameof(LoadNextOccurrenceTestCasesWithTimeZone))]
    public void GetNextOccurrences_ConsistentWithUtcMethods(
        string description, 
        string expression, 
        string baseTimeUtcStr,
        string[] expectedDateTimeStrs)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        var baseTimeUtc = DateTime.Parse(baseTimeUtcStr);
        var baseTimeLocal = TimeZoneInfo.ConvertTimeFromUtc(baseTimeUtc, TimeZoneInfo.Local);
        
        var expectedCount = expectedDateTimeStrs.Where(x => !string.IsNullOrEmpty(x)).Count();
        
        // Get results from both methods
        var utcResults = recurlyEx.TryGetNextOccurrencesInUtc(baseTimeUtc, expectedCount);
        var localResults = recurlyEx.TryGetNextOccurrences(baseTimeLocal, expectedCount);
        
        // Convert UTC results to local for comparison
        var utcResultsConvertedToLocal = utcResults.Select(utc => TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.Local)).ToList();
        
        // Both methods should produce equivalent results
        localResults.Should().Equal(utcResultsConvertedToLocal);
        
        testOutputHelper.WriteLine($"UTC results converted to local: {string.Join(", ", utcResultsConvertedToLocal.Select(d => d.ToString("F")))}");
        testOutputHelper.WriteLine($"Local results: {string.Join(", ", localResults.Select(d => d.ToString("F")))}");
    }

    [Fact]
    public void GetNextOccurrence_SingleResult_ReturnsSystemTimeZone()
    {
        var recurlyEx = RecurlyEx.Parse("daily at 14:00");
        var baseTime = new DateTime(2024, 1, 1, 10, 0, 0);
        
        var result = recurlyEx.GetNextOccurrence(baseTime);
        
        result.Hour.Should().Be(14);
        
        testOutputHelper.WriteLine($"System TimeZone: {TimeZoneInfo.Local.Id}");
        testOutputHelper.WriteLine($"Result: {result:F}");
    }

    [Fact]
    public void TryGetNextOccurrence_NoResult_ReturnsNull()
    {
        var recurlyEx = RecurlyEx.Parse("daily at 14:00");
        var baseTime = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Local);
        var maxLookahead = baseTime.AddHours(1); // Not enough time to find 14:00
        
        var result = recurlyEx.TryGetNextOccurrence(baseTime, maxLookahead);
        
        result.Should().BeNull();
    }

    [Fact]
    public void GetNextOccurrence_WithTokyoTimezone_CalculatesInTokyoReturnsLocal()
    {
        var recurlyEx = RecurlyEx.Parse("daily at 09:00 tz Asia/Tokyo");
        var baseTime = new DateTime(2024, 1, 1, 15, 0, 0, DateTimeKind.Local);
        
        var result = recurlyEx.GetNextOccurrence(baseTime);
        
        // Verify it reprents when 9:00 AM Tokyo occurs in system timezone
        var tokyoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");
        var tokyo9am = new DateTime(2024, 1, 2, 9, 0, 0); // Next day 9 AM Tokyo
        var expectedLocalTime = TimeZoneInfo.ConvertTime(tokyo9am, tokyoTimeZone, TimeZoneInfo.Local);
        
        result.Should().Be(expectedLocalTime);
        
        testOutputHelper.WriteLine($"System TimeZone: {TimeZoneInfo.Local.Id}");
        testOutputHelper.WriteLine($"Tokyo 9:00 AM = {expectedLocalTime:F} in system timezone");
        testOutputHelper.WriteLine($"Actual result: {result:F}");
    }

    [Fact]
    public void GetNextOccurrences_ThrowsWhenNotEnoughOccurrences()
    {
        var recurlyEx = RecurlyEx.Parse("daily at 14:00");
        var baseTime = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Local);
        var maxLookahead = baseTime.AddHours(1); // Not enough time to find any occurrences
        
        var action = () => recurlyEx.GetNextOccurrences(baseTime, 5, maxLookahead);
        
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(action);
    }

    public static IEnumerable<object[]> LoadNextOccurrenceTestCasesWithTimeZone()
    {
        return TestDataHelper.LoadNextOccurrenceTestCases(x => x.Expression.Contains("@tz") || x.Expression.Contains("@timezone"));
    }
}
