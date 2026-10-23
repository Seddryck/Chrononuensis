using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Localization;
using Pidgin;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chrononuensis.Parsers.Internals;
internal partial class Primitives
{
    // Keep the original characters in a value type until all required digits have
    // parsed. This preserves Pidgin's Unicode digit acceptance and failure position:
    // conversion of a complete non-ASCII number still throws as int.Parse did.
    private static readonly Parser<char, ulong> OneDigit = Parser.Digit.Select(c => (ulong)c);
    private static readonly Parser<char, ulong> TwoDigits = OneDigit.Then(Parser.Digit, AppendDigit);
    private static readonly Parser<char, ulong> ThreeDigits = TwoDigits.Then(Parser.Digit, AppendDigit);
    private static readonly Parser<char, ulong> FourDigits = ThreeDigits.Then(Parser.Digit, AppendDigit);

    private static ulong AppendDigit(ulong digits, char digit) => (digits << 16) | digit;

    private static int AccumulateDigits(ulong digits)
    {
        var original = digits;
        var shift = 48;
        while ((digits >> shift) == 0)
            shift -= 16;

        var value = 0;
        for (; shift >= 0; shift -= 16)
        {
            var digit = (char)(digits >> shift);
            if ((uint)(digit - '0') > 9)
                return ConvertNonAsciiDigits(original);
            value = value * 10 + (digit - '0');
        }
        return value;
    }

    private static int ConvertNonAsciiDigits(ulong digits)
    {
        // Exceptional path only: preserve int.Parse's FormatException and message,
        // after the complete primitive has parsed, including mixed Unicode/ASCII.
        Span<char> characters = stackalloc char[4];
        var start = characters.Length;
        while (digits != 0)
        {
            characters[--start] = (char)digits;
            digits >>= 16;
        }
        return int.Parse(characters[start..]);
    }

    public static Parser<char, int> OneDigitParser(int min, int max) =>
        OneDigit.Select(AccumulateDigits)
            .Assert(value => value >= min && value <= max, $"Value must be between {min} and {max}");

    public static Parser<char, int> OneDigitThenZeroParser(Func<int, int> func) =>
        OneDigit.Then(Parser.Char('0'), AppendDigit).Select(digits => func(AccumulateDigits(digits)));

    public static Parser<char, int> OneOrTwoDigitParser(int min, int max) =>
        OneDigit.Then(Parser.Try(Parser.Digit).Optional(),
            (first, second) => second.HasValue ? AppendDigit(first, second.Value) : first)
            .Select(AccumulateDigits)
            .Assert(value => value >= min && value <= max, $"Value must be between {min} and {max}");

    public static Parser<char, int> OneToThreeDigitParser(int min, int max) =>
        OneDigit.Then(
            Parser.Try(OneDigit.Then(Parser.Try(Parser.Digit).Optional(),
                (second, third) => third.HasValue ? AppendDigit(second, third.Value) : second).Optional()),
            (first, second) => !second.HasValue ? first
                : second.Value <= char.MaxValue ? (first << 16) | second.Value
                : (first << 32) | second.Value)
            .Select(AccumulateDigits)
            .Assert(value => value >= min && value <= max, $"Value must be between {min} and {max}");

    public static Parser<char, int> TwoDigitParser(Func<int, int> normalize) =>
        TwoDigits.Select(digits => normalize(AccumulateDigits(digits)));

    public static Parser<char, int> TwoDigitParser() => TwoDigits.Select(AccumulateDigits);

    public static Parser<char, int> TwoDigitParser(int min, int max) =>
        TwoDigitParser()
            .Assert(value => value >= min && value <= max, $"Value must be between {min} and {max}");

    public static Parser<char, int> ThreeDigitParser(int min, int max) =>
        ThreeDigits.Select(AccumulateDigits)
            .Assert(value => value >= min && value <= max, $"Value must be between {min} and {max}");

    public static Parser<char, int> ThreeDigitThenZeroParser() =>
        ThreeDigits.Then(Parser.Char('0'), AppendDigit).Select(AccumulateDigits);

    public static Parser<char, int> FourDigitParser() => FourDigits.Select(AccumulateDigits);

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
        => Parser.OneOf(values.Select(word => Parser.Try(Parser.String(word)))).ThenReturn(Unit.Value);

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

    public static Parser<char, Unit> LocalizedParser(string key, CultureInfo? culture = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        var value = LocalizedStrings.Get(key, culture ?? CultureInfo.CurrentUICulture);
        return Parser.String(value).IgnoreResult();
    }
}
