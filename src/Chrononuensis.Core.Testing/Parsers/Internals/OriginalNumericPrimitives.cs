// Original numeric primitives from origin/main (50c82ad), kept only for differential
// validation and before/after benchmarks. Do not use in production.
using Pidgin;

namespace Chrononuensis.Validation;

internal static class OriginalNumericPrimitives
{
    public static Parser<char, int> OneDigitParser(int min, int max) =>
        Parser.Digit.Repeat(1).Select(chars => int.Parse(new string(chars.ToArray())))
            .Assert(value => value >= min && value <= max, $"Value must be between {min} and {max}");

    public static Parser<char, int> OneDigitThenZeroParser(Func<int, int> func) =>
        Parser.Digit.Then(Parser.Char('0'), (digit, zero) => func(int.Parse(digit.ToString() + zero)));

    public static Parser<char, int> OneOrTwoDigitParser(int min, int max) =>
        Parser.Digit.Then(Parser.Try(Parser.Digit).Optional(),
        (first, second) => int.Parse(second.HasValue
                                ? $"{first}{second.Value}"
                                : $"{first}"))
                            .Assert(value => value >= min && value <= max, $"Value must be between {min} and {max}");

    public static Parser<char, int> OneToThreeDigitParser(int min, int max) =>
        Parser.Digit
            .Then(
                Parser.Try(
                        Parser.Digit.Then(
                            Parser.Try(Parser.Digit).Optional()
                            , (second, third) => third.HasValue ? $"{second}{third.Value}" : second.ToString()
                        ).Optional()
                )
                , (first, second) => int.Parse(second.HasValue
                                ? $"{first}{second.Value}"
                                : $"{first}"))
        .Assert(value => value >= min && value <= max, $"Value must be between {min} and {max}");


    public static Parser<char, int> TwoDigitParser(Func<int, int> normalize) =>
        Parser.Digit.Repeat(2).Select(chars => normalize(int.Parse(new string(chars.ToArray()))));

    public static Parser<char, int> TwoDigitParser() =>
        Parser.Digit.Repeat(2).Select(chars => int.Parse(new string(chars.ToArray())));

    public static Parser<char, int> TwoDigitParser(int min, int max) =>
        Parser.Digit.Repeat(2).Select(chars => int.Parse(new string(chars.ToArray())))
            .Assert(value => value >= min && value <= max, $"Value must be between {min} and {max}");

    public static Parser<char, int> ThreeDigitParser(int min, int max) =>
        Parser.Digit.Repeat(3).Select(chars => int.Parse(new string(chars.ToArray())))
            .Assert(value => value >= min && value <= max, $"Value must be between {min} and {max}");

    public static Parser<char, int> ThreeDigitThenZeroParser() =>
        Parser.Digit.Repeat(3).Then(Parser.Char('0'), (digits, zero) => int.Parse(new string(digits.ToArray()) + zero));

    public static Parser<char, int> FourDigitParser() =>
        Parser.Digit.Repeat(4).Select(chars => int.Parse(new string(chars.ToArray())));

}
