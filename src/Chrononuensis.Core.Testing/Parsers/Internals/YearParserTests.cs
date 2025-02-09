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
public class YearParserTests
{
    [TestCase("2000")]
    [TestCase("2025")]
    [TestCase("1000")]
    [TestCase("0800")]
    [TestCase("0032")]
    public void Parse_FourDigit_Valid(string value)
        => Assert.That(YearParser.DigitOn4.Parse(value).Success, Is.True);

    [TestCase("ABCD")]
    [TestCase("202*")]
    public void Parse_FourDigit_Invalid(string value)
        => Assert.That(YearParser.DigitOn4.Parse(value).Success, Is.False);

    [TestCase("17")]
    [TestCase("42")]
    [TestCase("00")]
    [TestCase("99")]
    public void Parse_TwoDigit_Valid(string value)
        => Assert.That(YearParser.DigitOn2.Parse(value).Success, Is.True);

    [TestCase("1")]
    [TestCase("4.2")]
    [TestCase("*00")]
    public void Parse_TwoDigit_Invalid(string value)
        => Assert.That(YearParser.DigitOn2.Parse(value).Success, Is.False);

    [TestCase("17", 2017)]
    [TestCase("42", 1942)]
    [TestCase("00", 2000)]
    [TestCase("99", 1999)]
    public void Parse_TwoDigit_CorrectValue(string value, int expected)
        => Assert.That(YearParser.DigitOn2.Parse(value).Value, Is.EqualTo(expected));

    [TestCase("MCMLXXVIII", 1978)]
    public void Parse_RomanNumeral_Valid(string value, int expected)
        => Assert.That(YearParser.RomanNumeral.Parse(value).Value, Is.EqualTo(expected));

    [TestCase("LXXVIII", 1978)]
    public void Parse_RomanNumeralShort_Valid(string value, int expected)
        => Assert.That(YearParser.RomanNumeralShort.Parse(value).Value, Is.EqualTo(expected));
}
