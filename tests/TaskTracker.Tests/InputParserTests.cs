using TaskTracker.Application.Validation;
using TaskTracker.Domain;

namespace TaskTracker.Tests;

/// <summary>Барикада валідації на межі прикладного шару.</summary>
public class InputParserTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Title_EmptyOrNull_Fails(string? text)
    {
        Result<string> result = InputParser.Title(text);

        Assert.False(result.IsSuccess);
        Assert.Equal("Назва не може бути порожньою.", result.Error);
    }

    [Fact]
    public void Title_LongerThanLimit_Fails()
    {
        Result<string> result = InputParser.Title(new string('x', InputParser.MaxTitleLength + 1));

        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("0")]
    [InlineData("-3")]
    [InlineData(null)]
    public void TaskId_NotPositiveNumber_Fails(string? text)
    {
        Assert.False(InputParser.TaskId(text).IsSuccess);
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("80,5")]
    [InlineData("багато")]
    [InlineData(null)]
    public void Hours_NegativeTooBigTextOrNull_Fails(string? text)
    {
        Assert.False(InputParser.Hours(text).IsSuccess);
    }

    [Theory]
    [InlineData("2,5", 2.5)]
    [InlineData("4.25", 4.25)]
    [InlineData("0", 0)]
    public void Hours_CommaOrDot_ParsesDecimal(string text, double expected)
    {
        Assert.Equal((decimal)expected, InputParser.Hours(text).Value);
    }

    [Theory]
    [InlineData("review", BoardColumn.Review)]
    [InlineData(" InProgress ", BoardColumn.InProgress)]
    public void Column_KnownName_Parses(string text, BoardColumn expected)
    {
        Assert.Equal(expected, InputParser.Column(text).Value);
    }

    [Theory]
    [InlineData("2")]
    [InlineData("Archive")]
    [InlineData("")]
    public void Column_NumberOrUnknownCode_Fails(string text)
    {
        Assert.False(InputParser.Column(text).IsSuccess);
    }

    [Theory]
    [InlineData("", TaskPriority.Normal)]
    [InlineData("1", TaskPriority.Critical)]
    [InlineData("low", TaskPriority.Low)]
    public void Priority_EmptyNumberOrName_Parses(string text, TaskPriority expected)
    {
        Assert.Equal(expected, InputParser.Priority(text).Value);
    }

    [Fact]
    public void Priority_OutOfRange_Fails()
    {
        Assert.False(InputParser.Priority("5").IsSuccess);
    }
}
