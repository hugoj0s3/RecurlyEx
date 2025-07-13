using RecurlyEx.Utils;

namespace RecurlyEx.Rules;

public class RecurlyExAtInOnRule : RecurlyExMatchableRule
{
    internal RecurlyExAtInOnRule()
    {
    }

    public override RecurlyExRuleSpec Spec => RecurlyExRuleSpec.AtInOn;
    
    public string InnerExpression { get; internal set; } = string.Empty;

    protected override bool DoMatch(DateTime dateTime)
    {
        var value = ExpressionUtil.TryGetValueForMatch(TimeUnit, dateTime, InnerExpression);
        return value.HasValue && DateTimeUtil.IsEqual(TimeUnit, dateTime, value.Value);
    }

    protected override (int timeToAdvance, RecurlyExTimeUnit timeUnit) DoGetTimeToAdvanceToNextOccurence(DateTime dateTime)
    {
        var targetValueToAdvance = ExpressionUtil.TryGetValueForMatch(this.TimeUnit, dateTime, this.InnerExpression);
        var currentValue = DateTimeUtil.GetPartValue(this.TimeUnit, dateTime);
            
        var timeToAdvance = (targetValueToAdvance ?? 0) - currentValue - 1;
        if (timeToAdvance <= 0)
        {
            return (1, this.TimeUnit);
        }
        
        return (timeToAdvance, this.TimeUnit);
    }
}

