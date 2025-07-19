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
    [MemberData(nameof(LoadNextOccurrenceTestCases))]
    public void GetNextOccurrences_ValidExpressions_ReturnsSystemTimeZone(
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

        // Convert UTC base time to system timezone
        var baseTimeUtc = DateTime.Parse(baseTimeUtcStr);
        var baseTimeLocal = TimeZoneInfo.ConvertTimeFromUtc(baseTimeUtc, TimeZoneInfo.Local);

        var expectedDateTimeUtc = expectedDateTimeStrs.Where(x => !string.IsNullOrEmpty(x)).Select(x => DateTime.Parse(x)).ToList();
        var expectedDateTimeLocal = expectedDateTimeUtc.Select(utc => TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.Local)).ToList();
        
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        
        IList<DateTime> nextOccurrences = new List<DateTime>();
        var task = Task.Run(() => nextOccurrences = recurlyEx.TryGetNextOccurrences(baseTimeLocal, expectedDateTimeStrs.Length), cts.Token);
        task.Wait(cts.Token);

        // Verify count
        nextOccurrences.Should().HaveCount(expectedDateTimeLocal.Count);
        
        // Verify all results are in local timezone
        foreach (var occurrence in nextOccurrences)
        {
            occurrence.Kind.Should().Be(DateTimeKind.Local);
        }
        
        // Verify the actual times match expected local times
        nextOccurrences.Should().Equal(expectedDateTimeLocal);
        
        stopwatch.Stop();
        testOutputHelper.WriteLine($"System TimeZone: {TimeZoneInfo.Local.Id}");
        testOutputHelper.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
        stopwatch.Elapsed.TotalMilliseconds.Should().BeLessThan(2000);
    }

    [Theory]
    [MemberData(nameof(LoadNextOccurrenceTestCases))]
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
        
        // Result should be in system timezone
        result.Kind.Should().Be(DateTimeKind.Local);
        
        // Verify it represents when 9:00 AM Tokyo occurs in system timezone
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

    public static IEnumerable<object[]> LoadNextOccurrenceTestCases()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "RecurlyEx.UnitTests.GetNextOcurrencesTestCases.json";
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new Exception($"Resource {resourceName} not found");
        }
        
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        var cases = JsonSerializer.Deserialize<List<NextOccurrenceTestCase>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });
        
        if (cases == null)
        {
            throw new Exception($"Failed to deserialize {resourceName}");
        }
        
        // Test both with and without @ symbols
        var testCasesWithoutAmpersat = cases
            .Select(x => new object[] { x.Description.Replace("@", " ") + " Without ampersat", x.Expression.Replace("@", " "), x.BaseTimeUtcStr, x.ExpectedDateTimeStrs });
        var testCasesWithAmpersat = cases
            .Select(x => new object[] { x.Description + " With ampersat", x.Expression, x.BaseTimeUtcStr, x.ExpectedDateTimeStrs });
        
        return testCasesWithoutAmpersat.Concat(testCasesWithAmpersat);
    }
    
    private class NextOccurrenceTestCase
    {
        public string Description { get; set; } = string.Empty;
        public string Expression { get; set; } = string.Empty;
        public string BaseTimeUtcStr { get; set; } = string.Empty;
        public string[] ExpectedDateTimeStrs { get; set; } = Array.Empty<string>();
    }
}
