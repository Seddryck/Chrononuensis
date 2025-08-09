using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chrononuensis.Testing.Localization.Ordinality;
public class FrenchMasculineOrdinalizerTests
{
    private readonly Chrononuensis.Localization.Ordinality.FrenchMasculineOrdinalizer _ordinalizer =
        new Chrononuensis.Localization.Ordinality.FrenchMasculineOrdinalizer();

    [TestCase("1er")]
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
                var expected = int.Parse(input.EndsWith("er") ? input[..^2] : input[..^1]);
                Assert.That(value, Is.EqualTo(expected), $"Parsed value of '{input}' should be {expected}");
            }
        });
    }

    [TestCase("1re")]   // feminine for 1 -> invalid for masculine
    [TestCase("1e")]    // forbidden in our strict rules for 1
    [TestCase("2er")]   // wrong suffix
    [TestCase("3re")]   // wrong suffix
    public void TryParse_InvalidInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    [TestCase("1")]   // missing suffix
    [TestCase("er")]  // missing number
    [TestCase("e")]   // missing number
    public void TryParse_IncompleteInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    // Keep failure expectation: parser is case-sensitive on suffix tokens
    [TestCase("1ER")]
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

public class FrenchFeminineOrdinalizerTests
{
    private readonly Chrononuensis.Localization.Ordinality.FrenchFeminineOrdinalizer _ordinalizer =
        new Chrononuensis.Localization.Ordinality.FrenchFeminineOrdinalizer();

    [TestCase("1re")]
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
                var expected = int.Parse(input.EndsWith("re") ? input[..^2] : input[..^1]);
                Assert.That(value, Is.EqualTo(expected), $"Parsed value of '{input}' should be {expected}");
            }
        });
    }

    [TestCase("1er")]   // masculine for 1 -> invalid for feminine
    [TestCase("1e")]    // forbidden in our strict rules for 1
    [TestCase("2re")]   // wrong suffix
    public void TryParse_InvalidInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    [TestCase("1")]
    [TestCase("re")]
    [TestCase("e")]
    public void TryParse_IncompleteInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    [TestCase("1RE")]
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

public class FrenchAnyGenderOrdinalizerTests
{
    private readonly Chrononuensis.Localization.Ordinality.FrenchOrdinalizer _ordinalizer =
        new Chrononuensis.Localization.Ordinality.FrenchOrdinalizer();

    [TestCase("1er")]
    [TestCase("1re")]
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
                var expected =
                    input.EndsWith("er") || input.EndsWith("re")
                        ? int.Parse(input[..^2])
                        : int.Parse(input[..^1]);
                Assert.That(value, Is.EqualTo(expected), $"Parsed value of '{input}' should be {expected}");
            }
        });
    }

    [TestCase("3re")]
    [TestCase("1e")]
    public void TryParse_InvalidInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    [TestCase("1")]
    [TestCase("er")]
    [TestCase("re")]
    [TestCase("e")]
    public void TryParse_IncompleteInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    [TestCase("1ER")]
    [TestCase("1RE")]
    [TestCase("2E")]
    public void TryParse_CaseInsensitive_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    // By default, AnyGender.ToOrdinal delegates to masculine (per your impl)
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
