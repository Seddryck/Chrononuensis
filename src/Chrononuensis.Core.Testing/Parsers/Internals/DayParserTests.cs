using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Formats.Tokens.Month;
using Chrononuensis.Parsers.Internals;
using NUnit.Framework;
using Pidgin;

namespace Chrononuensis.Testing.Parsers.Internals;
public class DayParserTests
{
    [TestCase("2")]
    [TestCase("25")]
    [TestCase("31")]
    public void Parse_Digit_Valid(string value)
        => Assert.That(DayParser.Digit.Parse(value).Success, Is.True);

    [TestCase("A")]
    [TestCase("400")]
    public void Parse_Digit_Invalid(string value)
        => Assert.That(DayParser.Digit.Parse(value).Success, Is.False);

    [TestCase("17")]
    [TestCase("31")]
    [TestCase("01")]
    public void Parse_PaddedDigit_Valid(string value)
        => Assert.That(DayParser.PaddedDigit.Parse(value).Success, Is.True);

    [TestCase("1")]
    [TestCase("4.2")]
    [TestCase("*00")]
    [TestCase("32")]
    public void Parse_TwoDigit_Invalid(string value)
        => Assert.That(DayParser.PaddedDigit.Parse(value).Success, Is.False);

    [TestCase("XXXI")]
    public void Parse_RomanNumeralMonthToken_Valid(string value)
        => Assert.That(DayParser.RomanNumeral.Parse(value).Success, Is.True);

    [TestCase("XL")]
    [TestCase("XXXII")]
    public void Parse_RomanNumeralMonthToken_Invalid(string value)
    {
        Assert.That(DayParser.RomanNumeral.Parse(value).Success, Is.False);
        Assert.That(DayParser.RomanNumeral.Parse(value).Error, Is.Not.Null);
    }
}
