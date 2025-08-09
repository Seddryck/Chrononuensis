using System;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Chrononuensis.Localization.Ordinality;
public sealed class SpanishMasculineOrdinalizer : IOrdinalizer
{
    public string ToOrdinal(int value) => $"{value}.º"; // common printed form

    public bool TryParse(string input, out int value)
    {
        var result = SpanishMascParser.Parse(input);
        if (result.Success) { value = result.Value; return true; }
        value = default; return false;
    }

    // Accept: "1º" or "1.º"
    private static Parser<char, int> SpanishMascParser =>
        Digit.AtLeastOnceString()
        .Before(Char('.').Optional().IgnoreResult())
        .Before(String("\u00BA").Or(String("\u00B0"))) // 'º' or '°'
        .Select(int.Parse);
}

public sealed class SpanishFeminineOrdinalizer : IOrdinalizer
{
    public string ToOrdinal(int value) => $"{value}.ª";

    public bool TryParse(string input, out int value)
    {
        var result = SpanishFemParser.Parse(input);
        if (result.Success) { value = result.Value; return true; }
        value = default; return false;
    }

    // Accept: "1ª" or "1.ª"
    private static Parser<char, int> SpanishFemParser =>
        Digit.AtLeastOnceString()
        .Before(Char('.').Optional().IgnoreResult())
        .Before(String("\u00AA")) // accept 'ª'
        .Select(int.Parse);
}

public sealed class SpanishAnyGenderOrdinalizer : IOrdinalizer
{
    private static readonly SpanishMasculineOrdinalizer _masc = new();
    private static readonly SpanishFeminineOrdinalizer _fem = new();

    // Default ToOrdinal uses masculine; change if you prefer feminine
    private readonly IOrdinalizer _defaultForToOrdinal;

    public SpanishAnyGenderOrdinalizer(IOrdinalizer? defaultForToOrdinal = null)
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
