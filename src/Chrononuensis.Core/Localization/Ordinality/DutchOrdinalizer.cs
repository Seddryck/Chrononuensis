using System;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Chrononuensis.Localization.Ordinality;
public sealed class DutchOrdinalizer : IOrdinalizer
{
    public string ToOrdinal(int value) => $"{value}e";

    public bool TryParse(string input, out int value)
    {
        var result = DutchParser.Parse(input);
        if (result.Success) { value = result.Value; return true; }
        value = default; return false;
    }

    private static Parser<char, int> DutchParser =>
        Digit.AtLeastOnceString()
            .Then(String("e"), (digits, _) => int.Parse(digits));
}
