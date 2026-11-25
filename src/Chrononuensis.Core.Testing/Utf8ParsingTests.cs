using System.Globalization;
using System.Text;
using Chrononuensis.Parsers;
using NUnit.Framework;

namespace Chrononuensis.Testing;

public class Utf8ParsingTests
{
    [Test]
    public void DefaultFormats_ParseThroughUtf8Interface()
    {
        using (Assert.EnterMultipleScope())
        {
            AssertDefaultFormat("02-14"u8, new MonthDay(2, 14));
            AssertDefaultFormat("2025-045"u8, new YearDay(2025, 45));
            AssertDefaultFormat("2025-W07"u8, new YearWeek(2025, 7));
            AssertDefaultFormat("2025-02"u8, new YearMonth(2025, 2));
            AssertDefaultFormat("2025-Q1"u8, new YearQuarter(2025, 1));
            AssertDefaultFormat("2025-H1"u8, new YearSemester(2025, 1));
            AssertDefaultFormat("2025"u8, new Year(2025));
            AssertDefaultFormat("IV Olympiad"u8, new Olympiad(4));
            AssertDefaultFormat("2020s"u8, new Decade(2020));
            AssertDefaultFormat("20"u8, new Century(20));
        }
    }

    private static void AssertDefaultFormat<T>(ReadOnlySpan<byte> input, T expected)
        where T : struct, IUtf8SpanParsable<T>
    {
        var culture = CultureInfo.InvariantCulture;
        var parsed = T.Parse(input, culture);
        var success = T.TryParse(input, culture, out var result);

        Assert.That(parsed, Is.EqualTo(expected), $"{typeof(T).Name}.Parse");
        Assert.That(success, Is.True, $"{typeof(T).Name}.TryParse success");
        Assert.That(result, Is.EqualTo(expected), $"{typeof(T).Name}.TryParse result");
    }

    [TestCase(255)]
    [TestCase(256)]
    [TestCase(257)]
    public void CustomFormat_ParsesSupplementaryCharacterAcrossBufferCutoff(int byteLength)
    {
        const string prefix = "2025-02";
        const string symbol = "🕒";
        var literal = symbol + new string('x', byteLength - prefix.Length - Encoding.UTF8.GetByteCount(symbol));
        var input = Encoding.UTF8.GetBytes(prefix + literal);
        var format = $"yyyy-MM'{literal}'";
        var parser = new YearMonthParser();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(input, Has.Length.EqualTo(byteLength));
            Assert.That(parser.Parse(input, format, null), Is.EqualTo((2025, 2)));
            Assert.That(parser.TryParse(input, format, null, out var year, out var month), Is.True);
            Assert.That(year, Is.EqualTo(2025));
            Assert.That(month, Is.EqualTo(2));
        }
    }

    [TestCase(0x80)]
    [TestCase(0xC3)]
    public void PooledInput_MalformedSuffix_IsRejected(int invalidByte)
    {
        var literal = new string('x', 250);
        var input = Encoding.UTF8.GetBytes("2025-02" + literal + "?");
        input[^1] = (byte)invalidByte;
        var format = $"yyyy-MM'{literal}\uFFFD'";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(() => YearMonth.Parse(input, format),
                Throws.TypeOf<FormatException>().With.Message.EqualTo("Input is not valid UTF-8."));
            Assert.That(YearMonth.TryParse(input, format, null, out var result), Is.False);
            Assert.That(result.Year, Is.Zero);
            Assert.That(result.Month, Is.Zero);
        }
    }

    [Test]
    public void EmptyInput_ThrowsOrClearsParserOutputs()
    {
        const string format = "yyyy-MM";
        var parser = new YearMonthParser();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(() => parser.Parse(ReadOnlySpan<byte>.Empty, format, null), Throws.TypeOf<FormatException>());
            Assert.That(parser.TryParse(ReadOnlySpan<byte>.Empty, format, null, out var year, out var month), Is.False);
            Assert.That(year, Is.Null);
            Assert.That(month, Is.Null);
        }
    }
}
