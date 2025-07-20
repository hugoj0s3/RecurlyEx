using System.Linq;
using NaturalCron;
using NaturalCron.Rules;
using FluentAssertions;

namespace NaturalCron.UnitTests;

public class NaturalCronInAtOnInRulesSingleTests
{
    [Theory]
    [InlineData("@in day 5")]
    [InlineData("@in days 5")]
    [InlineData("@on day 5")]
    [InlineData("@on days 5")]
    [InlineData("@at day 5")]
    [InlineData("@at days 5")]
    [InlineData("@in dy 5")]
    [InlineData("@in dys 5")]
    [InlineData("@on dy 5")]
    [InlineData("@on dys 5")]
    [InlineData("@at dy 5")]
    [InlineData("@at dys 5")]
    public void TryParse_InDay5_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.DayRules().Count.Should().Be(1);
        var dayRule = naturalCronExpr.DayRules().FirstOrDefault() as NaturalCronAtInOnRule;
        dayRule.Should().NotBeNull();
        dayRule.InnerExpression.Should().Be("5");
        dayRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Day);
        dayRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.MinuteRules().Count.Should().Be(0);
        naturalCronExpr.SecondRules().Count.Should().Be(0);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        naturalCronExpr.WeekRules().Count.Should().Be(0);
        naturalCronExpr.MonthRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }

    [Theory]
    [InlineData("@in mon")]
    [InlineData("@on mon")]
    [InlineData("@at mon")]
    public void TryParse_OnMonday_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.WeekRules().Count.Should().Be(1);
        var weekRule = naturalCronExpr.WeekRules().FirstOrDefault() as NaturalCronAtInOnRule;
        weekRule.Should().NotBeNull();
        weekRule.InnerExpression.Should().Be("mon");
        weekRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Week);
        weekRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.MinuteRules().Count.Should().Be(0);
        naturalCronExpr.SecondRules().Count.Should().Be(0);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        naturalCronExpr.DayRules().Count.Should().Be(0);
        naturalCronExpr.MonthRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }

    [Theory]
    [InlineData("@in sunday")]
    [InlineData("@on sunday")]
    [InlineData("@at sunday")]
    public void TryParse_OnSunday_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.WeekRules().Count.Should().Be(1);
        var weekRule = naturalCronExpr.WeekRules().FirstOrDefault() as NaturalCronAtInOnRule;
        weekRule.Should().NotBeNull();
        weekRule.InnerExpression.Should().Be("sunday");
        weekRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Week);
        weekRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.MinuteRules().Count.Should().Be(0);
        naturalCronExpr.SecondRules().Count.Should().Be(0);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        naturalCronExpr.DayRules().Count.Should().Be(0);
        naturalCronExpr.MonthRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }

    [Theory]
    [InlineData("@in month 5")]
    [InlineData("@in months 5")]
    [InlineData("@on month 5")]
    [InlineData("@on months 5")]
    [InlineData("@at month 5")]
    [InlineData("@at months 5")]
    [InlineData("@in mth 5")]
    [InlineData("@in mths 5")]
    [InlineData("@on mth 5")]
    [InlineData("@on mths 5")]
    [InlineData("@at mth 5")]
    [InlineData("@at mths 5")]
    public void TryParse_OnMonth5_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.MonthRules().Count.Should().Be(1);
        var monthRule = naturalCronExpr.MonthRules().FirstOrDefault() as NaturalCronAtInOnRule;
        monthRule.Should().NotBeNull();
        monthRule.InnerExpression.Should().Be("5");
        monthRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Month);
        monthRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.MinuteRules().Count.Should().Be(0);
        naturalCronExpr.SecondRules().Count.Should().Be(0);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        naturalCronExpr.DayRules().Count.Should().Be(0);
        naturalCronExpr.WeekRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }

    [Theory]
    [InlineData("@in may")]
    [InlineData("@on may")]
    [InlineData("@at may")]
    public void TryParse_OnMonthMay_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.MonthRules().Count.Should().Be(1);
        var monthRule = naturalCronExpr.MonthRules().FirstOrDefault() as NaturalCronAtInOnRule;
        monthRule.Should().NotBeNull();
        monthRule.InnerExpression.Should().Be("may");
        monthRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Month);
        monthRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.MinuteRules().Count.Should().Be(0);
        naturalCronExpr.SecondRules().Count.Should().Be(0);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        naturalCronExpr.DayRules().Count.Should().Be(0);
        naturalCronExpr.WeekRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }

    [Theory]
    [InlineData("@in june")]
    [InlineData("@on june")]
    [InlineData("@at june")]
    public void TryParse_OnMonthJune_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.MonthRules().Count.Should().Be(1);
        var monthRule = naturalCronExpr.MonthRules().FirstOrDefault() as NaturalCronAtInOnRule;
        monthRule.Should().NotBeNull();
        monthRule.InnerExpression.Should().Be("june");
        monthRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Month);
        monthRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.MinuteRules().Count.Should().Be(0);
        naturalCronExpr.SecondRules().Count.Should().Be(0);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        naturalCronExpr.DayRules().Count.Should().Be(0);
        naturalCronExpr.WeekRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }

    [Theory]
    [InlineData("@in 10:30")]
    [InlineData("@on 10:30")]
    [InlineData("@at 10:30")]
    public void TryParse_At1030_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.MinuteRules().Count.Should().Be(1);
        var minuteRule = naturalCronExpr.MinuteRules().FirstOrDefault() as NaturalCronAtInOnRule;
        minuteRule.Should().NotBeNull();
        minuteRule.InnerExpression.Should().Be("30");
        minuteRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Minute);
        minuteRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.HourRules().Count.Should().Be(1);
        var hourRule = naturalCronExpr.HourRules().FirstOrDefault() as NaturalCronAtInOnRule;
        hourRule.Should().NotBeNull();
        hourRule.InnerExpression.Should().Be("10");
        hourRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Hour);
        hourRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.SecondRules().Count.Should().Be(0);

        naturalCronExpr.DayRules().Count.Should().Be(0);
        naturalCronExpr.MonthRules().Count.Should().Be(0);
        naturalCronExpr.WeekRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }

    [Theory]
    [InlineData("@in 10:30pm")]
    [InlineData("@on 10:30pm")]
    [InlineData("@at 10:30pm")]
    public void TryParse_At1030pm_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.MinuteRules().Count.Should().Be(1);
        var minuteRule = naturalCronExpr.MinuteRules().FirstOrDefault() as NaturalCronAtInOnRule;
        minuteRule.Should().NotBeNull();
        minuteRule.InnerExpression.Should().Be("30");
        minuteRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Minute);
        minuteRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.HourRules().Count.Should().Be(1);
        var hourRule = naturalCronExpr.HourRules().FirstOrDefault() as NaturalCronAtInOnRule;
        hourRule.Should().NotBeNull();
        hourRule.InnerExpression.Should().Be("22");
        hourRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Hour);
        hourRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.SecondRules().Count.Should().Be(0);

        naturalCronExpr.DayRules().Count.Should().Be(0);
        naturalCronExpr.MonthRules().Count.Should().Be(0);
        naturalCronExpr.WeekRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }

    [Theory]
    [InlineData("@in 12:30am")]
    [InlineData("@on 12:30am")]
    [InlineData("@at 12:30am")]
    public void TryParse_At1230am_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.MinuteRules().Count.Should().Be(1);
        var minuteRule = naturalCronExpr.MinuteRules().FirstOrDefault() as NaturalCronAtInOnRule;
        minuteRule.Should().NotBeNull();
        minuteRule.InnerExpression.Should().Be("30");
        minuteRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Minute);
        minuteRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.HourRules().Count.Should().Be(1);
        var hourRule = naturalCronExpr.HourRules().FirstOrDefault() as NaturalCronAtInOnRule;
        hourRule.Should().NotBeNull();
        hourRule.InnerExpression.Should().Be("0");
        hourRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Hour);
        hourRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.SecondRules().Count.Should().Be(0);

        naturalCronExpr.DayRules().Count.Should().Be(0);
        naturalCronExpr.MonthRules().Count.Should().Be(0);
        naturalCronExpr.WeekRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }

    [Theory]
    [InlineData("@in JUN-11")]
    [InlineData("@on JUN-11")]
    [InlineData("@at JUN-11")]
    public void TryParse_AtJun11_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.MinuteRules().Count.Should().Be(0);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        naturalCronExpr.SecondRules().Count.Should().Be(0);

        naturalCronExpr.DayRules().Count.Should().Be(1);
        var dayRule = naturalCronExpr.DayRules().FirstOrDefault() as NaturalCronAtInOnRule;
        dayRule.Should().NotBeNull();
        dayRule.InnerExpression.Should().Be("11");
        dayRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Day);
        dayRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.MonthRules().Count.Should().Be(1);
        var monthRule = naturalCronExpr.MonthRules().FirstOrDefault() as NaturalCronAtInOnRule;
        monthRule.Should().NotBeNull();
        monthRule.InnerExpression.Should().Be("JUN");
        monthRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Month);
        monthRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.WeekRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }

    [Theory]
    [InlineData("@in 2020-11")]
    [InlineData("@on 2020-11")]
    [InlineData("@at 2020-11")]
    public void TryParse_2020Nov_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.MinuteRules().Count.Should().Be(0);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        naturalCronExpr.SecondRules().Count.Should().Be(0);

        naturalCronExpr.DayRules().Count.Should().Be(0);
        naturalCronExpr.MonthRules().Count.Should().Be(1);
        var monthRule = naturalCronExpr.MonthRules().FirstOrDefault() as NaturalCronAtInOnRule;
        monthRule.Should().NotBeNull();
        monthRule.InnerExpression.Should().Be("11");
        monthRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Month);
        monthRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.WeekRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(1);
        var yearRule = naturalCronExpr.YearRules().FirstOrDefault() as NaturalCronAtInOnRule;
        yearRule.Should().NotBeNull();
        yearRule.InnerExpression.Should().Be("2020");
        yearRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Year);
        yearRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);
    }

    [Theory]
    [InlineData("@in 2020-january-11 12:10:10pm")]
    [InlineData("@on 2020-january-11 12:10:10pm")]
    [InlineData("@at 2020-january-11 12:10:10pm")]
    public void TryParse_2020Jan11_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.MinuteRules().Count.Should().Be(1);
        var minuteRule = naturalCronExpr.MinuteRules().FirstOrDefault() as NaturalCronAtInOnRule;
        minuteRule.Should().NotBeNull();
        minuteRule.InnerExpression.Should().Be("10");
        minuteRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Minute);
        minuteRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.HourRules().Count.Should().Be(1);
        var hourRule = naturalCronExpr.HourRules().FirstOrDefault() as NaturalCronAtInOnRule;
        hourRule.Should().NotBeNull();
        hourRule.InnerExpression.Should().Be("12");
        hourRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Hour);
        hourRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.SecondRules().Count.Should().Be(1);
        var secondRule = naturalCronExpr.SecondRules().FirstOrDefault() as NaturalCronAtInOnRule;
        secondRule.Should().NotBeNull();
        secondRule.InnerExpression.Should().Be("10");
        secondRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Second);
        secondRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.DayRules().Count.Should().Be(1);
        var dayRule = naturalCronExpr.DayRules().FirstOrDefault() as NaturalCronAtInOnRule;
        dayRule.Should().NotBeNull();
        dayRule.InnerExpression.Should().Be("11");
        dayRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Day);
        dayRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.MonthRules().Count.Should().Be(1);
        var monthRule = naturalCronExpr.MonthRules().FirstOrDefault() as NaturalCronAtInOnRule;
        monthRule.Should().NotBeNull();
        monthRule.InnerExpression.Should().Be("january");
        monthRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Month);
        monthRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.WeekRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(1);
        var yearRule = naturalCronExpr.YearRules().FirstOrDefault() as NaturalCronAtInOnRule;
        yearRule.Should().NotBeNull();
        yearRule.InnerExpression.Should().Be("2020");
        yearRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Year);
        yearRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);
    }

    [Theory]
    [InlineData("@in LastDayOfTheMonth")]
    [InlineData("@on LastDayOfTheMonth")]
    [InlineData("@at LastDayOfTheMonth")]
    public void TryParse_LastDayOfTheMonth_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.MinuteRules().Count.Should().Be(0);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        naturalCronExpr.SecondRules().Count.Should().Be(0);
        naturalCronExpr.DayRules().Count.Should().Be(1);
        var dayRule = naturalCronExpr.DayRules().FirstOrDefault() as NaturalCronAtInOnRule;
        dayRule.Should().NotBeNull();
        dayRule.InnerExpression.Should().Be("LastDayOfTheMonth");
        dayRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Day);
        dayRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.MonthRules().Count.Should().Be(0);
        naturalCronExpr.WeekRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }

    [Theory]
    [InlineData("@in LastDayOfMonth")]
    [InlineData("@on LastDayOfMonth")]
    [InlineData("@at LastDayOfMonth")]
    public void TryParse_LastDayOfMonth_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.MinuteRules().Count.Should().Be(0);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        naturalCronExpr.SecondRules().Count.Should().Be(0);
        naturalCronExpr.DayRules().Count.Should().Be(1);
        var dayRule = naturalCronExpr.DayRules().FirstOrDefault() as NaturalCronAtInOnRule;
        dayRule.Should().NotBeNull();
        dayRule.InnerExpression.Should().Be("LastDayOfMonth");
        dayRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Day);
        dayRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.MonthRules().Count.Should().Be(0);
        naturalCronExpr.WeekRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }

    [Theory]
    [InlineData("@in LastDay")]
    [InlineData("@on LastDay")]
    [InlineData("@at LastDay")]
    public void TryParse_LastDay_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.MinuteRules().Count.Should().Be(0);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        naturalCronExpr.SecondRules().Count.Should().Be(0);
        naturalCronExpr.DayRules().Count.Should().Be(1);
        var dayRule = naturalCronExpr.DayRules().FirstOrDefault() as NaturalCronAtInOnRule;
        dayRule.Should().NotBeNull();
        dayRule.InnerExpression.Should().Be("LastDay");
        dayRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Day);
        dayRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.MonthRules().Count.Should().Be(0);
        naturalCronExpr.WeekRules().Count.Should().Be(0);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }

    [Theory]
    [InlineData("@in LastTue")]
    [InlineData("@on LastTue")]
    [InlineData("@at LastTue")]
    public void TryParse_LastTue_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.MinuteRules().Count.Should().Be(0);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        naturalCronExpr.SecondRules().Count.Should().Be(0);
        naturalCronExpr.WeekRules().Count.Should().Be(0);

        var dayRule = naturalCronExpr.DayRules().FirstOrDefault() as NaturalCronAtInOnRule;
        dayRule.Should().NotBeNull();
        dayRule.InnerExpression.Should().Be("LastTue");
        dayRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Day);
        dayRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.MonthRules().Count.Should().Be(0);
        naturalCronExpr.DayRules().Count.Should().Be(1);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }

    [Theory]
    [InlineData("@in LastTuesday")]
    [InlineData("@on LastTuesday")]
    [InlineData("@at LastTuesday")]
    public void TryParse_LastTuesday_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.MinuteRules().Count.Should().Be(0);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        naturalCronExpr.SecondRules().Count.Should().Be(0);
        naturalCronExpr.WeekRules().Count.Should().Be(0);
        var dayRule = naturalCronExpr.DayRules().FirstOrDefault() as NaturalCronAtInOnRule;
        dayRule.Should().NotBeNull();
        dayRule.InnerExpression.Should().Be("LastTuesday");
        dayRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Day);
        dayRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.MonthRules().Count.Should().Be(0);
        naturalCronExpr.DayRules().Count.Should().Be(1);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }

    [Theory]
    [InlineData("@in ThirdSat")]
    [InlineData("@on ThirdSat")]
    [InlineData("@at ThirdSat")]
    public void TryParse_ThirdSat_Success(string expression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.MinuteRules().Count.Should().Be(0);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        naturalCronExpr.SecondRules().Count.Should().Be(0);
        naturalCronExpr.WeekRules().Count.Should().Be(0);
        var dayRule = naturalCronExpr.DayRules().FirstOrDefault() as NaturalCronAtInOnRule;
        dayRule.Should().NotBeNull();
        dayRule.InnerExpression.Should().Be("ThirdSat");
        dayRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Day);
        dayRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.MonthRules().Count.Should().Be(0);
        naturalCronExpr.DayRules().Count.Should().Be(1);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }

    [Theory]
    [InlineData("@on 1stMonday", "1stMonday")]
    [InlineData("@on 1stMon", "1stMon")]
    [InlineData("@on FirstMonday", "FirstMonday")]
    [InlineData("@on FirstMon", "FirstMon")]
    [InlineData("@on 2ndMonday", "2ndMonday")]
    [InlineData("@on 2ndMon", "2ndMon")]
    [InlineData("@on SecondMonday", "SecondMonday")]
    [InlineData("@on SecondMon", "SecondMon")]
    [InlineData("@on 3rdMonday", "3rdMonday")]
    [InlineData("@on 3rdMon", "3rdMon")]
    [InlineData("@on ThirdMonday", "ThirdMonday")]
    [InlineData("@on ThirdMon", "ThirdMon")]
    [InlineData("@on 4thMonday", "4thMonday")]
    [InlineData("@on 4thMon", "4thMon")]
    [InlineData("@on FourthMonday", "FourthMonday")]
    [InlineData("@on FourthMon", "FourthMon")]
    [InlineData("@on 5thMonday", "5thMonday")]
    [InlineData("@on 5thMon", "5thMon")]
    [InlineData("@on FifthMonday", "FifthMonday")]
    [InlineData("@on FifthMon", "FifthMon")]
    [InlineData("@on LastMonday", "LastMonday")]
    [InlineData("@on LastMon", "LastMon")]
    [InlineData("@on 1stTuesday", "1stTuesday")]
    [InlineData("@on 1stTue", "1stTue")]
    [InlineData("@on FirstTuesday", "FirstTuesday")]
    [InlineData("@on FirstTue", "FirstTue")]
    [InlineData("@on 2ndTuesday", "2ndTuesday")]
    [InlineData("@on 2ndTue", "2ndTue")]
    [InlineData("@on SecondTuesday", "SecondTuesday")]
    [InlineData("@on SecondTue", "SecondTue")]
    [InlineData("@on 3rdTuesday", "3rdTuesday")]
    [InlineData("@on 3rdTue", "3rdTue")]
    [InlineData("@on ThirdTuesday", "ThirdTuesday")]
    [InlineData("@on ThirdTue", "ThirdTue")]
    [InlineData("@on 4thTuesday", "4thTuesday")]
    [InlineData("@on 4thTue", "4thTue")]
    [InlineData("@on FourthTuesday", "FourthTuesday")]
    [InlineData("@on FourthTue", "FourthTue")]
    [InlineData("@on 5thTuesday", "5thTuesday")]
    [InlineData("@on 5thTue", "5thTue")]
    [InlineData("@on FifthTuesday", "FifthTuesday")]
    [InlineData("@on FifthTue", "FifthTue")]
    [InlineData("@on LastTuesday", "LastTuesday")]
    [InlineData("@on LastTue", "LastTue")]
    [InlineData("@on 1stWednesday", "1stWednesday")]
    [InlineData("@on 1stWed", "1stWed")]
    [InlineData("@on FirstWednesday", "FirstWednesday")]
    [InlineData("@on FirstWed", "FirstWed")]
    [InlineData("@on 2ndWednesday", "2ndWednesday")]
    [InlineData("@on 2ndWed", "2ndWed")]
    [InlineData("@on SecondWednesday", "SecondWednesday")]
    [InlineData("@on SecondWed", "SecondWed")]
    [InlineData("@on 3rdWednesday", "3rdWednesday")]
    [InlineData("@on 3rdWed", "3rdWed")]
    [InlineData("@on ThirdWednesday", "ThirdWednesday")]
    [InlineData("@on ThirdWed", "ThirdWed")]
    [InlineData("@on 4thWednesday", "4thWednesday")]
    [InlineData("@on 4thWed", "4thWed")]
    [InlineData("@on FourthWednesday", "FourthWednesday")]
    [InlineData("@on FourthWed", "FourthWed")]
    [InlineData("@on 5thWednesday", "5thWednesday")]
    [InlineData("@on 5thWed", "5thWed")]
    [InlineData("@on FifthWednesday", "FifthWednesday")]
    [InlineData("@on FifthWed", "FifthWed")]
    [InlineData("@on LastWednesday", "LastWednesday")]
    [InlineData("@on LastWed", "LastWed")]
    [InlineData("@on 1stThursday", "1stThursday")]
    [InlineData("@on 1stThu", "1stThu")]
    [InlineData("@on FirstThursday", "FirstThursday")]
    [InlineData("@on FirstThu", "FirstThu")]
    [InlineData("@on 2ndThursday", "2ndThursday")]
    [InlineData("@on 2ndThu", "2ndThu")]
    [InlineData("@on SecondThursday", "SecondThursday")]
    [InlineData("@on SecondThu", "SecondThu")]
    [InlineData("@on 3rdThursday", "3rdThursday")]
    [InlineData("@on 3rdThu", "3rdThu")]
    [InlineData("@on ThirdThursday", "ThirdThursday")]
    [InlineData("@on ThirdThu", "ThirdThu")]
    [InlineData("@on 4thThursday", "4thThursday")]
    [InlineData("@on 4thThu", "4thThu")]
    [InlineData("@on FourthThursday", "FourthThursday")]
    [InlineData("@on FourthThu", "FourthThu")]
    [InlineData("@on 5thThursday", "5thThursday")]
    [InlineData("@on 5thThu", "5thThu")]
    [InlineData("@on FifthThursday", "FifthThursday")]
    [InlineData("@on FifthThu", "FifthThu")]
    [InlineData("@on LastThursday", "LastThursday")]
    [InlineData("@on LastThu", "LastThu")]
    [InlineData("@on 1stFriday", "1stFriday")]
    [InlineData("@on 1stFri", "1stFri")]
    [InlineData("@on FirstFriday", "FirstFriday")]
    [InlineData("@on FirstFri", "FirstFri")]
    [InlineData("@on 2ndFriday", "2ndFriday")]
    [InlineData("@on 2ndFri", "2ndFri")]
    [InlineData("@on SecondFriday", "SecondFriday")]
    [InlineData("@on SecondFri", "SecondFri")]
    [InlineData("@on 3rdFriday", "3rdFriday")]
    [InlineData("@on 3rdFri", "3rdFri")]
    [InlineData("@on ThirdFriday", "ThirdFriday")]
    [InlineData("@on ThirdFri", "ThirdFri")]
    [InlineData("@on 4thFriday", "4thFriday")]
    [InlineData("@on 4thFri", "4thFri")]
    [InlineData("@on FourthFriday", "FourthFriday")]
    [InlineData("@on FourthFri", "FourthFri")]
    [InlineData("@on 5thFriday", "5thFriday")]
    [InlineData("@on 5thFri", "5thFri")]
    [InlineData("@on FifthFriday", "FifthFriday")]
    [InlineData("@on FifthFri", "FifthFri")]
    [InlineData("@on LastFriday", "LastFriday")]
    [InlineData("@on LastFri", "LastFri")]
    [InlineData("@on 1stSaturday", "1stSaturday")]
    [InlineData("@on 1stSat", "1stSat")]
    [InlineData("@on FirstSaturday", "FirstSaturday")]
    [InlineData("@on FirstSat", "FirstSat")]
    [InlineData("@on 2ndSaturday", "2ndSaturday")]
    [InlineData("@on 2ndSat", "2ndSat")]
    [InlineData("@on SecondSaturday", "SecondSaturday")]
    [InlineData("@on SecondSat", "SecondSat")]
    [InlineData("@on 3rdSaturday", "3rdSaturday")]
    [InlineData("@on 3rdSat", "3rdSat")]
    [InlineData("@on ThirdSaturday", "ThirdSaturday")]
    [InlineData("@on ThirdSat", "ThirdSat")]
    [InlineData("@on 4thSaturday", "4thSaturday")]
    [InlineData("@on 4thSat", "4thSat")]
    [InlineData("@on FourthSaturday", "FourthSaturday")]
    [InlineData("@on FourthSat", "FourthSat")]
    [InlineData("@on 5thSaturday", "5thSaturday")]
    [InlineData("@on 5thSat", "5thSat")]
    [InlineData("@on FifthSaturday", "FifthSaturday")]
    [InlineData("@on FifthSat", "FifthSat")]
    [InlineData("@on LastSaturday", "LastSaturday")]
    [InlineData("@on LastSat", "LastSat")]
    [InlineData("@on 1stSunday", "1stSunday")]
    [InlineData("@on 1stSun", "1stSun")]
    [InlineData("@on FirstSunday", "FirstSunday")]
    [InlineData("@on FirstSun", "FirstSun")]
    [InlineData("@on 2ndSunday", "2ndSunday")]
    [InlineData("@on 2ndSun", "2ndSun")]
    [InlineData("@on SecondSunday", "SecondSunday")]
    [InlineData("@on SecondSun", "SecondSun")]
    [InlineData("@on 3rdSunday", "3rdSunday")]
    [InlineData("@on 3rdSun", "3rdSun")]
    [InlineData("@on ThirdSunday", "ThirdSunday")]
    [InlineData("@on ThirdSun", "ThirdSun")]
    [InlineData("@on 4thSunday", "4thSunday")]
    [InlineData("@on 4thSun", "4thSun")]
    [InlineData("@on FourthSunday", "FourthSunday")]
    [InlineData("@on FourthSun", "FourthSun")]
    [InlineData("@on 5thSunday", "5thSunday")]
    [InlineData("@on 5thSun", "5thSun")]
    [InlineData("@on FifthSunday", "FifthSunday")]
    [InlineData("@on FifthSun", "FifthSun")]
    [InlineData("@on LastSunday", "LastSunday")]
    [InlineData("@on LastSun", "LastSun")]
    public void TryParse_NthWeekDay_Success(string expression, string innerExpression)
    {
        var (naturalCronExpr, errors) = NaturalCronExpr.TryParse(expression);

        errors.Should().BeEmpty();
        naturalCronExpr.Should().NotBeNull();

        naturalCronExpr.MinuteRules().Count.Should().Be(0);
        naturalCronExpr.HourRules().Count.Should().Be(0);
        naturalCronExpr.SecondRules().Count.Should().Be(0);
        naturalCronExpr.WeekRules().Count.Should().Be(0);
        var dayRule = naturalCronExpr.DayRules().FirstOrDefault() as NaturalCronAtInOnRule;
        dayRule.Should().NotBeNull();
        dayRule.InnerExpression.Should().Be(innerExpression);
        dayRule.TimeUnit.Should().Be(NaturalCronTimeUnit.Day);
        dayRule.Spec.Should().Be(NaturalCronRuleSpec.AtInOn);

        naturalCronExpr.MonthRules().Count.Should().Be(0);
        naturalCronExpr.DayRules().Count.Should().Be(1);
        naturalCronExpr.YearRules().Count.Should().Be(0);
    }
}