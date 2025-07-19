using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using RecurlyEx;
using FluentAssertions;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace RecurlyEx.UnitTests;

[TestCaseOrderer("RecurlyEx.UnitTests.NumberCaseOrderer", "RecurlyEx.UnitTests")]
public class RecurlyExNextOccurrenceTests
{
    private readonly ITestOutputHelper testOutputHelper;

    public RecurlyExNextOccurrenceTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Theory]
    [MemberData(nameof(LoadNextOccurrenceTestCases))]
    public void GetNextOccurrences_ValidExpressions_Success(
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
        
        return cases
            .Select(x => new object[] { x.Description, x.Expression, x.BaseTimeUtcStr, x.ExpectedDateTimeStrs });
    }
    
    private class NextOccurrenceTestCase
    {
        public string Description { get; set; } = string.Empty;
        public string Expression { get; set; } = string.Empty;
        public string BaseTimeUtcStr { get; set; } = string.Empty;
        public string[] ExpectedDateTimeStrs { get; set; } = Array.Empty<string>();
    }
}
