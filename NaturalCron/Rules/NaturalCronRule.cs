namespace NaturalCron.Rules;

public abstract class NaturalCronRule
{
    public NaturalCronTimeUnit TimeUnit { get; internal set; }

    public abstract NaturalCronRuleSpec Spec { get; }
    
    public string FullExpression { get; internal set; } = string.Empty;
}