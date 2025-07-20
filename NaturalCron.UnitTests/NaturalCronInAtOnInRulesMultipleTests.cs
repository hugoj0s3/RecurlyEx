using System.Linq;
using NaturalCron;
using NaturalCron.Rules;
using FluentAssertions;

namespace NaturalCron.UnitTests;

public class NaturalCronInAtOnInRulesMultipleTests
{
    [Theory]
    [InlineData("@in days [1,2,3,4,5,6,7]")]
    [InlineData("@on days [1,2,3,4,5,6,7]")]
    [InlineData("@at days [1,2,3,4,5,6,7]")]
    public void TryParse_AtMultipleDays_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);
        
        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();
        
        naturalCronExpr.DayRules().Count.Should().Be(1);
        var dayRules = naturalCronExpr.DayRules().FirstOrDefault() as NaturalCronAtInOnMultiplesRule;
        dayRules.Should().NotBeNull();
        dayRules.DayRules.Select(x => x.InnerExpression).Should().BeEquivalentTo("1", "2", "3", "4", "5", "6", "7");
        dayRules.TimeUnit.Should().Be(NaturalCronTimeUnit.Day);
        dayRules.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);
        
        naturalCronExpr.MinuteRules().Count.Should().Be(0);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        naturalCronExpr.SecondRules().Count.Should().Be(0);
        
        naturalCronExpr.MonthRules().Count.Should().Be(0);
        naturalCronExpr.WeekRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@in [JAN, FEB, MAR, APR, MAY, JUN]")]
    [InlineData("@on [JAN, FEB, MAR, APR, MAY, JUN]")]
    [InlineData("@at [JAN, FEB, MAR, APR, MAY, JUN]")]
    public void TryParse_AtMultipleMonths_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);
        
        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();
        
        naturalCronExpr.MonthRules().Count.Should().Be(1);
        var monthRule = naturalCronExpr.MonthRules().FirstOrDefault() as NaturalCronAtInOnMultiplesRule;
        monthRule.Should().NotBeNull();
        monthRule.MonthRules.Select(x => x.InnerExpression).Should().BeEquivalentTo("JAN", "FEB", "MAR", "APR", "MAY", "JUN");
        monthRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Month);
        monthRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);
        
        naturalCronExpr.MinuteRules().Count.Should().Be(0);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        naturalCronExpr.SecondRules().Count.Should().Be(0);
        
        naturalCronExpr.DayRules().Count.Should().Be(0);
        naturalCronExpr.WeekRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@in [MONDAY, TUESDAY, WEDNESDAY, THURSDAY, FRIDAY, SATURDAY, SUNDAY]")]
    [InlineData("@on [MONDAY, TUESDAY, WEDNESDAY, THURSDAY, FRIDAY, SATURDAY, SUNDAY]")]
    [InlineData("@at [MONDAY, TUESDAY, WEDNESDAY, THURSDAY, FRIDAY, SATURDAY, SUNDAY]")]
    public void TryParse_AtMultipleWeekdays_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);
        
        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();
        
        naturalCronExpr.WeekRules().Count.Should().Be(1);
        var weekRule = naturalCronExpr.WeekRules().FirstOrDefault() as NaturalCronAtInOnMultiplesRule;
        weekRule.Should().NotBeNull();
        weekRule.WeekRules.Select(x => x.InnerExpression).Should().BeEquivalentTo("MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY", "SUNDAY");
        weekRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Week);
        weekRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);
        
        naturalCronExpr.MinuteRules().Count.Should().Be(0);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        naturalCronExpr.SecondRules().Count.Should().Be(0);
        
        naturalCronExpr.DayRules().Count.Should().Be(0);
        naturalCronExpr.MonthRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@in [10:30, 11:30, 12:30]")]
    [InlineData("@on [10:30, 11:30, 12:30]")]
    [InlineData("@at [10:30, 11:30, 12:30]")]
    public void TryParse_AtMultipleTimes_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);
        
        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();
        
        var minuteRule = naturalCronExpr.MinuteRules().FirstOrDefault() as NaturalCronAtInOnMultiplesRule;
        minuteRule.Should().NotBeNull();
        minuteRule.HourRules.Select(x => x.InnerExpression).Should().BeEquivalentTo("10", "11", "12");
        minuteRule.MinuteRules.Select(x => x.InnerExpression).Should().BeEquivalentTo("30", "30", "30");
        minuteRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Minute);
        minuteRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);
        
        naturalCronExpr.MinuteRules().Count.Should().Be(1);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        
        naturalCronExpr.SecondRules().Count.Should().Be(0);
        
        naturalCronExpr.DayRules().Count.Should().Be(0);
        naturalCronExpr.MonthRules().Count.Should().Be(0);
        naturalCronExpr.WeekRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }
}