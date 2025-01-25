﻿using System;
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
        => Assert.That(YearParser.FourDigit.Parse(value).Success, Is.True);

    [TestCase("ABCD")]
    [TestCase("202*")]
    public void Parse_FourDigit_Invalid(string value)
        => Assert.That(YearParser.FourDigit.Parse(value).Success, Is.False);

    [TestCase("17")]
    [TestCase("42")]
    [TestCase("00")]
    [TestCase("99")]
    public void Parse_TwoDigit_Valid(string value)
        => Assert.That(YearParser.TwoDigit.Parse(value).Success, Is.True);

    [TestCase("1")]
    [TestCase("4.2")]
    [TestCase("*00")]
    public void Parse_TwoDigit_Invalid(string value)
        => Assert.That(YearParser.TwoDigit.Parse(value).Success, Is.False);

    [TestCase("17", 2017)]
    [TestCase("42", 1942)]
    [TestCase("00", 2000)]
    [TestCase("99", 1999)]
    public void Parse_TwoDigit_CorrectValue(string value, int expected)
        => Assert.That(YearParser.TwoDigit.Parse(value).Value, Is.EqualTo(expected));
}
