using System;
using Xunit;

namespace RimDev.AspNetCore.FeatureFlags.Tests;

public class JsonBooleanConverterTests
{
    private readonly JsonBooleanConverter sut = new();

    [Theory]
    [InlineData(false, typeof(DateTime))]
    [InlineData(true, typeof(string))]
    [InlineData(true, typeof(bool))]
    public void CanConvert_returns_expected(bool expected, Type objectType)
    {
        var result = sut.CanConvert(objectType);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(null, "")]
    [InlineData(null, "null")]
    [InlineData(false, "false")]
    [InlineData(false, "False")]
    [InlineData(false, "FALSE")]
    [InlineData(true, "true")]
    [InlineData(true, "True")]
    [InlineData(true, "TRUE")]
    public void ReadJson_returns_expected_for_string(bool? expected, string input)
    {
        var result = sut.ReadJson(input);
        Assert.Equal(expected, result);
    }
}
