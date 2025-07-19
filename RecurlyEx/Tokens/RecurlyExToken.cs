using RecurlyEx.Utils;

namespace RecurlyEx.Tokens;

public class RecurlyExToken
{
    public RecurlyExTokenType Type { get; internal set; }
    public string Value { get; internal set; } = string.Empty;
    public bool IsValid { get; internal set; }
    public string Error { get; internal set; } = string.Empty;

    public int Line { get; internal set; }

    public int StartCol { get; internal set; }
    public int EndCol { get; internal set; }

    public string GetErrorWithLinePosition()
    {
        return $"{Line}:{StartCol}-{EndCol} {Error}";
    }
    
    public bool IsDayOrdinal()
    {
        return Type == RecurlyExTokenType.TimeUnit && KeywordsConstants.DayOrdinals.ToUpper().Contains(Value.ToUpper());
    }
}