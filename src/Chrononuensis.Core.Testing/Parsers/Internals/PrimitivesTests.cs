using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Parsers.Internals;
using NUnit.Framework;
using Pidgin;

namespace Chrononuensis.Testing.Parsers.Internals;

public class PrimitivesTests
{
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
}
