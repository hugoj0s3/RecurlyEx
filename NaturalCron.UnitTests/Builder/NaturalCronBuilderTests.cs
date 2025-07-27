using NaturalCron.Builder;
using System;
using FluentAssertions;
using Xunit;

namespace NaturalCron.UnitTests.Builder
{
    public class NaturalCronBuilderTests
    {
        [Theory]
        [InlineData("Every Day", false)]
        [InlineData("@Every Day", true)]
        public void Build_ShouldHandleEveryDay_WithAmpersatOption(string expected, bool useAmpersat)
        {
            var builder = NaturalCronBuilder.Start();

            if (useAmpersat)
                builder.UseAmpersatPrefix();

            var expr = builder.Every().Day().Build();

            expr.Expression.Should().Be(expected);
        }

        [Theory]
        [InlineData("Every 2 Days", 2)]
        [InlineData("Every 10 Days", 10)]
        public void Build_ShouldHandleEveryWithValue(string expected, int value)
        {
            var expr = NaturalCronBuilder.Start()
                .Every(value)
                .Days()
                .Build();

            expr.Expression.Should().Be(expected);
        }

        [Fact]
        public void Build_ShouldHandleAnchored()
        {
            var expr = NaturalCronBuilder.Start()
                .Every(3)
                .Weeks()
                .AnchoredOn(DayOfWeek.Friday)
                .Build();

            expr.Expression.Should().Be("Every 3 Weeks AnchoredOn Fri");
        }

        [Theory]
        [InlineData("On Mon", DayOfWeek.Monday)]
        [InlineData("On Sun", DayOfWeek.Sunday)]
        public void Build_ShouldHandleOn_DayOfWeek(string expected, DayOfWeek day)
        {
            var expr = NaturalCronBuilder.Start()
                .On(day)
                .Build();

            expr.Expression.Should().Be(expected);
        }

        [Fact]
        public void Build_ShouldHandleOnMultipleValues()
        {
            var expr = NaturalCronBuilder.Start()
                .On("mon", "wed", "fri")
                .Build();

            expr.Expression.Should().Be("On [mon, wed, fri]");
        }

        [Fact]
        public void Build_ShouldHandleInMonth()
        {
            var expr = NaturalCronBuilder.Start()
                .In(NaturalCronMonth.Jan)
                .Build();

            expr.Expression.Should().Be("In Jan");
        }

        [Fact]
        public void Build_ShouldHandleAtTime()
        {
            var expr = NaturalCronBuilder.Start()
                .AtTime(10, 30)
                .Build();

            expr.Expression.Should().Be("At 10:30");
        }

        [Fact]
        public void Build_ShouldHandleBetweenRange()
        {
            var expr = NaturalCronBuilder.Start()
                .Every().Day()
                .Between("Jan", "Mar")
                .Build();

            expr.Expression.Should().Be("Every Day Between Jan and Mar");
        }

        [Fact]
        public void Build_ShouldHandleFromAndUpto()
        {
            var expr = NaturalCronBuilder.Start()
                .Every().Day()
                .From("Jan")
                .Upto("Dec")
                .Build();

            expr.Expression.Should().Be("Every Day From Jan Upto Dec");
        }

        [Fact]
        public void Build_ShouldHandleWithTimeZone()
        {
            var expr = NaturalCronBuilder.Start()
                .Every()
                .Day()
                .WithTimeZone("America/New_York")
                .Build();

            expr.Expression.Should().Be("Every Day TimeZone America/New_York");
        }

        [Fact]
        public void Build_ShouldReturnValidNaturalCronExpr()
        {
            var expr = NaturalCronBuilder.Start()
                .Every()
                .Day()
                .Build();

            expr.Should().NotBeNull();
            expr.Expression.Should().Be("Every Day");
        }
        
        [Fact]
        public void Build_ShouldHandleUseWeekFullName()
        {
            var expr = NaturalCronBuilder.Start()
                .UseWeekFullName()
                .On(DayOfWeek.Monday)
                .Build();

            expr.Expression.Should().Be("On Monday");
        }

        [Fact]
        public void Build_ShouldHandleUseMonthFullName()
        {
            var expr = NaturalCronBuilder.Start()
                .UseMonthFullName()
                .In(NaturalCronMonth.Jan)
                .Build();

            expr.Expression.Should().Be("In January");
        }

        [Fact]
        public void Build_ShouldHandleUseWeekAndMonthFullNameTogether()
        {
            var expr = NaturalCronBuilder.Start()
                .UseWeekFullName()
                .UseMonthFullName()
                .On(DayOfWeek.Friday)
                .In(NaturalCronMonth.Dec)
                .Build();

            expr.Expression.Should().Be("On Friday In December");
        }

        [Fact]
        public void Build_ShouldHandleEveryWithFullNamesAndAnchors()
        {
            var expr = NaturalCronBuilder.Start()
                .UseWeekFullName()
                .UseMonthFullName()
                .Every(2)
                .Weeks()
                .AnchoredOn(DayOfWeek.Monday)
                .Build();

            expr.Expression.Should().Be("Every 2 Weeks AnchoredOn Monday");
        }

        [Fact]
        public void Build_ShouldHandleComplexExpression_WithEverything()
        {
            var expr = NaturalCronBuilder.Start()
                .UseAmpersatPrefix()
                .UseWeekFullName()
                .UseMonthFullName()
                .Every(2)
                .Weeks()
                .AnchoredOn(DayOfWeek.Monday)
                .Between(NaturalCronMonth.Jan, NaturalCronMonth.Mar)
                .WithTimeZone("UTC")
                .Build();

            expr.Expression.Should().Be("@Every 2 Weeks AnchoredOn Monday @Between January and March @TimeZone UTC");
        }

        [Fact]
        public void Build_ShouldHandleComplexExpression_WithoutAmpersat()
        {
            var expr = NaturalCronBuilder.Start()
                .UseWeekFullName()
                .Every(3)
                .Months()
                .AnchoredIn(NaturalCronMonth.Feb)
                .From(NaturalCronMonth.Feb)
                .Upto(NaturalCronMonth.Dec)
                .Build();

            expr.Expression.Should().Be("Every 3 Months AnchoredIn Feb From Feb Upto Dec");
        }
        
        [Theory]
        [InlineData("Yearly")]
        [InlineData("Monthly")]
        [InlineData("Weekly")]
        [InlineData("Daily")]
        [InlineData("Hourly")]
        [InlineData("Minutely")]
        [InlineData("Secondly")]
        public void Build_ShouldHandleStaticTimeUnitMethods(string expected)
        {
            var expr = expected switch
            {
                "Yearly" => NaturalCronBuilder.Yearly(),
                "Monthly" => NaturalCronBuilder.Monthly(),
                "Weekly" => NaturalCronBuilder.Weekly(),
                "Daily" => NaturalCronBuilder.Daily(),
                "Hourly" => NaturalCronBuilder.Hourly(),
                "Minutely" => NaturalCronBuilder.Minutely(),
                "Secondly" => NaturalCronBuilder.Secondly(),
                _ => throw new ArgumentException("Invalid time unit")
            };

            expr.Build().Expression.Should().Be(expected);
        }

        [Fact]
        public void Build_ShouldHandleStaticEveryMethods()
        {
            var expr1 = NaturalCronBuilder.Every().Day().Build();
            expr1.Expression.Should().Be("Every Day");

            var expr2 = NaturalCronBuilder.Every(5).Days().Build();
            expr2.Expression.Should().Be("Every 5 Days");
        }

        [Fact]
        public void Build_ShouldHandleStaticConfigurationMethods()
        {
            var expr1 = NaturalCronBuilder.UseAmpersat().Daily().Build();
            expr1.Expression.Should().Be("@Daily");

            var expr2 = NaturalCronBuilder.UseWeekFullName().On(DayOfWeek.Monday).Build();
            expr2.Expression.Should().Be("On Monday");

            var expr3 = NaturalCronBuilder.UseMonthFullName().In(NaturalCronMonth.Jan).Build();
            expr3.Expression.Should().Be("In January");
        }

        [Theory]
        [InlineData(10, 30, 45, null, "At 10:30:45")]
        [InlineData(2, 15, null, NaturalCronAmOrPm.Pm, "At 02:15pm")]
        [InlineData(11, 0, 30, NaturalCronAmOrPm.Am, "At 11:00:30am")]
        public void Build_ShouldHandleAtTimeWithSecondsAndAmPm(int hour, int minute, int? second, NaturalCronAmOrPm? amOrPm, string expected)
        {
            var expr = NaturalCronBuilder.Start()
                .AtTime(hour, minute, second, amOrPm)
                .Build();

            expr.Expression.Should().Be(expected);
        }

        [Fact]
        public void Build_ShouldHandleBetweenTime()
        {
            var expr1 = NaturalCronBuilder.Start()
                .Hourly()
                .BetweenTime(9, 0, 17, 30)
                .Build();

            expr1.Expression.Should().Be("Hourly Between 09:00 and 17:30");

            var expr2 = NaturalCronBuilder.Start()
                .Hourly()
                .BetweenTime(9, 0, 30, 17, 30, 0)
                .Build();

            expr2.Expression.Should().Be("Hourly Between 09:00:30 and 17:30:00");
        }

        [Fact]
        public void Build_ShouldHandleFromTimeAndUptoTime()
        {
            var expr1 = NaturalCronBuilder.Start()
                .Hourly()
                .FromTime(9, 0)
                .Build();

            expr1.Expression.Should().Be("Hourly From 09:00");

            var expr2 = NaturalCronBuilder.Start()
                .Hourly()
                .UptoTime(17, 30, 0, NaturalCronAmOrPm.Pm)
                .Build();

            expr2.Expression.Should().Be("Hourly Upto 17:30:00pm");
        }

        [Theory]
        [InlineData(NaturalCronDayPosition.FirstDay, "At FirstDay")]
        [InlineData(NaturalCronDayPosition.FirstWeekday, "At FirstWeekday")]
        [InlineData(NaturalCronDayPosition.LastDay, "At LastDay")]
        [InlineData(NaturalCronDayPosition.LastWeekday, "At LastWeekday")]
        public void Build_ShouldHandleAtDayPosition(NaturalCronDayPosition position, string expected)
        {
            var expr = NaturalCronBuilder.Start()
                .AtDayPosition(position)
                .Build();

            expr.Expression.Should().Be(expected);
        }

        [Theory]
        [InlineData(NaturalCronNthWeekDay.First, NaturalCronDayOfWeek.Mon, "On 1stMon")]
        [InlineData(NaturalCronNthWeekDay.Second, NaturalCronDayOfWeek.Fri, "On 2ndFri")]
        [InlineData(NaturalCronNthWeekDay.Third, NaturalCronDayOfWeek.Wed, "On 3rdWed")]
        [InlineData(NaturalCronNthWeekDay.Fourth, NaturalCronDayOfWeek.Thu, "On 4thThu")]
        [InlineData(NaturalCronNthWeekDay.Fifth, NaturalCronDayOfWeek.Sat, "On 5thSat")]
        [InlineData(NaturalCronNthWeekDay.Last, NaturalCronDayOfWeek.Sun, "On LastSun")]
        public void Build_ShouldHandleOnNthWeekday(NaturalCronNthWeekDay nthDay, NaturalCronDayOfWeek dayOfWeek, string expected)
        {
            var expr = NaturalCronBuilder.Start()
                .OnNthWeekday(nthDay, dayOfWeek)
                .Build();

            expr.Expression.Should().Be(expected);
        }

        [Theory]
        [InlineData(15, "On ClosestWeekdayTo 15th")]
        [InlineData(1, "On ClosestWeekdayTo 1st")]
        [InlineData(22, "On ClosestWeekdayTo 22nd")]
        [InlineData(23, "On ClosestWeekdayTo 23rd")]
        public void Build_ShouldHandleOnClosestWeekdayTo(int day, string expected)
        {
            var expr = NaturalCronBuilder.Start()
                .OnClosestWeekdayTo(day)
                .Build();

            expr.Expression.Should().Be(expected);
        }

        [Theory]
        [InlineData(1, "At 1st")]
        [InlineData(2, "At 2nd")]
        [InlineData(3, "At 3rd")]
        [InlineData(4, "At 4th")]
        [InlineData(11, "At 11th")]
        [InlineData(12, "At 12th")]
        [InlineData(13, "At 13th")]
        [InlineData(21, "At 21st")]
        [InlineData(22, "At 22nd")]
        [InlineData(23, "At 23rd")]
        [InlineData(31, "At 31st")]
        public void Build_ShouldHandleAtDayWithOrdinals(int day, string expected)
        {
            var expr = NaturalCronBuilder.Start()
                .AtDay(day)
                .Build();

            expr.Expression.Should().Be(expected);
        }

        [Fact]
        public void Build_ShouldHandleMonthAndDayMethods()
        {
            var expr1 = NaturalCronBuilder.Start()
                .At(NaturalCronMonth.Dec, 25)
                .Build();

            expr1.Expression.Should().Be("At 25th Dec");

            var expr2 = NaturalCronBuilder.Start()
                .Every().Day()
                .From(NaturalCronMonth.Jan, 1)
                .Build();

            expr2.Expression.Should().Be("Every Day From 1st Jan");

            var expr3 = NaturalCronBuilder.Start()
                .Every().Day()
                .Upto(NaturalCronMonth.Dec, 31)
                .Build();

            expr3.Expression.Should().Be("Every Day Upto 31st Dec");
        }

        [Fact]
        public void Build_ShouldHandleBetweenMonthAndDay()
        {
            var expr = NaturalCronBuilder.Start()
                .Daily()
                .Between(NaturalCronMonth.Jan, 1, NaturalCronMonth.Dec, 31)
                .Build();

            expr.Expression.Should().Be("Daily Between 1st Jan and 31st Dec");
        }

        [Fact]
        public void Build_ShouldHandleAnchoredInAndAnchoredAt()
        {
            var expr1 = NaturalCronBuilder.Start()
                .Every(2)
                .Months()
                .AnchoredIn(NaturalCronMonth.Feb)
                .Build();

            expr1.Expression.Should().Be("Every 2 Months AnchoredIn Feb");

            var expr2 = NaturalCronBuilder.Start()
                .Every(3)
                .Days()
                .AnchoredAt("12:00")
                .Build();

            expr2.Expression.Should().Be("Every 3 Days AnchoredAt 12:00");
        }

        [Fact]
        public void Build_ShouldHandleAnchoredWithIntegerValues()
        {
            var expr1 = NaturalCronBuilder.Start()
                .Every(2)
                .Days()
                .AnchoredOn(15)
                .Build();

            expr1.Expression.Should().Be("Every 2 Days AnchoredOn 15th");

            var expr2 = NaturalCronBuilder.Start()
                .Every(3)
                .Weeks()
                .AnchoredIn(5)
                .Build();

            expr2.Expression.Should().Be("Every 3 Weeks AnchoredIn 5");
        }

        [Fact]
        public void Build_ShouldHandleDayOfWeekFromAndUpto()
        {
            var expr1 = NaturalCronBuilder.Start()
                .Daily()
                .From(DayOfWeek.Monday)
                .Build();

            expr1.Expression.Should().Be("Daily From Mon");

            var expr2 = NaturalCronBuilder.Start()
                .Daily()
                .Upto(NaturalCronDayOfWeek.Fri)
                .Build();

            expr2.Expression.Should().Be("Daily Upto Fri");
        }

        [Fact]
        public void Build_ShouldHandleMonthFromAndUpto()
        {
            var expr1 = NaturalCronBuilder.Start()
                .Every(2)
                .Days()
                .From(NaturalCronMonth.Mar)
                .Build();

            expr1.Expression.Should().Be("Every 2 Days From Mar");

            var expr2 = NaturalCronBuilder.Start()
                .Every(3)
                .Months()
                .Upto(NaturalCronMonth.Nov)
                .Build();

            expr2.Expression.Should().Be("Every 3 Months Upto Nov");
        }

        [Fact]
        public void Build_ShouldHandleFromDayAndUptoDay()
        {
            var expr1 = NaturalCronBuilder.Start()
                .Hourly()
                .FromDay(5)
                .Build();

            expr1.Expression.Should().Be("Hourly From 5th");

            var expr2 = NaturalCronBuilder.Start()
                .Hourly()
                .UptoDay(25)
                .Build();

            expr2.Expression.Should().Be("Hourly Upto 25th");
        }

        [Fact]
        public void Build_ShouldHandleBetweenDayOfWeekEnums()
        {
            var expr1 = NaturalCronBuilder.Start()
                .Every(2)
                .Days()
                .Between(DayOfWeek.Monday, DayOfWeek.Friday)
                .Build();

            expr1.Expression.Should().Be("Every 2 Days Between Mon and Fri");

            var expr2 = NaturalCronBuilder.Start()
                .Daily()
                .At("12:00")
                .Between(NaturalCronDayOfWeek.Sun, NaturalCronDayOfWeek.Mon)
                .Build();

            expr2.Expression.Should().Be("Daily At 12:00 Between Sun and Mon");
        }

        [Fact]
        public void Build_ShouldHandleStaticOnInAtMethods()
        {
            var expr1 = NaturalCronBuilder.On("Monday").Build();
            expr1.Expression.Should().Be("On Monday");

            var expr2 = NaturalCronBuilder.In("January").Build();
            expr2.Expression.Should().Be("In January");

            var expr3 = NaturalCronBuilder.At("12:00").Build();
            expr3.Expression.Should().Be("At 12:00");

            var expr4 = NaturalCronBuilder.On("Mon", "Wed", "Fri").Build();
            expr4.Expression.Should().Be("On [Mon, Wed, Fri]");
        }

        [Fact]
        public void Build_ShouldHandleStaticDayAndTimeMethods()
        {
            var expr1 = NaturalCronBuilder.AtDay(15).Build();
            expr1.Expression.Should().Be("At 15th");

            var expr2 = NaturalCronBuilder.AtDayPosition(NaturalCronDayPosition.LastDay).Build();
            expr2.Expression.Should().Be("At LastDay");

            var expr3 = NaturalCronBuilder.OnNthWeekday(NaturalCronNthWeekDay.First, NaturalCronDayOfWeek.Mon).Build();
            expr3.Expression.Should().Be("On 1stMon");

            var expr4 = NaturalCronBuilder.OnClosestWeekdayTo(15).Build();
            expr4.Expression.Should().Be("On ClosestWeekdayTo 15th");

            var expr5 = NaturalCronBuilder.AtTime(14, 30).Build();
            expr5.Expression.Should().Be("At 14:30");
        }

        [Fact]
        public void Build_ShouldHandleComplexChainingWithAllFeatures()
        {
            var expr = NaturalCronBuilder.Start()
                .UseAmpersatPrefix()
                .UseWeekFullName()
                .UseMonthFullName()
                .Every(2)
                .Weeks()
                .AnchoredOn(DayOfWeek.Monday)
                .Between(NaturalCronMonth.Mar, NaturalCronMonth.Nov)
                .AtTime(9, 30, 0, NaturalCronAmOrPm.Am)
                .WithTimeZone("America/New_York")
                .Build();

            expr.Expression.Should().Be("@Every 2 Weeks AnchoredOn Monday @Between March and November @At 09:30:00am @TimeZone America/New_York");
        }

        [Fact]
        public void Build_ShouldHandleBetweenMultipleRanges()
        {
            var expr = NaturalCronBuilder.Start()
                .Every()
                .Day()
                .Between(("Jan", "Mar"), ("Jun", "Aug"), ("Nov", "Dec"))
                .Build();

            expr.Expression.Should().Be("Every Day Between [Jan and Mar, Jun and Aug, Nov and Dec]");
        }

        [Fact]
        public void Build_ShouldHandleSingleTimeUnitMethods()
        {
            var expr1 = NaturalCronBuilder.Start().Every().Day().Build();
            expr1.Expression.Should().Be("Every Day");

            var expr2 = NaturalCronBuilder.Start().Every().Week().Build();
            expr2.Expression.Should().Be("Every Week");

            var expr3 = NaturalCronBuilder.Start().Every().Month().Build();
            expr3.Expression.Should().Be("Every Month");

            var expr4 = NaturalCronBuilder.Start().Every().Year().Build();
            expr4.Expression.Should().Be("Every Year");
        }
        
        [Fact]
        public void Build_ShouldHandleNewSingleTimeUnitMethods()
        {
            // Test Every().Second()
            var expr1 = NaturalCronBuilder.Start().Every().Second().Build();
            expr1.Expression.Should().Be("Every Second");

            // Test Every().Minute()
            var expr2 = NaturalCronBuilder.Start().Every().Minute().Build();
            expr2.Expression.Should().Be("Every Minute");

            // Test Every().Hour()
            var expr3 = NaturalCronBuilder.Start().Every().Hour().Build();
            expr3.Expression.Should().Be("Every Hour");
        }

        [Fact]
        public void Build_ShouldHandleNewSingleTimeUnitMethodsWithRanges()
        {
            // Test Every().Minute() with Between
            var expr1 = NaturalCronBuilder.Start()
                .Every().Minute()
                .Between("10:00am", "12:00pm")
                .Build();
            expr1.Expression.Should().Be("Every Minute Between 10:00am and 12:00pm");

            // Test Every().Hour() with From
            var expr2 = NaturalCronBuilder.Start()
                .Every().Hour()
                .From("08:00pm")
                .Build();
            expr2.Expression.Should().Be("Every Hour From 08:00pm");

            // Test Every().Second() with Upto
            var expr3 = NaturalCronBuilder.Start()
                .Every().Second()
                .Upto("12:00pm")
                .Build();
            expr3.Expression.Should().Be("Every Second Upto 12:00pm");
        }

        [Fact]
        public void Build_ShouldHandleNewSingleTimeUnitMethodsWithAmpersat()
        {
            // Test Every().Minute() with ampersat prefix
            var expr1 = NaturalCronBuilder.Start()
                .UseAmpersatPrefix()
                .Every().Minute()
                .Build();
            expr1.Expression.Should().Be("@Every Minute");

            // Test Every().Hour() with ampersat prefix
            var expr2 = NaturalCronBuilder.Start()
                .UseAmpersatPrefix()
                .Every().Hour()
                .Build();
            expr2.Expression.Should().Be("@Every Hour");

            // Test Every().Second() with ampersat prefix
            var expr3 = NaturalCronBuilder.Start()
                .UseAmpersatPrefix()
                .Every().Second()
                .Build();
            expr3.Expression.Should().Be("@Every Second");
        }

        [Fact]
        public void Build_ShouldHandleNewSingleTimeUnitMethodsWithTimezone()
        {
            // Test Every().Minute() with timezone
            var expr1 = NaturalCronBuilder.Start()
                .Every().Minute()
                .WithTimeZone("America/New_York")
                .Build();
            expr1.Expression.Should().Be("Every Minute TimeZone America/New_York");

            // Test Every().Hour() with timezone
            var expr2 = NaturalCronBuilder.Start()
                .Every().Hour()
                .WithTimeZone("UTC")
                .Build();
            expr2.Expression.Should().Be("Every Hour TimeZone UTC");
        }

        [Theory]
        [InlineData("Every Second", "Second")]
        [InlineData("Every Minute", "Minute")]
        [InlineData("Every Hour", "Hour")]
        public void Build_ShouldHandleNewSingleTimeUnitMethodsParameterized(string expected, string timeUnit)
        {
            var expr = timeUnit switch
            {
                "Second" => NaturalCronBuilder.Start().Every().Second().Build(),
                "Minute" => NaturalCronBuilder.Start().Every().Minute().Build(),
                "Hour" => NaturalCronBuilder.Start().Every().Hour().Build(),
                _ => throw new ArgumentException("Invalid time unit")
            };

            expr.Expression.Should().Be(expected);
        }
        
        [Fact]
        public void Build_ShouldHandleEveryWithValueForNewTimeUnits()
        {
            // Test Every(5).Seconds()
            var expr1 = NaturalCronBuilder.Start()
                .Every(5)
                .Seconds()
                .Build();
            expr1.Expression.Should().Be("Every 5 Seconds");

            // Test Every(15).Minutes()
            var expr2 = NaturalCronBuilder.Start()
                .Every(15)
                .Minutes()
                .Build();
            expr2.Expression.Should().Be("Every 15 Minutes");

            // Test Every(4).Hours()
            var expr3 = NaturalCronBuilder.Start()
                .Every(4)
                .Hours()
                .Build();
            expr3.Expression.Should().Be("Every 4 Hours");
        }

        [Fact]
        public void Build_ShouldHandleEveryWithValueForNewTimeUnitsWithAnchoring()
        {
            // Test Every(30).Seconds() with anchoring
            var expr1 = NaturalCronBuilder.Start()
                .Every(30)
                .Seconds()
                .AnchoredAt(15)
                .Build();
            expr1.Expression.Should().Be("Every 30 Seconds AnchoredAt 15");

            // Test Every(10).Minutes() with anchoring
            var expr2 = NaturalCronBuilder.Start()
                .Every(10)
                .Minutes()
                .AnchoredAt("30")
                .Build();
            expr2.Expression.Should().Be("Every 10 Minutes AnchoredAt 30");

            // Test Every(6).Hours() with anchoring
            var expr3 = NaturalCronBuilder.Start()
                .Every(6)
                .Hours()
                .AnchoredAt("09:00")
                .Build();
            expr3.Expression.Should().Be("Every 6 Hours AnchoredAt 09:00");
        }

        [Fact]
        public void Build_ShouldHandleEveryWithValueForNewTimeUnitsWithRanges()
        {
            // Test Every(5).Minutes() with Between
            var expr1 = NaturalCronBuilder.Start()
                .Every(5)
                .Minutes()
                .Between("10:00", "22:00")
                .Build();
            expr1.Expression.Should().Be("Every 5 Minutes Between 10:00 and 22:00");

            // Test Every(2).Hours() with From
            var expr2 = NaturalCronBuilder.Start()
                .Every(2)
                .Hours()
                .From("08:00")
                .Build();
            expr2.Expression.Should().Be("Every 2 Hours From 08:00");

            // Test Every(10).Seconds() with Upto
            var expr3 = NaturalCronBuilder.Start()
                .Every(10)
                .Seconds()
                .Upto("min 45")
                .Build();
            expr3.Expression.Should().Be("Every 10 Seconds Upto min 45");
        }

        [Fact]
        public void Build_ShouldHandleEveryWithValueForNewTimeUnitsWithAmpersat()
        {
            // Test Every(3).Minutes() with ampersat
            var expr1 = NaturalCronBuilder.Start()
                .UseAmpersatPrefix()
                .Every(3)
                .Minutes()
                .Build();
            expr1.Expression.Should().Be("@Every 3 Minutes");

            // Test Every(8).Hours() with ampersat
            var expr2 = NaturalCronBuilder.Start()
                .UseAmpersatPrefix()
                .Every(8)
                .Hours()
                .Build();
            expr2.Expression.Should().Be("@Every 8 Hours");

            // Test Every(20).Seconds() with ampersat
            var expr3 = NaturalCronBuilder.Start()
                .UseAmpersatPrefix()
                .Every(20)
                .Seconds()
                .Build();
            expr3.Expression.Should().Be("@Every 20 Seconds");
        }

        [Fact]
        public void Build_ShouldHandleEveryWithValueForNewTimeUnitsWithTimezone()
        {
            // Test Every(30).Minutes() with timezone
            var expr1 = NaturalCronBuilder.Start()
                .Every(30)
                .Minutes()
                .WithTimeZone("America/New_York")
                .Build();
            expr1.Expression.Should().Be("Every 30 Minutes TimeZone America/New_York");

            // Test Every(12).Hours() with timezone
            var expr2 = NaturalCronBuilder.Start()
                .Every(12)
                .Hours()
                .WithTimeZone("UTC")
                .Build();
            expr2.Expression.Should().Be("Every 12 Hours TimeZone UTC");
        }

        [Theory]
        [InlineData("Every 2 Seconds", 2, "Seconds")]
        [InlineData("Every 5 Minutes", 5, "Minutes")]
        [InlineData("Every 3 Hours", 3, "Hours")]
        [InlineData("Every 7 Days", 7, "Days")]
        [InlineData("Every 2 Weeks", 2, "Weeks")]
        [InlineData("Every 6 Months", 6, "Months")]
        [InlineData("Every 2 Years", 2, "Years")]
        public void Build_ShouldHandleEveryWithValueParameterized(string expected, int value, string timeUnit)
        {
            var expr = timeUnit switch
            {
                "Seconds" => NaturalCronBuilder.Start().Every(value).Seconds().Build(),
                "Minutes" => NaturalCronBuilder.Start().Every(value).Minutes().Build(),
                "Hours" => NaturalCronBuilder.Start().Every(value).Hours().Build(),
                "Days" => NaturalCronBuilder.Start().Every(value).Days().Build(),
                "Weeks" => NaturalCronBuilder.Start().Every(value).Weeks().Build(),
                "Months" => NaturalCronBuilder.Start().Every(value).Months().Build(),
                "Years" => NaturalCronBuilder.Start().Every(value).Years().Build(),
                _ => throw new ArgumentException("Invalid time unit")
            };

            expr.Expression.Should().Be(expected);
        }

        [Fact]
        public void Build_ShouldHandleEveryWithValueComplexChaining()
        {
            // Test complex chaining with new time units
            var expr1 = NaturalCronBuilder.Start()
                .UseAmpersatPrefix()
                .Every(15)
                .Minutes()
                .Between("10:00", "22:00")
                .WithTimeZone("Europe/London")
                .Build();
            expr1.Expression.Should().Be("@Every 15 Minutes @Between 10:00 and 22:00 @TimeZone Europe/London");

            // Test anchoring with range
            var expr2 = NaturalCronBuilder.Start()
                .Every(4)
                .Hours()
                .AnchoredAt("08:00")
                .From("09:00")
                .Build();
            expr2.Expression.Should().Be("Every 4 Hours AnchoredAt 08:00 From 09:00");
        }
        
        [Fact]
        public void Build_ShouldHandleTimeOnlyMethods()
        {
            var time1 = new TimeOnly(14, 30);
            var expr1 = NaturalCronBuilder.Start()
                .At(time1)
                .Build();

            expr1.Expression.Should().Be("At 14:30");

            var time2 = new TimeOnly(9, 15, 30);
            var expr2 = NaturalCronBuilder.Start()
                .At(time2, includeSeconds: true)
                .Build();

            expr2.Expression.Should().Be("At 09:15:30");

            var startTime = new TimeOnly(9, 0);
            var endTime = new TimeOnly(17, 30);
            var expr3 = NaturalCronBuilder.Start()
                .Daily()
                .Between(startTime, endTime)
                .Build();

            expr3.Expression.Should().Be("Daily Between 09:00 and 17:30");

            var expr4 = NaturalCronBuilder.Start()
                .Hourly()
                .From(new TimeOnly(8, 0))
                .Build();

            expr4.Expression.Should().Be("Hourly From 08:00");

            var expr5 = NaturalCronBuilder.Start()
                .Minutely()
                .Upto(new TimeOnly(18, 0))
                .Build();

            expr5.Expression.Should().Be("Minutely Upto 18:00");
        }
    }
}
