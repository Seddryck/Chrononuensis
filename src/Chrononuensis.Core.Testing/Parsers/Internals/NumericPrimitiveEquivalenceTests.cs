using Chrononuensis.Parsers.Internals;
using Chrononuensis.Validation;
using NUnit.Framework;
using Pidgin;
using System.Globalization;

namespace Chrononuensis.Testing.Parsers.Internals;

public class NumericPrimitiveEquivalenceTests
{
    internal static (Parser<char, int> Before, Parser<char, int> After) Parsers(string kind) => kind switch
    {
        "one" => (OriginalNumericPrimitives.OneDigitParser(1, 4), Primitives.OneDigitParser(1, 4)),
        "one-zero" => (OriginalNumericPrimitives.OneDigitThenZeroParser(x => x + 1900), Primitives.OneDigitThenZeroParser(x => x + 1900)),
        "one-two" => (OriginalNumericPrimitives.OneOrTwoDigitParser(1, 53), Primitives.OneOrTwoDigitParser(1, 53)),
        "one-three" => (OriginalNumericPrimitives.OneToThreeDigitParser(1, 366), Primitives.OneToThreeDigitParser(1, 366)),
        "two" => (OriginalNumericPrimitives.TwoDigitParser(), Primitives.TwoDigitParser()),
        "two-range" => (OriginalNumericPrimitives.TwoDigitParser(1, 12), Primitives.TwoDigitParser(1, 12)),
        "two-normalize" => (OriginalNumericPrimitives.TwoDigitParser(x => x < 40 ? x + 2000 : x + 1900), Primitives.TwoDigitParser(x => x < 40 ? x + 2000 : x + 1900)),
        "three" => (OriginalNumericPrimitives.ThreeDigitParser(1, 366), Primitives.ThreeDigitParser(1, 366)),
        "three-zero" => (OriginalNumericPrimitives.ThreeDigitThenZeroParser(), Primitives.ThreeDigitThenZeroParser()),
        "four" => (OriginalNumericPrimitives.FourDigitParser(), Primitives.FourDigitParser()),
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };

    [TestCase("one")]
    [TestCase("one-zero")]
    [TestCase("one-two")]
    [TestCase("one-three")]
    [TestCase("two")]
    [TestCase("two-range")]
    [TestCase("two-normalize")]
    [TestCase("three")]
    [TestCase("three-zero")]
    [TestCase("four")]
    public void MatchesOriginalValuesFailuresAndExceptions(string kind)
    {
        var (before, after) = Parsers(kind);
        var inputs = new HashSet<string> { "", "-1", " 1", "+1", "abc", "9999", "1000", "12345" };
        for (var i = 0; i <= 1000; i++)
        {
            var number = i.ToString(CultureInfo.InvariantCulture);
            inputs.Add(number);
            inputs.Add(number.PadLeft(4, '0'));
            inputs.Add(number + "x");
        }
        // Cover every UTF-16 character Pidgin accepts as a digit, at every position,
        // including incomplete prefixes and subsequent invalid characters.
        for (var c = 0; c <= char.MaxValue; c++)
        {
            if (!char.IsDigit((char)c)) continue;
            for (var position = 0; position < 4; position++)
            {
                var prefix = new string('1', position) + (char)c;
                inputs.Add(prefix);
                inputs.Add(prefix + "x");
                inputs.Add(prefix.PadRight(4, '0'));
            }
        }
        foreach (var input in inputs)
        {
            Assert.That(Observe(after, input, false), Is.EqualTo(Observe(before, input, false)), $"{kind}: {input}");
            Assert.That(Observe(after, input, true), Is.EqualTo(Observe(before, input, true)), $"{kind} span: {input}");
        }
    }

    [TestCase("en-US")]
    [TestCase("fr-FR")]
    [TestCase("ar-SA")]
    public void UnicodeConversionExceptionsMatchUnderDifferentCultures(string culture)
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            var (before, after) = Parsers("four");
            foreach (var input in new[] { "2025", "٢٠٢٥", "２０２５", "2٠25", "٢٠x", "٢٠" })
                Assert.That(Observe(after, input, false), Is.EqualTo(Observe(before, input, false)));
        }
        finally { CultureInfo.CurrentCulture = previous; }
    }

    private static string Observe(Parser<char, int> parser, string input, bool span)
    {
        try
        {
            var result = span ? parser.Parse(input.AsSpan()) : parser.Parse(input);
            return result.Success ? $"value:{result.Value}" : $"failure:{result.Error}";
        }
        catch (FormatException ex) { return $"{ex.GetType().Name}:{ex.Message}"; }
    }
}
