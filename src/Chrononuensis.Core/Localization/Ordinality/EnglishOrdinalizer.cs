using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chrononuensis.Localization.Ordinality;
public class EnglishOrdinalizer : IOrdinalizer
{
    public string ToOrdinal(int value) => $"{value}{GetOrdinalSuffix(value)}";

    private static string GetOrdinalSuffix(int number)
    {
        if (number % 100 is 11 or 12 or 13)
            return "th";

        return (number % 10) switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th"
        };
    }

    public bool TryParse(string input, out int value)
    {
        var result = EnglishOrdinalParser.Parse(input);
        if (result.Success)
        {
            value = result.Value;
            return true;
        }

        value = default;
        return false;
    }

    private static Parser<char, int> EnglishOrdinalParser =>
        Digit.AtLeastOnceString()
        .Then(
            Try(String("st")).Or(Try(String("nd"))).Or(Try(String("rd"))).Or(String("th")),
            (digitsStr, suffix) => (Number: int.Parse(digitsStr), Suffix: suffix)
        )
        .Where(pair => string.Equals(pair.Suffix, GetOrdinalSuffix(pair.Number), StringComparison.OrdinalIgnoreCase))
        .Select(pair => pair.Number);
}
