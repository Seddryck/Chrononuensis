using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chrononuensis.Testing.Localization.Ordinality;
public class GermanOrdinalizerTests
{
    private readonly Chrononuensis.Localization.Ordinality.GermanOrdinalizer _ordinalizer =
        new Chrononuensis.Localization.Ordinality.GermanOrdinalizer();

    [TestCase("1.")]
    [TestCase("2.")]
    [TestCase("3.")]
    [TestCase("10.")]
    [TestCase("21.")]
    [TestCase("101.")]
    public void TryParse_ValidInput_ReturnsExpected(string input)
    {
        var ok = _ordinalizer.TryParse(input, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(ok, Is.True, $"Should successfully parse '{input}'");
            if (ok)
            {
                var expected = int.Parse(input[..^1]); // strip trailing '.'
                Assert.That(value, Is.EqualTo(expected), $"Parsed value of '{input}' should be {expected}");
            }
        });
    }

    [TestCase("1e")]
    [TestCase("1st")]
    [TestCase("1")]      // missing dot
    public void TryParse_InvalidInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    [TestCase(".")]      // missing number
    public void TryParse_IncompleteInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    [Theory]
    public void RoundTrip_Holds_For_Positive_Integers(
        [Values(1, 2, 3, 4, 10, 11, 20, 21, 22, 23, 24, 100, 101, 111, 112, 113)]
        int n)
    {
        var s = _ordinalizer.ToOrdinal(n);
        Assert.That(_ordinalizer.TryParse(s, out var back), Is.True, $"Should parse '{s}'");
        Assert.That(back, Is.EqualTo(n));
    }
}
