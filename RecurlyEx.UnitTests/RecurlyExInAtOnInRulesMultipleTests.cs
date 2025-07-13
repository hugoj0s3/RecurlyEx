using RecurlyEx;
using RecurlyEx.Rules;
using FluentAssertions;

namespace RecurlyEx.UnitTests;

public class RecurlyExInAtOnInRulesMultipleTests
{
    [Theory]
    [InlineData("@in days [1,2,3,4,5,6,7]")]
    [InlineData("@on days [1,2,3,4,5,6,7]")]
    [InlineData("@at days [1,2,3,4,5,6,7]")]
    public void TryParse_AtMultipleDays_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.DayRules().Count.Should().Be(1);
        var dayRules = recurlyEx.DayRules().FirstOrDefault() as RecurlyExAtInOnMultiplesRule;
        dayRules.Should().NotBeNull();
        dayRules.DayRules.Select(x => x.InnerExpression).Should().BeEquivalentTo("1", "2", "3", "4", "5", "6", "7");
        dayRules.TimeUnit.Should().Be(RecurlyExTimeUnit.Day);
        dayRules.Spec.Should().Be(RecurlyExRuleSpec.AtInOn);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.SecondRules().Count.Should().Be(0);
        
        recurlyEx.MonthRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@in [JAN, FEB, MAR, APR, MAY, JUN]")]
    [InlineData("@on [JAN, FEB, MAR, APR, MAY, JUN]")]
    [InlineData("@at [JAN, FEB, MAR, APR, MAY, JUN]")]
    public void TryParse_AtMultipleMonths_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.MonthRules().Count.Should().Be(1);
        var monthRule = recurlyEx.MonthRules().FirstOrDefault() as RecurlyExAtInOnMultiplesRule;
        monthRule.Should().NotBeNull();
        monthRule.MonthRules.Select(x => x.InnerExpression).Should().BeEquivalentTo("JAN", "FEB", "MAR", "APR", "MAY", "JUN");
        monthRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Month);
        monthRule.Spec.Should().Be(RecurlyExRuleSpec.AtInOn);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.SecondRules().Count.Should().Be(0);
        
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@in [MONDAY, TUESDAY, WEDNESDAY, THURSDAY, FRIDAY, SATURDAY, SUNDAY]")]
    [InlineData("@on [MONDAY, TUESDAY, WEDNESDAY, THURSDAY, FRIDAY, SATURDAY, SUNDAY]")]
    [InlineData("@at [MONDAY, TUESDAY, WEDNESDAY, THURSDAY, FRIDAY, SATURDAY, SUNDAY]")]
    public void TryParse_AtMultipleWeekdays_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.WeekRules().Count.Should().Be(1);
        var weekRule = recurlyEx.WeekRules().FirstOrDefault() as RecurlyExAtInOnMultiplesRule;
        weekRule.Should().NotBeNull();
        weekRule.WeekRules.Select(x => x.InnerExpression).Should().BeEquivalentTo("MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY", "SUNDAY");
        weekRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Week);
        weekRule.Spec.Should().Be(RecurlyExRuleSpec.AtInOn);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.SecondRules().Count.Should().Be(0);
        
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.MonthRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@in [10:30, 11:30, 12:30]")]
    [InlineData("@on [10:30, 11:30, 12:30]")]
    [InlineData("@at [10:30, 11:30, 12:30]")]
    public void TryParse_AtMultipleTimes_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        var minuteRule = recurlyEx.MinuteRules().FirstOrDefault() as RecurlyExAtInOnMultiplesRule;
        minuteRule.Should().NotBeNull();
        minuteRule.HourRules.Select(x => x.InnerExpression).Should().BeEquivalentTo("10", "11", "12");
        minuteRule.MinuteRules.Select(x => x.InnerExpression).Should().BeEquivalentTo("30", "30", "30");
        minuteRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Minute);
        minuteRule.Spec.Should().Be(RecurlyExRuleSpec.AtInOn);
        
        recurlyEx.MinuteRules().Count.Should().Be(1);
        recurlyEx.HourRules().Count.Should().Be(0);
        
        recurlyEx.SecondRules().Count.Should().Be(0);
        
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.MonthRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
}