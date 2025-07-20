using NaturalCron.Utils;

namespace NaturalCron.Rules;

public class NaturalCronBetweenMultiplesRule : NaturalCronMatchableRule
{

    public override NaturalCronRuleSpec Spec => NaturalCronRuleSpec.Between;
    
    internal IList<NaturalCronBetweenRule> BetweenRules { get; set; } = new List<NaturalCronBetweenRule>();
    
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

    protected override (int timeToAdvance, NaturalCronTimeUnit timeUnit) DoGetTimeToAdvanceToNextOccurence(DateTime dateTime)
    {
        var results = BetweenRules.Select(x => x.GetTimeToAdvanceToNextOccurence(dateTime))
            .OrderBy(x => DateTimeUtil.GetDurationInSeconds(x.timeUnit, x.timeToAdvance, dateTime.Year, dateTime.Month))
            .ToList();
        
        if (!results.Any())
        {
            return (1, NaturalCronTimeUnit.Second);
        }
        
        return results.First();
    }
}