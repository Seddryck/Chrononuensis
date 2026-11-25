using System.Globalization;
using System.Text;
using Chrononuensis.Formats.Tokens;
using Chrononuensis.Parsers;
using NUnit.Framework;

namespace Chrononuensis.Testing.Parsers;

public class TypedPeriodParserTests
{
    [Test]
    public void SingleComponentReturnsTheFirstMatchAndConsumesRemainingTokens()
    {
        const string format = "'year 'yyyy[ / | - ]yy-MM";
        const string input = "year 2025/26-06";
        var parser = new YearParser();

        Assert.That(parser.Parse(input, format, null), Is.EqualTo(2025));
        Assert.That(parser.Parse(input.AsSpan(), format, null), Is.EqualTo(2025));
        Assert.That(parser.TryParse(input, format, null, out var year), Is.True);
        Assert.That(year, Is.EqualTo(2025));
        Assert.That(parser.TryParse(input.AsSpan(), format, null, out year), Is.True);
        Assert.That(year, Is.EqualTo(2025));
        Assert.That(parser.TryParse("year 2025/26-99".AsSpan(), format, null, out year), Is.False);
        Assert.That(year, Is.Null);
    }

    [Test]
    public void SingleComponentTryParseDistinguishesMissingComponentFromInvalidInput()
    {
        var parser = new YearParser();

        Assert.That(parser.TryParse("2025-extra", "yyyy", null, out var year), Is.False);
        Assert.That(year, Is.Null);
        Assert.That(parser.TryParse("zz".AsSpan(), "MM", null, out year), Is.False);
        Assert.That(year, Is.Null);
        Assert.That(Assert.Throws<FormatException>(() => parser.Parse("06", "MM", null))!.Message,
            Is.EqualTo("Token 'year' not found in the format"));
        Assert.That(Assert.Throws<FormatException>(() => parser.TryParse("06".AsSpan(), "MM", null, out _))!.Message,
            Is.EqualTo("Token 'year' not found in the format"));
    }

    [Test]
    public void TuplesFollowComponentOrderAndUseTheFirstMatch()
    {
        const string input = "06-2025-07-2026";
        const string format = "MM-yyyy-MM-yyyy";
        var parser = new YearMonthParser();

        Assert.That(parser.Parse(input, format, null), Is.EqualTo((2025, 6)));
        Assert.That(parser.Parse(input.AsSpan(), format, null), Is.EqualTo((2025, 6)));
        Assert.That(parser.Parse("06-2025-07-2026"u8, format, null), Is.EqualTo((2025, 6)));
        Assert.That(parser.TryParse(input, format, null, out var year, out var month), Is.True);
        Assert.That((year, month), Is.EqualTo((2025, 6)));
        Assert.That(parser.TryParse(input.AsSpan(), format, null, out year, out month), Is.True);
        Assert.That((year, month), Is.EqualTo((2025, 6)));
        Assert.That(parser.TryParse("06-2025-07-2026"u8, format, null, out year, out month), Is.True);
        Assert.That((year, month), Is.EqualTo((2025, 6)));
    }

    [TestCase("2025", "yyyy", "month")]
    [TestCase("06", "MM", "year")]
    [TestCase("literal", "'literal'", "year")]
    public void SuccessfulInputWithMissingComponentsThrowsForAllOverloads(
        string input, string format, string missing)
    {
        var parser = new YearMonthParser();
        var bytes = Encoding.UTF8.GetBytes(input);
        var message = $"Token '{missing}' not found in the format";
        Assert.That(Assert.Throws<FormatException>(() => parser.Parse(input, format, null))!.Message, Is.EqualTo(message));
        Assert.That(Assert.Throws<FormatException>(() => parser.Parse(input.AsSpan(), format, null))!.Message, Is.EqualTo(message));
        Assert.That(Assert.Throws<FormatException>(() => parser.Parse(bytes, format, null))!.Message, Is.EqualTo(message));
        Assert.That(Assert.Throws<FormatException>(() => parser.TryParse(input, format, null, out _, out _))!.Message, Is.EqualTo(message));
        Assert.That(Assert.Throws<FormatException>(() => parser.TryParse(input.AsSpan(), format, null, out _, out _))!.Message, Is.EqualTo(message));
        Assert.That(Assert.Throws<FormatException>(() => parser.TryParse(bytes, format, null, out _, out _))!.Message, Is.EqualTo(message));
    }

    [TestCase("2025-99", "yyyy-MM")]
    [TestCase("2025-june!", "yyyy-MMMM")]
    [TestCase("2025!", "yyyy")]
    [TestCase("xx25", "yyyy")]
    [TestCase("06-2025-99", "MM-yyyy-MM")]
    public void InputErrorsMatchLegacyPipelineAndClearTryParseOutputs(string input, string format)
    {
        var parser = new YearMonthParser();
        var provider = CultureInfo.InvariantCulture;
        Type[] components = [typeof(IYearToken), typeof(IMonthToken)];
        var legacy = Assert.Throws<FormatException>(() => parser.ParseInternal(input, format, provider, components));

        Assert.That(Assert.Throws<FormatException>(() => parser.Parse(input, format, provider))!.Message, Is.EqualTo(legacy!.Message));
        Assert.That(Assert.Throws<FormatException>(() => parser.Parse(input.AsSpan(), format, provider))!.Message, Is.EqualTo(legacy.Message));
        Assert.That(parser.TryParse(input, format, provider, out var year, out var month), Is.False);
        Assert.That((year, month), Is.EqualTo(((int?)null, (int?)null)));
        Assert.That(parser.TryParse(input.AsSpan(), format, provider, out year, out month), Is.False);
        Assert.That((year, month), Is.EqualTo(((int?)null, (int?)null)));
    }

    [Test]
    public void PipelinesWithTheSameFormatAndResultTypeKeepTheirOwnComponentMapping()
    {
        const string input = "2025-06-17";
        const string format = "yyyy-MM-dd";
        var yearMonth = new YearMonthParser();
        var monthDay = new MonthDayParser();

        Assert.That(yearMonth.Parse(input.AsSpan(), format, null), Is.EqualTo((2025, 6)));
        Assert.That(monthDay.Parse(input.AsSpan(), format, null), Is.EqualTo((6, 17)));
        Assert.That(yearMonth.Parse(input.AsSpan(), format, null), Is.EqualTo((2025, 6)));

        Type[] components = [typeof(IDayToken), typeof(IYearToken), typeof(IDayToken)];
        Assert.That(yearMonth.ParseInternal(input, format, null, components), Is.EqualTo(new object[] { 17, 2025, 17 }));
        Assert.That(yearMonth.TryParseInternal(input.AsSpan(), format, null, components, out var result), Is.True);
        Assert.That(result, Is.EqualTo(new object[] { 17, 2025, 17 }));
    }

    [Test]
    public void DerivedParsersCanStillUseTheLegacyUtf8Overloads()
    {
        var parser = new LegacyUtf8Parser();

        Assert.That(parser.Parse("2025"u8), Is.EqualTo(new object[] { 2025 }));
        Assert.That(parser.TryParse("2025"u8, out var result), Is.True);
        Assert.That(result, Is.EqualTo(new object[] { 2025 }));
        Assert.That(parser.TryParse(new byte[] { 0xFF }, out result), Is.False);
        Assert.That(result, Is.Null);
        Assert.That(Assert.Throws<FormatException>(() => parser.Parse(new byte[] { 0xFF }))!.Message,
            Is.EqualTo("Input is not valid UTF-8."));
    }

    private sealed class LegacyUtf8Parser : ChrononuensisParser
    {
        private static readonly Type[] Components = [typeof(IYearToken)];

        internal object[] Parse(ReadOnlySpan<byte> input)
            => ParseInternal(input, "yyyy", null, Components);

        internal bool TryParse(ReadOnlySpan<byte> input, out object[]? results)
            => TryParseInternal(input, "yyyy", null, Components, out results);
    }

    [Test]
    public void ExplicitProviderStillProcessesRepeatedNumericTokens()
    {
        var normalized = new List<int>();
        var provider = new PeriodParsingOptions(CultureInfo.GetCultureInfo("fr-FR"), yearNormalizer: year =>
        {
            normalized.Add(year);
            return 2100 + year;
        });

        var parser = new YearMonthParser();
        Assert.That(parser.Parse("janvier-25-26"u8, "MMMM-yy-yy", provider), Is.EqualTo((2125, 1)));
        Assert.That(normalized, Is.EqualTo(new[] { 25, 26 }));
    }
}
