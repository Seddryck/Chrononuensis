using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using Chrononuensis.Parsers.Internals;
using NUnit.Framework;
using Pidgin;

namespace Chrononuensis.Testing.Parsers.Internals;

public class PrimitivesTests
{
    [Test]
    public void Parse_LiteralChar_ReturnsUnit()
    {
        var actual = Primitives.CharParser('*').Parse("*");
        Assert.That(actual.Value, Is.EqualTo(Unit.Value));
    }

    [Test]
    public void Parse_LiteralString_ReturnsUnit()
    {
        var actual = Primitives.StringParser("st").Parse("st");
        Assert.That(actual.Value, Is.EqualTo(Unit.Value));
    }

    [Test]
    public void Parse_LiteralStrings_ReturnsUnit()
    {
        var actual = Primitives.StringParsers(["st", "nd", "rd", "th"]).Parse("rd");
        Assert.That(actual.Value, Is.EqualTo(Unit.Value));
    }

    [Test]
    public void Parse_LiteralStringsSameStart_ReturnsUnit()
    {
        var actual = Primitives.StringParsers(["er", "e"]).Parse("e");
        Assert.That(actual.Value, Is.EqualTo(Unit.Value));
    }

    [Test]
    public void Parse_FourDigits_ReturnsInt()
    {
        var actual = Primitives.FourDigitParser().Parse("2021");
        Assert.That(actual.Value, Is.EqualTo(2021));
    }

    [Test]
    public void Parse_ThreeDigits_MissingDigit()
    {
        var actual = Primitives.FourDigitParser().Parse("202");
        Assert.That(actual.Error, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Error.Unexpected.HasValue, Is.False);
            Assert.That(actual.Error?.EOF, Is.True);
            Assert.That(actual.Error.Expected.ElementAt(0).Label, Is.EqualTo("digit"));
            Assert.That(actual.Error.ErrorPos.Col, Is.EqualTo(4));
        });
    }

    [Test]
    public void Parse_LetterInPlaceOfDigit_MissingDigits()
    {
        var actual = Primitives.FourDigitParser().Parse("20A2");
        Assert.That(actual.Error, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Error?.EOF, Is.False);
            Assert.That(actual.Error.Unexpected.Value, Is.EqualTo('A'));
            Assert.That(actual.Error.Expected.ElementAt(0).Label, Is.EqualTo("digit"));
            Assert.That(actual.Error.ErrorPos.Col, Is.EqualTo(3));
        });
    }

    [Test]
    public void Parse_IncorrectChar_MissingChar()
    {
        var actual = Primitives.CharParser('-').Parse("*");
        Assert.That(actual.Error, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Error?.EOF, Is.False);
            Assert.That(actual.Error.Unexpected.Value, Is.EqualTo('*'));
            Assert.That(actual.Error.Expected.ElementAt(0).Label, Is.Null);
            Assert.That(actual.Error.Expected.ElementAt(0).Tokens, Is.EqualTo("-"));
            Assert.That(actual.Error.ErrorPos.Col, Is.EqualTo(1));
        });
    }

    [Test]
    public void Parse_FourDigitsThenTwoDigits_ReturnsInt()
    {
        var actual = Primitives.CombineParsers(
            Primitives.FourDigitParser().Cast<object>(),
            Primitives.CharParser('-').Cast<object>(),
            Primitives.TwoDigitParser(0, 99).Cast<object>()
            ).Parse("2021-06").Value;
        Assert.That(actual, Is.EqualTo(new object?[] { 2021, Unit.Value, 6 }));
    }

    [Test]
    [TestCase("1", 1)]
    [TestCase("1 A", 1)]
    [TestCase("12", 12)]
    [TestCase("123", 123)]
    [TestCase("1234", 123)]
    public void Parse_OneToThreeDigits_ReturnsInt(string input, int expected)
    {
        var actual = Primitives.OneToThreeDigitParser(0, 999).Parse(input);
        Assert.That(actual.Value, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("10", 10)]
    [TestCase("04", 4)]
    public void Parse_TwoDigits_ReturnsInt(string input, int expected)
    {
        var actual = Primitives.TwoDigitParser().Parse(input);
        Assert.That(actual.Value, Is.EqualTo(expected));
    }

    [TestCase("es-es", "Olimpiada")]
    [TestCase("fr-fr", "Olympiade")]
    public void Parse_Localized_ReturnsInt(string culture, string key)
    {
        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
        var actual = Primitives.LocalizedParser("Olympiad").Parse(key);
        Assert.That(actual.Value, Is.EqualTo(Unit.Value));
    }

    [Test]
    public void Parse_Localized_ErrorWhenIncorrectValue()
    {
        var actual = Primitives.LocalizedParser("Olympiad").Parse("NotExisting");
        Assert.That(actual.Error, Is.Not.Null.Or.Empty);
    }

    [Test]
    public void Parse_Localized_ErrorWhenIncorrectValu2e()
        => Assert.Throws<MissingManifestResourceException>(
            () => Primitives.LocalizedParser("NotExisting").Parse("Olympiad"));

    [Test]
    [TestCase([])]
    public void Combine_Empty_Throws(params Parser<char, object>[] values)
        => Assert.Throws<ArgumentException>(() => Primitives.CombineParsers(values));
}
