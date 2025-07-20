using System.Collections;
using NaturalCron.Rules;
using NaturalCron.Tokens.Parser.Dtos;

namespace NaturalCron.Tokens.Parser.ParseSpecStrategies;

internal abstract class ParseRuleSpecStrategy
{
    public abstract RuleSpecType Type { get; }
    public abstract (IList<NaturalCronRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec);
}