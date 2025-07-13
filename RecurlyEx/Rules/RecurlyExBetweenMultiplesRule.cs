using RecurlyEx.Utils;

namespace RecurlyEx.Rules;

public class RecurlyExBetweenMultiplesRule : RecurlyExMatchableRule
{

    public override RecurlyExRuleSpec Spec => RecurlyExRuleSpec.Between;
    
    internal IList<RecurlyExBetweenRule> BetweenRules { get; set; } = new List<RecurlyExBetweenRule>();
    
    protected override bool DoMatch(DateTime dateTime)
    {
        foreach (var betweenRule in BetweenRules)
        {
            if (betweenRule.Match(dateTime))
            {
                return true;
            }
        }
        
        return false;
    }

    protected override (int timeToAdvance, RecurlyExTimeUnit timeUnit) DoGetTimeToAdvanceToNextOccurence(DateTime dateTime)
    {
        var results = BetweenRules.Select(x => x.GetTimeToAdvanceToNextOccurence(dateTime))
            .OrderBy(x => DateTimeUtil.GetDurationInSeconds(x.timeUnit, x.timeToAdvance, dateTime.Year, dateTime.Month))
            .ToList();
        
        if (!results.Any())
        {
            return (1, RecurlyExTimeUnit.Second);
        }
        
        return results.First();
    }
}