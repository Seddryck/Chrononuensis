using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Pidgin;
using static Pidgin.Parser;

namespace Chrononuensis.Parsers.Internals;
internal static partial class Primitives
{
    public static Parser<char, int> RomanNumeral(int min, int max) =>
        RomanNumber
            .Assert(value => value >= min && value <= max, $"Value must be between {min} and {max}");

    public static Parser<char, int> RomanNumeral(int min, int max, Func<int, int> _normalize) =>
        RomanNumeral(min, max).Select(_normalize);

    // Define individual Roman numeral parsers
    private static readonly Parser<char, int> M = String("M").ThenReturn(1000);
    private static readonly Parser<char, int> D = String("D").ThenReturn(500);
    private static readonly Parser<char, int> C = String("C").ThenReturn(100);
    private static readonly Parser<char, int> L = String("L").ThenReturn(50);
    private static readonly Parser<char, int> X = String("X").ThenReturn(10);
    private static readonly Parser<char, int> V = String("V").ThenReturn(5);
    private static readonly Parser<char, int> I = String("I").ThenReturn(1);

    // Define subtractive pairs
    private static readonly Parser<char, int> CM = String("CM").ThenReturn(900);
    private static readonly Parser<char, int> CD = String("CD").ThenReturn(400);
    private static readonly Parser<char, int> XC = String("XC").ThenReturn(90);
    private static readonly Parser<char, int> XL = String("XL").ThenReturn(40);
    private static readonly Parser<char, int> IX = String("IX").ThenReturn(9);
    private static readonly Parser<char, int> IV = String("IV").ThenReturn(4);

    // Define the full parser by prioritizing larger values first
    internal static readonly Parser<char, int> RomanNumber =
        OneOf(Try(CM), M, Try(CD), D, C, Try(XC), Try(XL), L, X, Try(IX), Try(IV), V, I)
        .AtLeastOnce() // Ensure at least one valid symbol
        .Select(values => values.Sum()); // Sum up the parsed values
}
