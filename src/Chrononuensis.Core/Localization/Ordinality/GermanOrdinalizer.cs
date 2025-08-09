using System;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Chrononuensis.Localization.Ordinality;
public sealed class GermanOrdinalizer : IOrdinalizer
{
    public string ToOrdinal(int value) => $"{value}.";

    public bool TryParse(string input, out int value)
    {
        var result = GermanParser.Parse(input);
        if (result.Success) { value = result.Value; return true; }
        value = default; return false;
    }

    private static Parser<char, int> GermanParser =>
        Digit.AtLeastOnceString()
            .Then(Char('.'), (digits, _) => int.Parse(digits));
}
