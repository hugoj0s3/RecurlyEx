namespace RecurlyEx.Rules;

public abstract class RecurlyExRule
{
    public RecurlyExTimeUnit TimeUnit { get; internal set; }

    public abstract RecurlyExRuleSpec Spec { get; }
    
    public string FullExpression { get; internal set; } = string.Empty;
}