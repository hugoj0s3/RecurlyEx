using NaturalCron.Builder;
using System;
using FluentAssertions;
using Xunit;

namespace NaturalCron.UnitTests.Builder
{
    public class NaturalCronBuilderTests
    {
        [Theory]
        [InlineData("every day", false)]
        [InlineData("@every day", true)]
        public void Build_ShouldHandleEveryDay_WithAmpersatOption(string expected, bool useAmpersat)
        {
            var builder = NaturalCronBuilder.Start();

            if (useAmpersat)
                builder.UseAmpersatPrefix();

            var expr = builder.Every().Day().Build();

            expr.Expression.Should().Be(expected);
        }

        [Theory]
        [InlineData("every 2 days", 2)]
        [InlineData("every 10 days", 10)]
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

            expr.Expression.Should().Be("every 3 weeks anchoredOn fri");
        }

        [Theory]
        [InlineData("on mon", DayOfWeek.Monday)]
        [InlineData("on sun", DayOfWeek.Sunday)]
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

            expr.Expression.Should().Be("on [mon, wed, fri]");
        }

        [Fact]
        public void Build_ShouldHandleInMonth()
        {
            var expr = NaturalCronBuilder.Start()
                .In(NaturalCronMonth.Jan)
                .Build();

            expr.Expression.Should().Be("in jan");
        }

        [Fact]
        public void Build_ShouldHandleAtTime()
        {
            var expr = NaturalCronBuilder.Start()
                .AtTime(10, 30)
                .Build();

            expr.Expression.Should().Be("at 10:30");
        }

        [Fact]
        public void Build_ShouldHandleBetweenRange()
        {
            var expr = NaturalCronBuilder.Start()
                .Every().Day()
                .Between("Jan", "Mar")
                .Build();

            expr.Expression.Should().Be("every day between Jan and Mar");
        }

        [Fact]
        public void Build_ShouldHandleFromAndUpto()
        {
            var expr = NaturalCronBuilder.Start()
                .Every().Day()
                .From("Jan")
                .Upto("Dec")
                .Build();

            expr.Expression.Should().Be("every day from Jan upto Dec");
        }

        [Fact]
        public void Build_ShouldHandleWithTimeZone()
        {
            var expr = NaturalCronBuilder.Start()
                .Every()
                .Day()
                .WithTimeZone("America/New_York")
                .Build();

            expr.Expression.Should().Be("every day TimeZone America/New_York");
        }

        [Fact]
        public void Build_ShouldReturnValidNaturalCronExpr()
        {
            var expr = NaturalCronBuilder.Start()
                .Every()
                .Day()
                .Build();

            expr.Should().NotBeNull();
            expr.Expression.Should().Be("every day");
        }
        
        [Fact]
        public void Build_ShouldHandleUseWeekFullName()
        {
            var expr = NaturalCronBuilder.Start()
                .UseWeekFullName()
                .On(DayOfWeek.Monday)
                .Build();

            expr.Expression.Should().Be("on monday");
        }

        [Fact]
        public void Build_ShouldHandleUseMonthFullName()
        {
            var expr = NaturalCronBuilder.Start()
                .UseMonthFullName()
                .In(NaturalCronMonth.Jan)
                .Build();

            expr.Expression.Should().Be("in january");
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

            expr.Expression.Should().Be("on friday in december");
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

            expr.Expression.Should().Be("every 2 weeks anchoredOn monday");
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

            expr.Expression.Should().Be("@every 2 weeks anchoredOn monday @between january and march @TimeZone UTC");
        }

        [Fact]
        public void Build_ShouldHandleComplexExpression_WithoutAmpersat()
        {
            var expr = NaturalCronBuilder.Start()
                .UseWeekFullName()
                .Every(3)
                .Months()
                .AnchoredOn(DayOfWeek.Sunday)
                .From(NaturalCronMonth.Feb)
                .Upto(NaturalCronMonth.Dec)
                .Build();

            expr.Expression.Should().Be("every 3 months anchoredOn sunday from feb upto dec");
        }
    }
}
