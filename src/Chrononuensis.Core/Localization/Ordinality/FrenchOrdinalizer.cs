using System;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Chrononuensis.Localization.Ordinality;
/// <summary>
/// Accepts either masculine (1er) or feminine (1re) for 1, and "e" for ≥2.
/// ToOrdinal delegates to a default gender (masculine by default).
/// </summary>
public sealed class FrenchOrdinalizer : IOrdinalizer
{
    private static readonly FrenchMasculineOrdinalizer _masc = new();
    private static readonly FrenchFeminineOrdinalizer _fem = new();

    private readonly IOrdinalizer _defaultForToOrdinal;

    /// <param name="defaultForToOrdinal">
    /// Ordinalizer used by <see cref="ToOrdinal"/>. Defaults to masculine (common for « siècle »).
    /// </param>
    public FrenchOrdinalizer(IOrdinalizer? defaultForToOrdinal = null)
    {
        _defaultForToOrdinal = defaultForToOrdinal ?? _masc;
    }

    public string ToOrdinal(int value) => _defaultForToOrdinal.ToOrdinal(value);

    public bool TryParse(string input, out int value)
    {
        if (_masc.TryParse(input, out value)) return true;
        if (_fem.TryParse(input, out value)) return true;
        value = default;
        return false;
    }
}

public sealed class FrenchFeminineOrdinalizer : IOrdinalizer
{
    public string ToOrdinal(int value) => value == 1 ? "1re" : $"{value}e";

    public bool TryParse(string input, out int value)
    {
        var result = FrenchFeminineParser.Parse(input);
        if (result.Success)
        {
            value = result.Value;
            return true;
        }
        value = default;
        return false;
    }

    private static Parser<char, int> FrenchFeminineParser =>
        Digit.AtLeastOnceString()
            .Then(
                Try(String("re")).Or(String("e")),
                (digitsStr, suffix) => (Number: int.Parse(digitsStr), Suffix: suffix)
            )
            .Assert(
                p => p.Number == 1 ? p.Suffix == "re" : p.Suffix == "e",
                p => $"Invalid feminine ordinal suffix '{p.Suffix}' for number {p.Number}"
            )
            .Select(p => p.Number);
}

public sealed class FrenchMasculineOrdinalizer : IOrdinalizer
{
    public string ToOrdinal(int value) => value == 1 ? "1er" : $"{value}e";

    public bool TryParse(string input, out int value)
    {
        var result = FrenchMasculineParser.Parse(input);
        if (result.Success)
        {
            value = result.Value;
            return true;
        }
        value = default;
        return false;
    }

    private static Parser<char, int> FrenchMasculineParser =>
        Digit.AtLeastOnceString()
            .Then(
                Try(String("er")).Or(String("e")),
                (digitsStr, suffix) => (Number: int.Parse(digitsStr), Suffix: suffix)
            )
            .Assert(
                p => p.Number == 1 ? p.Suffix == "er" : p.Suffix == "e",
                p => $"Invalid masculine ordinal suffix '{p.Suffix}' for number {p.Number}"
            )
            .Select(p => p.Number);
}
