using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chrononuensis.Testing.Localization.Ordinality;
public class ItalianMasculineOrdinalizerTests
{
    private readonly Chrononuensis.Localization.Ordinality.ItalianMasculineOrdinalizer _ordinalizer =
        new Chrononuensis.Localization.Ordinality.ItalianMasculineOrdinalizer();

    [TestCase("1º")]
    [TestCase("1°")]   // degree sign variant accepted by parser
    [TestCase("2º")]
    [TestCase("10º")]
    [TestCase("21º")]
    [TestCase("101º")]
    public void TryParse_ValidInput_ReturnsExpected(string input)
    {
        var ok = _ordinalizer.TryParse(input, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(ok, Is.True, $"Should successfully parse '{input}'");
            if (ok)
            {
                var trimmed = (input.EndsWith("º") || input.EndsWith("°")) ? input[..^1] : input;
                var expected = int.Parse(trimmed);
                Assert.That(value, Is.EqualTo(expected), $"Parsed value of '{input}' should be {expected}");
            }
        });
    }

    [TestCase("1ª")]
    [TestCase("2ª")]
    [TestCase("-1º")]
    public void TryParse_InvalidInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    [TestCase("1")]
    [TestCase("º")]
    [TestCase("°")]
    public void TryParse_IncompleteInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    // ASCII lookalike should fail
    [TestCase("1o")]
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
        var s = _ordinalizer.ToOrdinal(n);               // emits "º"
        Assert.That(_ordinalizer.TryParse(s, out var back), Is.True, $"Should parse '{s}'");
        Assert.That(back, Is.EqualTo(n));
    }
}

public class ItalianFeminineOrdinalizerTests
{
    private readonly Chrononuensis.Localization.Ordinality.ItalianFeminineOrdinalizer _ordinalizer =
        new Chrononuensis.Localization.Ordinality.ItalianFeminineOrdinalizer();

    [TestCase("1ª")]
    [TestCase("2ª")]
    [TestCase("10ª")]
    [TestCase("21ª")]
    [TestCase("101ª")]
    public void TryParse_ValidInput_ReturnsExpected(string input)
    {
        var ok = _ordinalizer.TryParse(input, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(ok, Is.True, $"Should successfully parse '{input}'");
            if (ok)
            {
                var expected = int.Parse(input[..^1]);  // strip 'ª'
                Assert.That(value, Is.EqualTo(expected), $"Parsed value of '{input}' should be {expected}");
            }
        });
    }

    [TestCase("1º")]
    [TestCase("1°")]
    [TestCase("2º")]
    [TestCase("-1ª")]
    public void TryParse_InvalidInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    [TestCase("1")]
    [TestCase("ª")]
    public void TryParse_IncompleteInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    [TestCase("1a")]
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
        var s = _ordinalizer.ToOrdinal(n);               // emits "ª"
        Assert.That(_ordinalizer.TryParse(s, out var back), Is.True, $"Should parse '{s}'");
        Assert.That(back, Is.EqualTo(n));
    }
}

public class ItalianAnyGenderOrdinalizerTests
{
    private readonly Chrononuensis.Localization.Ordinality.ItalianAnyGenderOrdinalizer _ordinalizer =
        new Chrononuensis.Localization.Ordinality.ItalianAnyGenderOrdinalizer();

    [TestCase("1º")]
    [TestCase("1°")]
    [TestCase("1ª")]
    [TestCase("2º")]
    [TestCase("2ª")]
    [TestCase("10º")]
    [TestCase("10ª")]
    [TestCase("21º")]
    [TestCase("21ª")]
    public void TryParse_ValidInput_ReturnsExpected(string input)
    {
        var ok = _ordinalizer.TryParse(input, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(ok, Is.True, $"Should successfully parse '{input}'");
            if (ok)
            {
                var trimmed = (input.EndsWith("º") || input.EndsWith("°") || input.EndsWith("ª"))
                    ? input[..^1] : input;
                var expected = int.Parse(trimmed);
                Assert.That(value, Is.EqualTo(expected), $"Parsed value of '{input}' should be {expected}");
            }
        });
    }

    [TestCase("1")]
    [TestCase("º")]
    [TestCase("°")]
    [TestCase("ª")]
    public void TryParse_IncompleteInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    [TestCase("1o")]
    [TestCase("1a")]
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
        var s = _ordinalizer.ToOrdinal(n);               // uses your default-gender choice
        Assert.That(_ordinalizer.TryParse(s, out var back), Is.True, $"Should parse '{s}'");
        Assert.That(back, Is.EqualTo(n));
    }
}
