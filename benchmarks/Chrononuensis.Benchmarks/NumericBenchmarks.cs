using Chrononuensis.Parsers.Internals;
using Chrononuensis.Validation;
using Pidgin;

namespace Chrononuensis.Benchmarks;

internal static partial class Program
{
    static partial void RunNumeric()
    {
        CompareNumeric("Fixed two digits", "06", OriginalNumericPrimitives.TwoDigitParser(), Primitives.TwoDigitParser());
        CompareNumeric("Fixed four digits", "2025", OriginalNumericPrimitives.FourDigitParser(), Primitives.FourDigitParser());
        CompareNumeric("Variable one/two digits (one)", "6", OriginalNumericPrimitives.OneOrTwoDigitParser(1, 12), Primitives.OneOrTwoDigitParser(1, 12));
        CompareNumeric("Variable one/two digits (two)", "12", OriginalNumericPrimitives.OneOrTwoDigitParser(1, 12), Primitives.OneOrTwoDigitParser(1, 12));
        CompareNumeric("Variable one/three digits (three)", "365", OriginalNumericPrimitives.OneToThreeDigitParser(1, 366), Primitives.OneToThreeDigitParser(1, 366));
    }

    private static void CompareNumeric(string name, string input, Parser<char, int> before, Parser<char, int> after)
    {
        // Precompose both pipelines; only parsing/conversion is timed. The same
        // string overload and boxing sink are used for each implementation.
        Compare(name, () => sink = before.Parse(input).Value, () => sink = after.Parse(input).Value);
    }
}
