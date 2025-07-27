using NaturalCron.Builder;

namespace NaturalCron.UnitTests.Builder;

using System;
using FluentAssertions;
using Xunit;

public class NaturalCronBuilderTests
{
    [Theory]
    [InlineData("every day", false)]
    [InlineData("@every day", true)]
    public void BuildExpr_ShouldHandleEveryDay_WithAmpersatOption(string expected, bool useAmpersat)
    {
        var builder = NaturalCronBuilder.Start();

        if (useAmpersat)
            builder.UseAmpersatPrefix();

        var result = builder.Every().Day().BuildExpr();

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("every 2 days", 2)]
    [InlineData("every 10 days", 10)]
    public void BuildExpr_ShouldHandleEveryWithValue(string expected, int value)
    {
        var result = NaturalCronBuilder.Start()
            .Every(value)
            .Days()
            .BuildExpr();

        result.Should().Be(expected);
    }

    [Fact]
    public void BuildExpr_ShouldHandleAnchored()
    {
        var result = NaturalCronBuilder.Start()
            .Every(3)
            .Weeks()
            .AnchoredOn(DayOfWeek.Friday)
            .BuildExpr();

        result.Should().Be("every 3 weeks anchoredOn fri");
    }

    [Theory]
    [InlineData("on mon", DayOfWeek.Monday)]
    [InlineData("on sun", DayOfWeek.Sunday)]
    public void BuildExpr_ShouldHandleOn_DayOfWeek(string expected, DayOfWeek day)
    {
        var result = NaturalCronBuilder.Start()
            .On(day)
            .BuildExpr();

        result.Should().Be(expected);
    }

    [Fact]
    public void BuildExpr_ShouldHandleOnMultipleValues()
    {
        var result = NaturalCronBuilder.Start()
            .On("Mon", "Wed", "Fri")
            .BuildExpr();

        result.Should().Be("on [Mon, Wed, Fri]");
    }

    [Fact]
    public void BuildExpr_ShouldHandleInMonth()
    {
        var result = NaturalCronBuilder.Start()
            .In(NaturalCronMonth.Jan)
            .BuildExpr();

        result.Should().Be("in jan");
    }

    [Fact]
    public void BuildExpr_ShouldHandleAtTime()
    {
        var result = NaturalCronBuilder.Start()
            .AtTime(10, 30)
            .BuildExpr();

        result.Should().Be("at 10:30");
    }

    [Fact]
    public void BuildExpr_ShouldHandleBetweenRange()
    {
        var result = NaturalCronBuilder.Start()
            .Every().Day()
            .Between("Jan", "Mar")
            .BuildExpr();

        result.Should().Be("every day between Jan and Mar");
    }

    [Fact]
    public void BuildExpr_ShouldHandleFromAndUpto()
    {
        var result = NaturalCronBuilder.Start()
            .Every().Day()
            .From("Jan")
            .Upto("Dec")
            .BuildExpr();

        result.Should().Be("every day from Jan upto Dec");
    }

    [Fact]
    public void BuildExpr_ShouldHandleWithTimeZone()
    {
        var result = NaturalCronBuilder.Start()
            .Every()
            .Day()
            .WithTimeZone("America/New_York")
            .BuildExpr();

        result.Should().Be("every day TimeZone America/New_York");
    }

    [Fact]
    public void Build_ShouldReturnNaturalCronExpr()
    {
        var expr = NaturalCronBuilder.Start()
            .Every()
            .Day()
            .Build();

        expr.Should().NotBeNull();
        expr.Expression.Should().Be("every day");
    }
}