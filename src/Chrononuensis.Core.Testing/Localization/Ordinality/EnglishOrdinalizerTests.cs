using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chrononuensis.Testing.Localization.Ordinality;
public class EnglishOrdinalizerTests
{
    private readonly Chrononuensis.Localization.Ordinality.EnglishOrdinalizer _ordinalizer =
        new Chrononuensis.Localization.Ordinality.EnglishOrdinalizer();

    [TestCase("1st")]
    [TestCase("2nd")]
    [TestCase("3rd")]
    [TestCase("4th")]
    [TestCase("11th")]
    [TestCase("12th")]
    [TestCase("13th")]
    [TestCase("14th")]
    [TestCase("21st")]
    [TestCase("22nd")]
    [TestCase("23rd")]
    [TestCase("24th")]
    [TestCase("100th")]
    [TestCase("101st")]
    [TestCase("120th")]
    [TestCase("121st")]
    [TestCase("122nd")]
    [TestCase("123rd")]
    [TestCase("124th")]
    public void TryParse_ValidInput_ReturnsExpected(string input)
    {
        var isParsable = _ordinalizer.TryParse(input, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(isParsable, Is.True, $"Should successfully parse '{input}'");

            // Only perform expected comparison if parsing succeeded
            if (isParsable)
            {
                var expected = int.Parse(input[..^2]);
                Assert.That(value, Is.EqualTo(expected), $"Parsed value of '{input}' should be {expected}");
            }
        });
    }

    [TestCase("1nd")]
    [TestCase("2rd")]
    [TestCase("1th")]
    [TestCase("4st")]
    [TestCase("11st")]
    [TestCase("12nd")]
    [TestCase("13rd")]
    [TestCase("21th")]
    [TestCase("22st")]
    [TestCase("23nd")]
    public void TryParse_InvalidInput_ReturnsExpected(string input)
    {
        var isParsable = _ordinalizer.TryParse(input, out var value);
        Assert.That(isParsable, Is.False, $"Should not successfully parse '{input}'");
    }

    [TestCase("1")]
    [TestCase("st")]
    [TestCase("1th")]
    [TestCase("4st")]
    [TestCase("11st")]
    [TestCase("12nd")]
    [TestCase("13rd")]
    public void TryParse_IncompleteInput_ReturnsExpected(string input)
    {
        var isParsable = _ordinalizer.TryParse(input, out var value);
        Assert.That(isParsable, Is.False, $"Should not successfully parse '{input}'");
    }

    [TestCase("1ST")]
    [TestCase("2Nd")]
    [TestCase("3rD")]
    [TestCase("4TH")]
    public void TryParse_CaseInsensitive_ReturnsExpected(string input)
    {
        var isParsable = _ordinalizer.TryParse(input, out var value);
        Assert.That(isParsable, Is.False, $"Should not successfully parse '{input}'");
    }

    [Theory]
    public void RoundTrip_Holds_For_Positive_Integers(
        [Values(1, 2, 3, 4, 10, 11, 12, 13, 20, 21, 22, 23, 24, 100, 101, 102, 103, 111, 112, 113, 121, 122, 123)] int n)
    {
        var s = _ordinalizer.ToOrdinal(n);
        Assert.That(_ordinalizer.TryParse(s, out var back), Is.True, $"Should parse '{s}'");
        Assert.That(back, Is.EqualTo(n));
    }
}
