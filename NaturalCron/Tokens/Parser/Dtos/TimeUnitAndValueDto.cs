namespace NaturalCron.Tokens.Parser.Dtos;

internal class TimeUnitAndValueDto
{
    public NaturalCronTimeUnitAndUnknown TimeUnit { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public bool IsValid { get; set; }
}