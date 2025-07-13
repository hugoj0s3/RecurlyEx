using System.Collections;
using RecurlyEx.Rules;
using RecurlyEx.Tokens.Parser.Dtos;

namespace RecurlyEx.Tokens.Parser.ParseSpecStrategies;

internal abstract class ParseRuleSpecStrategy
{
    public abstract RuleSpecType Type { get; }
    public abstract (IList<RecurlyExRule> rules, IList<string> errors) Parse(GroupedRuleSpec groupedRuleSpec);
}