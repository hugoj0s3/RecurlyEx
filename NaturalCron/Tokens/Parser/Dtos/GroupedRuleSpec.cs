namespace NaturalCron.Tokens.Parser.Dtos;

internal class GroupedRuleSpec
{
    public RuleSpecType Type { get; set; }
    public List<NaturalCronToken> Tokens { get; set; } = new List<NaturalCronToken>();
}