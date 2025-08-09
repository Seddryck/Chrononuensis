using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chrononuensis.Testing.Localization.Ordinality;
public class DutchOrdinalizerTests
{
    private readonly Chrononuensis.Localization.Ordinality.DutchOrdinalizer _ordinalizer =
        new Chrononuensis.Localization.Ordinality.DutchOrdinalizer();

    [TestCase("1e")]
    [TestCase("2e")]
    [TestCase("3e")]
    [TestCase("10e")]
    [TestCase("21e")]
    [TestCase("101e")]
    public void TryParse_ValidInput_ReturnsExpected(string input)
    {
        var ok = _ordinalizer.TryParse(input, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(ok, Is.True, $"Should successfully parse '{input}'");
            if (ok)
            {
                var expected = int.Parse(input[..^1]);
                Assert.That(value, Is.EqualTo(expected), $"Parsed value of '{input}' should be {expected}");
            }
        });
    }

    [TestCase("1ste")]   // not supported by strict parser
    [TestCase("2de")]    // not supported by strict parser
    [TestCase("3ste")]   // not supported by strict parser
    public void TryParse_InvalidInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    [TestCase("1")]    // missing suffix
    [TestCase("e")]    // missing number
    public void TryParse_IncompleteInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    // Keep the same pattern you asked for: case-sensitive, so these fail
    [TestCase("1E")]
    [TestCase("2E")]
    public void TryParse_CaseInsensitive_ReturnsFalse(string input)
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
