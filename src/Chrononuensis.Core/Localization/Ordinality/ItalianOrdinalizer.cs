using System;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Chrononuensis.Localization.Ordinality;
public sealed class ItalianMasculineOrdinalizer : IOrdinalizer
{
    // Commonly "1º"; degree sign "°" is often seen, we'll emit the canonical "º"
    public string ToOrdinal(int value) => $"{value}º";

    public bool TryParse(string input, out int value)
    {
        var result = ItalianMascParser.Parse(input);
        if (result.Success) { value = result.Value; return true; }
        value = default; return false;
    }

    // Accept: "1º" (U+00BA) or "1°" (U+00B0)
    private static Parser<char, int> ItalianMascParser =>
        Digit.AtLeastOnceString()
            .Then(String("\u00BA").Or(String("\u00B0")), (digits, _) => int.Parse(digits));
}

public sealed class ItalianFeminineOrdinalizer : IOrdinalizer
{
    public string ToOrdinal(int value) => $"{value}ª";

    public bool TryParse(string input, out int value)
    {
        var result = ItalianFemParser.Parse(input);
        if (result.Success) { value = result.Value; return true; }
        value = default; return false;
    }

    // Accept: "1ª"
    private static Parser<char, int> ItalianFemParser =>
        Digit.AtLeastOnceString()
            .Then(String("\u00AA"), (digits, _) => int.Parse(digits));
}

public sealed class ItalianAnyGenderOrdinalizer : IOrdinalizer
{
    private static readonly ItalianMasculineOrdinalizer _masc = new();
    private static readonly ItalianFeminineOrdinalizer _fem = new();

    private readonly IOrdinalizer _defaultForToOrdinal;

    public ItalianAnyGenderOrdinalizer(IOrdinalizer? defaultForToOrdinal = null)
    {
        _defaultForToOrdinal = defaultForToOrdinal ?? _masc;
    }

    public string ToOrdinal(int value) => _defaultForToOrdinal.ToOrdinal(value);

    public bool TryParse(string input, out int value)
    {
        if (_masc.TryParse(input, out value)) return true;
        if (_fem.TryParse(input, out value)) return true;
        value = default; return false;
    }
}
