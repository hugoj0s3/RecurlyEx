namespace RecurlyEx.Tokens.Parser.Dtos;

internal class GroupedRuleSpec
{
    public RuleSpecType Type { get; set; }
    public List<RecurlyExToken> Tokens { get; set; } = new List<RecurlyExToken>();
}