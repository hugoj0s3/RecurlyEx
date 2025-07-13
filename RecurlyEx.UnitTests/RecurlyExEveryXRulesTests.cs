using RecurlyEx;
using RecurlyEx.Rules;
using FluentAssertions;

namespace RecurlyEx.UnitTests;

public class RecurlyExEveryXRulesTests
{
    [Theory]
    [InlineData("@every 2 seconds")]
    [InlineData("@every 2 second")]
    [InlineData("@every 2 sec")]
    [InlineData("@every 2 secs")]
    public void TryParse_Every2Second_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.SecondRules().Count.Should().Be(1);
        var secondRule = recurlyEx.SecondRules().FirstOrDefault() as RecurlyExEveryXRule;
        secondRule.Should().NotBeNull();
        secondRule.Value.Should().Be(2);
        secondRule.AnchoredValue.Should().BeNull();
        secondRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Second);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.MonthRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@every 2 seconds BasedOn 10")]
    [InlineData("@every 2 seconds BasedAt 10")]
    [InlineData("@every 2 seconds BasedIn 10")]
    [InlineData("@every 2 seconds AnchoredIn 10")]
    [InlineData("@every 2 seconds AnchoredAt 10")]
    [InlineData("@every 2 seconds AnchoredOn 10")]
    public void TryParse_Every2SecondFrom10Seconds_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.SecondRules().Count.Should().Be(1);
        var secondRule = recurlyEx.SecondRules().FirstOrDefault() as RecurlyExEveryXRule;
        secondRule.Should().NotBeNull();
        secondRule.Value.Should().Be(2);
        secondRule.AnchoredValue.Should().Be("10");
        secondRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Second);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.MonthRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@every 3 minutes ")]
    [InlineData("@every 3 minute")]
    [InlineData("@every 3 min")]
    [InlineData("@every 3 mins")]
    public void TryParse_Every3Minutes_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.MinuteRules().Count.Should().Be(1);
        var minuteRule = recurlyEx.MinuteRules().FirstOrDefault() as RecurlyExEveryXRule;
        minuteRule.Should().NotBeNull();
        minuteRule.Value.Should().Be(3);
        minuteRule.AnchoredValue.Should().BeNull();
        minuteRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Minute);
        
        recurlyEx.SecondRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.MonthRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@every 3 minutes AnchoredOn 20")]
    [InlineData("@every 3 minutes BasedOn 20")]
    [InlineData("@every 3 minutes BasedAt 20")]
    [InlineData("@every 3 minutes BasedIn 20")]
    public void TryParse_Every3MinutesFrom10Minutes_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.MinuteRules().Count.Should().Be(1);
        var minuteRule = recurlyEx.MinuteRules().FirstOrDefault() as RecurlyExEveryXRule;
        minuteRule.Should().NotBeNull();
        minuteRule.Value.Should().Be(3);
        minuteRule.AnchoredValue.Should().Be("20");
        minuteRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Minute);
        
        recurlyEx.SecondRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.MonthRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@every 3 hours ")]
    [InlineData("@every 3 hour")]
    [InlineData("@every 3 hr")]
    [InlineData("@every 3 hrs")]
    public void TryParse_Every3Hours_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.HourRules().Count.Should().Be(1);
        var hourRule = recurlyEx.HourRules().FirstOrDefault() as RecurlyExEveryXRule;
        hourRule.Should().NotBeNull();
        hourRule.Value.Should().Be(3);
        hourRule.AnchoredValue.Should().BeNull();
        hourRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Hour);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.SecondRules().Count.Should().Be(0);
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.MonthRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@every 3 hours AnchoredOn 20")]
    [InlineData("@every 3 hours BasedOn 20")]
    [InlineData("@every 3 hours BasedAt 20")]
    [InlineData("@every 3 hours BasedIn 20")]
    public void TryParse_Every3HoursFrom10Hours_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.HourRules().Count.Should().Be(1);
        var hourRule = recurlyEx.HourRules().FirstOrDefault() as RecurlyExEveryXRule;
        hourRule.Should().NotBeNull();
        hourRule.Value.Should().Be(3);
        hourRule.AnchoredValue.Should().Be("20");
        hourRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Hour);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.SecondRules().Count.Should().Be(0);
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.MonthRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@every 3 days ")]
    [InlineData("@every 3 day")]
    [InlineData("@every 3 dy")]
    [InlineData("@every 3 dys")]
    public void TryParse_Every3Days_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.DayRules().Count.Should().Be(1);
        var dayRule = recurlyEx.DayRules().FirstOrDefault() as RecurlyExEveryXRule;
        dayRule.Should().NotBeNull();
        dayRule.Value.Should().Be(3);
        dayRule.AnchoredValue.Should().BeNull();
        dayRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Day);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.SecondRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.MonthRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@every 3 days AnchoredOn 20")]
    [InlineData("@every 3 days BasedOn 20")]
    [InlineData("@every 3 days BasedAt 20")]
    [InlineData("@every 3 days BasedIn 20")]
    public void TryParse_Every3DaysFrom10Days_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.DayRules().Count.Should().Be(1);
        var dayRule = recurlyEx.DayRules().FirstOrDefault() as RecurlyExEveryXRule;
        dayRule.Should().NotBeNull();
        dayRule.Value.Should().Be(3);
        dayRule.AnchoredValue.Should().Be("20");
        dayRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Day);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.SecondRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.MonthRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@every 3 months ")]
    [InlineData("@every 3 month")]
    [InlineData("@every 3 mth")]
    [InlineData("@every 3 mths")]
    public void TryParse_Every3Months_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.MonthRules().Count.Should().Be(1);
        var monthRule = recurlyEx.MonthRules().FirstOrDefault() as RecurlyExEveryXRule;
        monthRule.Should().NotBeNull();
        monthRule.Value.Should().Be(3);
        monthRule.AnchoredValue.Should().BeNull();
        monthRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Month);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.SecondRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@every 3 months AnchoredOn 5")]
    [InlineData("@every 3 months BasedOn 5")]
    [InlineData("@every 3 months BasedAt 5")]
    [InlineData("@every 3 months BasedIn 5")]
    public void TryParse_Every3MonthsFrom10Months_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.MonthRules().Count.Should().Be(1);
        var monthRule = recurlyEx.MonthRules().FirstOrDefault() as RecurlyExEveryXRule;
        monthRule.Should().NotBeNull();
        monthRule.Value.Should().Be(3);
        monthRule.AnchoredValue.Should().Be("5");
        monthRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Month);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.SecondRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@every 3 months AnchoredOn Feb")]
    [InlineData("@every 3 months BasedOn Feb")]
    [InlineData("@every 3 months BasedAt Feb")]
    [InlineData("@every 3 months BasedIn Feb")]
    public void TryParse_Every3MonthsFromFeb_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.MonthRules().Count.Should().Be(1);
        var monthRule = recurlyEx.MonthRules().FirstOrDefault() as RecurlyExEveryXRule;
        monthRule.Should().NotBeNull();
        monthRule.Value.Should().Be(3);
        monthRule.AnchoredValue.Should().Be("Feb");
        monthRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Month);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.SecondRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@every 3 years ")]
    [InlineData("@every 3 year")]
    [InlineData("@every 3 yr")]
    [InlineData("@every 3 yrs")]
    public void TryParse_Every3Years_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.YearRules().Count.Should().Be(1);
        var yearRule = recurlyEx.YearRules().FirstOrDefault() as RecurlyExEveryXRule;
        yearRule.Should().NotBeNull();
        yearRule.Value.Should().Be(3);
        yearRule.AnchoredValue.Should().BeNull();
        yearRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Year);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.SecondRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.MonthRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@every 3 years AnchoredOn 5")]
    [InlineData("@every 3 years BasedOn 5")]
    [InlineData("@every 3 years BasedAt 5")]
    [InlineData("@every 3 years BasedIn 5")]
    public void TryParse_Every3YearsFrom10Years_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.YearRules().Count.Should().Be(1);
        var yearRule = recurlyEx.YearRules().FirstOrDefault() as RecurlyExEveryXRule;
        yearRule.Should().NotBeNull();
        yearRule.Value.Should().Be(3);
        yearRule.AnchoredValue.Should().Be("5");
        yearRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Year);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.SecondRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.MonthRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@every week AnchoredOn Monday")]
    [InlineData("@every week BasedOn Monday")]
    [InlineData("@every week BasedAt Monday")]
    [InlineData("@every week BasedIn Monday")]
    public void TryParse_EveryWeekFromMonday_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.WeekRules().Count.Should().Be(1);
        var weekRule = recurlyEx.WeekRules().FirstOrDefault() as RecurlyExEveryXRule;
        weekRule.Should().NotBeNull();
        weekRule.Value.Should().Be(1);
        weekRule.AnchoredValue.Should().Be("Monday");
        weekRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Week);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.SecondRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
        recurlyEx.MonthRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@monthly AnchoredOn apr")]
    [InlineData("@monthly BasedOn apr")]
    [InlineData("@monthly BasedAt apr")]
    [InlineData("@monthly BasedIn apr")]
    public void TryParse_MonthlyFromApril_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.MonthRules().Count.Should().Be(1);
        var monthRule = recurlyEx.MonthRules().FirstOrDefault() as RecurlyExEveryXRule;
        monthRule.Should().NotBeNull();
        monthRule.Value.Should().Be(1);
        monthRule.AnchoredValue.Should().Be("apr");
        monthRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Month);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.SecondRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }

    [Theory]
    [InlineData("@daily AnchoredOn 5")]
    [InlineData("@daily BasedOn 5")]
    [InlineData("@daily BasedAt 5")]
    [InlineData("@daily BasedIn 5")]
    public void TryParse_DailyFrom5_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);

        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.DayRules().Count.Should().Be(1);
        var dayRule = recurlyEx.DayRules().FirstOrDefault() as RecurlyExEveryXRule;
        dayRule.Should().NotBeNull();
        dayRule.Value.Should().Be(1);
        dayRule.AnchoredValue.Should().Be("5");
        dayRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Day);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.SecondRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.MonthRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@hourly AnchoredOn 5am")]
    [InlineData("@hourly BasedOn 5am")]
    [InlineData("@hourly BasedAt 5am")]
    [InlineData("@hourly BasedIn 5am")]
    public void TryParse_HourlyFrom5_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.HourRules().Count.Should().Be(1);
        var hourRule = recurlyEx.HourRules().FirstOrDefault() as RecurlyExEveryXRule;
        hourRule.Should().NotBeNull();
        hourRule.Value.Should().Be(1);
        hourRule.AnchoredValue.Should().Be("5am");
        hourRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Hour);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.SecondRules().Count.Should().Be(0);
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.MonthRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@minutely AnchoredOn 5")]
    [InlineData("@minutely BasedOn 5")]
    [InlineData("@minutely BasedAt 5")]
    [InlineData("@minutely BasedIn 5")]
    public void TryParse_MinutelyFrom5_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.MinuteRules().Count.Should().Be(1);
        var minuteRule = recurlyEx.MinuteRules().FirstOrDefault() as RecurlyExEveryXRule;
        minuteRule.Should().NotBeNull();
        minuteRule.Value.Should().Be(1);
        minuteRule.AnchoredValue.Should().Be("5");
        minuteRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Minute);
        
        recurlyEx.SecondRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.MonthRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@yearly AnchoredOn 2020")]
    [InlineData("@yearly BasedOn 2020")]
    [InlineData("@yearly BasedAt 2020")]
    [InlineData("@yearly BasedIn 2020")]
    public void TryParse_YearlyFrom2020_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.YearRules().Count.Should().Be(1);
        var yearRule = recurlyEx.YearRules().FirstOrDefault() as RecurlyExEveryXRule;
        yearRule.Should().NotBeNull();
        yearRule.Value.Should().Be(1);
        yearRule.AnchoredValue.Should().Be("2020");
        yearRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Year);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.SecondRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.WeekRules().Count.Should().Be(0);
        recurlyEx.MonthRules().Count.Should().Be(0);
    }
    
    [Theory]
    [InlineData("@weekly AnchoredOn Tuesday")]
    [InlineData("@weekly BasedOn Tuesday")]
    [InlineData("@weekly BasedAt Tuesday")]
    [InlineData("@weekly BasedIn Tuesday")]
    public void TryParse_WeeklyFromTuesday_Success(string expression)
    {
        var (recurlyEx, errors) = RecurlyEx.TryParse(expression);
        
        errors.Should().BeEmpty();
        recurlyEx.Should().NotBeNull();
        
        recurlyEx.WeekRules().Count.Should().Be(1);
        var weekRule = recurlyEx.WeekRules().FirstOrDefault() as RecurlyExEveryXRule;
        weekRule.Should().NotBeNull();
        weekRule.Value.Should().Be(1);
        weekRule.AnchoredValue.Should().Be("Tuesday");
        weekRule.TimeUnit.Should().Be(RecurlyExTimeUnit.Week);
        
        recurlyEx.MinuteRules().Count.Should().Be(0);
        recurlyEx.SecondRules().Count.Should().Be(0);
        recurlyEx.HourRules().Count.Should().Be(0);
        recurlyEx.DayRules().Count.Should().Be(0);
        recurlyEx.MonthRules().Count.Should().Be(0);
        recurlyEx.YearRules().Count.Should().Be(0);
    }
}