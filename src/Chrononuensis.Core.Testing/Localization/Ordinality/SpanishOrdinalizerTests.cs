using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chrononuensis.Testing.Localization.Ordinality;
public class SpanishMasculineOrdinalizerTests
{
    private readonly Chrononuensis.Localization.Ordinality.SpanishMasculineOrdinalizer _ordinalizer =
        new Chrononuensis.Localization.Ordinality.SpanishMasculineOrdinalizer();

    [TestCase("1º")]
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
                // strip optional dot + indicator (1 or 2 chars)
                var trimmed = input.EndsWith("º") ? input[..^1]
                           : input.EndsWith(".º") ? input[..^2]
                           : input;
                var expected = int.Parse(trimmed);
                Assert.That(value, Is.EqualTo(expected), $"Parsed value of '{input}' should be {expected}");
            }
        });
    }

    [TestCase("1ª")]
    [TestCase("1.ª")]
    [TestCase("2ª")]
    [TestCase("2.ª")]
    [TestCase("-1º")]
    public void TryParse_InvalidInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    [TestCase("1")]     // missing indicator
    [TestCase("º")]     // missing number
    [TestCase(".º")]    // missing number
    public void TryParse_IncompleteInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    // Keep the same section name/policy: treat letter lookalikes as invalid
    [TestCase("1o")]
    [TestCase("1.O")]
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
        var s = _ordinalizer.ToOrdinal(n);               // e.g., "21.º"
        Assert.That(_ordinalizer.TryParse(s, out var back), Is.True, $"Should parse '{s}'");
        Assert.That(back, Is.EqualTo(n));
    }
}

public class SpanishFeminineOrdinalizerTests
{
    private readonly Chrononuensis.Localization.Ordinality.SpanishFeminineOrdinalizer _ordinalizer =
        new Chrononuensis.Localization.Ordinality.SpanishFeminineOrdinalizer();

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
                var trimmed = input.EndsWith("ª") ? input[..^1]
                           : input.EndsWith(".ª") ? input[..^2]
                           : input;
                var expected = int.Parse(trimmed);
                Assert.That(value, Is.EqualTo(expected), $"Parsed value of '{input}' should be {expected}");
            }
        });
    }

    [TestCase("1º")]
    [TestCase("1.º")]
    [TestCase("2º")]
    [TestCase("2.º")]
    [TestCase("-1ª")]
    public void TryParse_InvalidInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    [TestCase("1")]
    [TestCase("ª")]
    [TestCase(".ª")]
    public void TryParse_IncompleteInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    // Letter lookalikes should fail
    [TestCase("1a")]
    [TestCase("1.A")]
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
        var s = _ordinalizer.ToOrdinal(n);               // e.g., "21.ª"
        Assert.That(_ordinalizer.TryParse(s, out var back), Is.True, $"Should parse '{s}'");
        Assert.That(back, Is.EqualTo(n));
    }
}

public class SpanishAnyGenderOrdinalizerTests
{
    private readonly Chrononuensis.Localization.Ordinality.SpanishAnyGenderOrdinalizer _ordinalizer =
        new Chrononuensis.Localization.Ordinality.SpanishAnyGenderOrdinalizer();

    [TestCase("1º")]
    [TestCase("1ª")]
    [TestCase("2º")]
    [TestCase("10º")]
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
                var trimmed =
                    (input.EndsWith("º") || input.EndsWith("ª")) ? input[..^1]
                  : (input.EndsWith(".º") || input.EndsWith(".ª")) ? input[..^2]
                  : input;

                var expected = int.Parse(trimmed);
                Assert.That(value, Is.EqualTo(expected), $"Parsed value of '{input}' should be {expected}");
            }
        });
    }

    [TestCase("-1º")]
    public void TryParse_InvalidInput_ReturnsFalse(string input)
    {
        var ok = _ordinalizer.TryParse(input, out _);
        Assert.That(ok, Is.False, $"Should not successfully parse '{input}'");
    }

    [TestCase("1")]
    [TestCase("º")]
    [TestCase("ª")]
    [TestCase(".º")]
    [TestCase(".ª")]
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
        var s = _ordinalizer.ToOrdinal(n);               // defaults to your chosen gender in impl
        Assert.That(_ordinalizer.TryParse(s, out var back), Is.True, $"Should parse '{s}'");
        Assert.That(back, Is.EqualTo(n));
    }
}

