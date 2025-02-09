using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Parsers.Internals;
using NUnit.Framework;
using Pidgin;

namespace Chrononuensis.Testing.Parsers.Internals;

public class RomanNumeralTests
{
    [Test]
    [TestCase("M", 1000)]
    [TestCase("D", 500)]
    [TestCase("C", 100)]
    [TestCase("L", 50)]
    [TestCase("X", 10)]
    [TestCase("V", 5)]
    [TestCase("I", 1)]
    public void Parse_Single_ReturnsInt(string value, int expected)
    {
        var actual = Primitives.RomanNumber.Parse(value);
        Assert.That(actual.Value, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("CM", 900)]
    [TestCase("CD", 400)]
    [TestCase("XC", 90)]
    [TestCase("XL", 40)]
    [TestCase("IX", 9)]
    [TestCase("IV", 4)]
    public void Parse_Subtractive_ReturnsInt(string value, int expected)
    {
        var actual = Primitives.RomanNumber.Parse(value);
        Assert.That(actual.Value, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("MCMLXXVIII", 1978)]
    [TestCase("CMXCIX", 999)]
    [TestCase("CDIV", 404)]
    [TestCase("XCVIII", 98)]
    [TestCase("CXLVI", 146)]
    [TestCase("XII", 12)]
    [TestCase("VIII", 8)]
    public void Parse_Composite_ReturnsInt(string value, int expected)
    {
        var actual = Primitives.RomanNumber.Parse(value);
        Assert.That(actual.Value, Is.EqualTo(expected));
    }
}
