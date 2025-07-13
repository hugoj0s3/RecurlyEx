namespace RecurlyEx.Tokens.Parser.Dtos;

internal class TimeUnitAndValueDto
{
    public RecurlyExTimeUnitAndUnknown TimeUnit { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public bool IsValid { get; set; }
}