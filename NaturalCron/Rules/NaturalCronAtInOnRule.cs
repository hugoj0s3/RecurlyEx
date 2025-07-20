using NaturalCron.Utils;

namespace NaturalCron.Rules;

public class NaturalCronAtInOnRule : NaturalCronMatchableRule
{
    internal NaturalCronAtInOnRule()
    {
    }

    public override NaturalCronRuleSpec Spec => NaturalCronRuleSpec.AtInOn;
    
    public string InnerExpression { get; internal set; } = string.Empty;

    protected override bool DoMatch(DateTime dateTime)
    {
        var value = ExpressionUtil.TryGetValueForMatch(TimeUnit, dateTime, InnerExpression);
        return value.HasValue && DateTimeUtil.IsEqual(TimeUnit, dateTime, value.Value);
    }

    protected override (int timeToAdvance, NaturalCronTimeUnit timeUnit) DoGetTimeToAdvanceToNextOccurence(DateTime dateTime)
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

