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
public class MonthParserTests
{
    private static string[] DigitMonthToken => Enumerable.Range(1, 12).Select(i => i.ToString()).ToArray();
    private static string[] PaddedDigitMonthToken => Enumerable.Range(1, 12).Select(i => i.ToString("D2")).ToArray();
    private static string[] AbbreviationMonthToken => CultureInfo.InvariantCulture.DateTimeFormat.AbbreviatedMonthNames
        .Where(m => !string.IsNullOrEmpty(m))
        .ToArray();

    private static string[] LabelMonthToken => CultureInfo.InvariantCulture.DateTimeFormat.MonthNames
        .Where(m => !string.IsNullOrEmpty(m))
        .ToArray();

    [TestCaseSource(nameof(DigitMonthToken))]
    public void Parse_DigitMonthToken_Valid(string value)
        => Assert.That(MonthParser.Digit.Parse(value).Success, Is.True);

    [TestCase("0")]
    [TestCase("13")]
    public void Parse_DigitMonthToken_Invalid(string value)
    {
        Assert.That(MonthParser.Digit.Parse(value).Success, Is.False);
        Assert.That(MonthParser.Digit.Parse(value).Error, Is.Not.Null);
        Assert.That(MonthParser.Digit.Parse(value).Error.Message, Is.EqualTo("Value must be between 1 and 12"));
    }

    [TestCaseSource(nameof(PaddedDigitMonthToken))]
    public void Parse_PaddedDigitMonthToken_Valid(string value)
        => Assert.That(MonthParser.PaddedDigit.Parse(value).Success, Is.True);

    [TestCase("00")]
    [TestCase("13")]
    public void Parse_PaddedDigitMonthToken_Invalid(string value)
    {
        Assert.That(MonthParser.PaddedDigit.Parse(value).Success, Is.False);
        Assert.That(MonthParser.PaddedDigit.Parse(value).Error, Is.Not.Null);
        Assert.That(MonthParser.PaddedDigit.Parse(value).Error.Message, Is.EqualTo("Value must be between 1 and 12"));
    }

    [TestCaseSource(nameof(AbbreviationMonthToken))]
    public void Parse_AbbreviationMonthToken_Valid(string value)
        => Assert.That(MonthParser.Abbreviation.Parse(value).Success, Is.True);

    [TestCase("Xyz")]
    [TestCase("jan")]
    public void Parse_AbbreviationMonthToken_Invalid(string value)
    {
        Assert.That(MonthParser.Abbreviation.Parse(value).Success, Is.False);
        Assert.That(MonthParser.Abbreviation.Parse(value).Error, Is.Not.Null);
    }


    [TestCaseSource(nameof(LabelMonthToken))]
    public void Parse_LabelMonthToken_Valid(string value)
        => Assert.That(MonthParser.Label.Parse(value).Success, Is.True);

    [TestCase("Xyz")]
    [TestCase("jan")]
    public void Parse_LabelMonthToken_Invalid(string value)
    {
        Assert.That(MonthParser.Label.Parse(value).Success, Is.False);
        Assert.That(MonthParser.Label.Parse(value).Error, Is.Not.Null);
    }
}
