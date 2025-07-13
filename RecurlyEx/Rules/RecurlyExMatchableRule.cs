using RecurlyEx.Utils;

namespace RecurlyEx.Rules;

public abstract class RecurlyExMatchableRule : RecurlyExRule
{
    private DateTime? lastMatchCall = null;
    private bool? lastMatchCallResult = null;
    
    private DateTime? lastGetTimeToAdvanceToNextOccurenceCall = null;
    private int? lastTimeToAdvanceToNextOccurence = 0;
    private RecurlyExTimeUnit? lastTimeUnitToAdvanceToNextOccurence = RecurlyExTimeUnit.Second;
    
    public bool Match(DateTime dateTime)
    {
        if (lastMatchCall == dateTime && lastMatchCallResult.HasValue)
        {
            return lastMatchCallResult.Value;
        }
        
        
        lastMatchCallResult = DoMatch(dateTime);
        lastMatchCall = dateTime;

        return lastMatchCallResult.Value;
    }
    
    public (int timeToAdvance, RecurlyExTimeUnit timeUnit) GetTimeToAdvanceToNextOccurence(DateTime dateTime)
    {
        if (lastGetTimeToAdvanceToNextOccurenceCall == dateTime && 
            lastTimeToAdvanceToNextOccurence.HasValue && 
            lastTimeUnitToAdvanceToNextOccurence.HasValue)
        {
            return (lastTimeToAdvanceToNextOccurence.Value, lastTimeUnitToAdvanceToNextOccurence.Value);
        }
        
        (lastTimeToAdvanceToNextOccurence, lastTimeUnitToAdvanceToNextOccurence) = DoGetTimeToAdvanceToNextOccurence(dateTime);
        lastGetTimeToAdvanceToNextOccurenceCall = dateTime;
        return (lastTimeToAdvanceToNextOccurence.Value, lastTimeUnitToAdvanceToNextOccurence.Value);
    }
    
    protected abstract bool DoMatch(DateTime dateTime);
    protected virtual (int timeToAdvance, RecurlyExTimeUnit timeUnit) DoGetTimeToAdvanceToNextOccurence(DateTime dateTime)
    {
        return GetMinSafeTimeToAdvance(dateTime);
    }
    
    internal (int, RecurlyExTimeUnit) GetMinSafeTimeToAdvance(DateTime dateTime)
    {
        if (this.TimeUnit == RecurlyExTimeUnit.Second)
        {
            return (1, RecurlyExTimeUnit.Second);
        }
        
        var timeUnitToAdvance = this.TimeUnit - 1;
        if (timeUnitToAdvance == RecurlyExTimeUnit.Week)
        {
            timeUnitToAdvance = RecurlyExTimeUnit.Day;
        }
        
        var timeUnitValueToAdvance = ExpressionUtil.GetMaxValueByTimeUnit(timeUnitToAdvance, dateTime) - DateTimeUtil.GetPartValue(timeUnitToAdvance, dateTime) - 1;
        if (timeUnitValueToAdvance <= 0)
        {
            return (1, timeUnitToAdvance);
        }
            
        return (timeUnitValueToAdvance, timeUnitToAdvance);
    }
}