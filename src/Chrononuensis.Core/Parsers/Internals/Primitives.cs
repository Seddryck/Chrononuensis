using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Pidgin;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chrononuensis.Parsers.Internals;
internal partial class Primitives
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

    public static Parser<char, int> ListParser(string[] values)
    {
        var parsers = values
                        .Where(abbr => !string.IsNullOrEmpty(abbr))
                        .Select((abbr, index) => new { abbr, index = index + 1 })
                        .Select(entry => Parser.Try(Parser.String(entry.abbr)).Select(_ => entry.index));

        return Parser.OneOf(parsers);
    }

    public static Parser<char, Unit> CharParser(char c)
        => Parser.Char(c).IgnoreResult();

    public static Parser<char, Unit> StringParser(string str)
        => Parser.String(str).IgnoreResult();

    public static Parser<char, Unit> StringParsers(string[] values)
        => Parser.OneOf(values.Select(Parser.String)).ThenReturn(Unit.Value);

    public static Parser<char, object[]> CombineParsers(params Parser<char, object>[] parsers)
    {
        if (parsers == null || parsers.Length == 0)
            throw new ArgumentException("At least one parser must be provided.", nameof(parsers));

        // Start with the first parser
        var combinedParser = parsers[0].Select(result => new List<object>() { result });

        // Aggregate the rest
        for (var i = 1; i < parsers.Length; i++)
        {
            var parser = parsers[i];
            combinedParser = combinedParser
                .Then(parser, (results, result) =>
                {
                    results.Add(result);
                    return results;
                });
        }

        // Convert List<object> to object[] at the end
        return combinedParser.Select(results => results.ToArray());
    }
}
