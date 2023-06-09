﻿using No8.Ascii.Tests.Helpers;
using Xunit;

namespace No8.Ascii.Tests;

[TestClass]
public class StringHelperTests
{
    [Fact]
    public void ParseArguments_Null()
    {
        string? str = null;
        var result = str!.ParseArguments();

        Assert.Null(result);
    }

    [Fact]
    public void ParseArguments_Empty()
    {
        var result = string.Empty.ParseArguments();
        Assert.Empty(result!);
    }

    [Fact]
    public void ParseArguments_Single()
    {
        var result = "single".ParseArguments();

        Assert.Equal("single", result![0]);
    }

    [Fact]
    public void ParseArguments_SingleSeparatorEnd()
    {
        var result = "single\t".ParseArguments();

        Assert.Equal("single", result![0]);
    }

    [Fact]
    public void ParseArguments_Multiple()
    {
        var result = "one two three".ParseArguments();

        Assert.Equal(3, result!.Count);
        Assert.Equal("three", result[2]);
    }

    [Fact]
    public void ParseArguments_MultipleSeparators()
    {
        var result = "one\ttwo\tthree".ParseArguments();

        Assert.Equal(3, result!.Count);
        Assert.Equal("three", result[2]);
    }

    [Fact]
    public void ParseArguments_MultipleLines()
    {
        var result = "one\ntwo\nthree".ParseArguments();

        Assert.Equal(3, result!.Count);
        Assert.Equal("three", result[2]);
    }

    [Fact]
    public void ParseArguments_SingleQuoted()
    {
        var result = "'single'".ParseArguments();

        Assert.Equal("single", result![0]);
    }

    [Fact]
    public void ParseArguments_MultipleQuoted()
    {
        var result = "'one two three'".ParseArguments();

        Assert.Equal("one two three", result![0]);
    }

    [Fact]
    public void ParseArguments_MultipleQuotedMultiple()
    {
        var result = "before 'one two three' after".ParseArguments();

        Assert.Equal(3, result!.Count);
        Assert.Equal("before", result[0]);
        Assert.Equal("one two three", result[1]);
        Assert.Equal("after", result[2]);
    }

    [Fact]
    public void ParseArguments_Bad_SingleQuoted()
    {
        Assert.Equal("single", "'single".ParseArguments()![0]);
        Assert.Equal("single", "\"single".ParseArguments()![0]);
    }

    [Fact]
    public void ParseArguments_Bad_SingleQuotedEnd()
    {
        Assert.Equal("single", "single\'".ParseArguments()![0]);
    }

    [Fact]
    public void ParseArguments_MidWordQuoted()
    {
        var result = "sin'gle'".ParseArguments();

        Assert.Equal("sin", result![0]);
        Assert.Equal("gle", result[1]);
    }
}

