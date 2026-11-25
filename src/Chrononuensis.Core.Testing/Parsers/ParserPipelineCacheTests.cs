using System.Globalization;
using Chrononuensis.Parsers;
using NUnit.Framework;

namespace Chrononuensis.Testing.Parsers;

public class ParserPipelineCacheTests
{
    [Test]
    public void ExplicitProvidersKeepCulturesAndNormalizationPoliciesIsolated()
    {
        var parser = new YearMonthParser();
        var french = new PeriodParsingOptions(CultureInfo.GetCultureInfo("fr-FR"), yearNormalizer: value => 2100 + value);
        var english = new PeriodParsingOptions(CultureInfo.GetCultureInfo("en-US"), yearNormalizer: value => 2000 + value);
        Assert.That(parser.Parse("25-janvier", "yy-MMMM", french), Is.EqualTo((2125, 1)));
        Assert.That(parser.Parse("25-January".AsSpan(), "yy-MMMM", english), Is.EqualTo((2025, 1)));
        Assert.That(parser.TryParse("25-janvier", "yy-MMMM", french, out var year, out var month), Is.True);
        Assert.That((year, month), Is.EqualTo((2125, 1)));
        Assert.That(parser.TryParse("25-January".AsSpan(), "yy-MMMM", english, out year, out month), Is.True);
        Assert.That((year, month), Is.EqualTo((2025, 1)));
    }

    [Test]
    public void ProvidersShareTokenizedFormatsButBuildSeparatePipelines()
    {
        var cache = new ParserPipelineCache();
        var format = "yy-MMMM";
        var french = cache.Get(format, CultureInfo.GetCultureInfo("fr-FR"));
        var english = cache.Get(new string(format.AsSpan()), CultureInfo.GetCultureInfo("en-US"));
        var implicitProvider = cache.Get(format);

        Assert.That(english.Tokens, Is.SameAs(french.Tokens));
        Assert.That(implicitProvider.Tokens, Is.SameAs(french.Tokens));
        Assert.That(english.Parser, Is.Not.SameAs(french.Parser));
        Assert.That(cache.Get(format, CultureInfo.GetCultureInfo("fr-FR")).Parser, Is.Not.SameAs(french.Parser));
    }

    [Test]
    public void ExplicitProviderTokenRetentionIsBoundedAndSkipsOversizedFormats()
    {
        var cache = new ParserPipelineCache();
        var provider = CultureInfo.InvariantCulture;
        var first = cache.Get("yyyy-MM", provider);
        for (var i = 0; i < ParserPipelineCache.Capacity; i++)
            cache.Get($"yyyy-MM '{i}'", provider);

        Assert.That(cache.Get("yyyy-MM", provider).Tokens, Is.Not.SameAs(first.Tokens));

        var literal = new string('x', ParserPipelineCache.MaximumFormatLength);
        var format = $"yyyy-MM '{literal}'";
        var large = cache.Get(format, provider);
        Assert.That(cache.Get(format, provider).Tokens, Is.Not.SameAs(large.Tokens));
        Assert.That(new YearMonthParser().Parse($"2025-06 {literal}".AsSpan(), format, provider), Is.EqualTo((2025, 6)));
    }

    [Test]
    public void RepeatedFormatsObserveChangesToAnExplicitProvider()
    {
        const string format = "yy-MMMM";
        var culture = (CultureInfo)CultureInfo.GetCultureInfo("en-US").Clone();
        var firstYear = 2000;
        var provider = new PeriodParsingOptions(culture, yearNormalizer: year => firstYear + year);
        var parser = new YearMonthParser();

        Assert.That(parser.Parse("25-January", format, provider), Is.EqualTo((2025, 1)));

        var monthNames = culture.DateTimeFormat.MonthNames;
        monthNames[0] = "FirstMonth";
        culture.DateTimeFormat.MonthNames = monthNames;
        firstYear = 2100;

        Assert.That(parser.TryParse("25-January", format, provider, out var year, out var month), Is.False);
        Assert.That((year, month), Is.EqualTo(((int?)null, (int?)null)));
        Assert.Throws<FormatException>(() => parser.Parse("25-January".AsSpan(), format, provider));
        Assert.That(parser.TryParse("25-FirstMonth".AsSpan(), format, provider, out year, out month), Is.True);
        Assert.That((year, month), Is.EqualTo((2125, 1)));
    }

    [Test]
    public void ConcurrentExplicitProvidersShareTokensAndKeepParsingIndependent()
    {
        const string format = "yy-MMMM 'concurrent'";
        var cache = new ParserPipelineCache();
        var pipelines = new ParserPipelineCache.Pipeline[100];
        var parser = new YearMonthParser();
        var french = new PeriodParsingOptions(CultureInfo.GetCultureInfo("fr-FR"), yearNormalizer: year => 2100 + year);
        var english = new PeriodParsingOptions(CultureInfo.GetCultureInfo("en-US"), yearNormalizer: year => 2000 + year);
        Parallel.For(0, pipelines.Length, i =>
        {
            var useFrench = i % 2 == 0;
            var provider = useFrench ? french : english;
            var input = useFrench ? "25-janvier concurrent" : "25-January concurrent";
            var expected = useFrench ? (2125, 1) : (2025, 1);

            pipelines[i] = cache.Get(format, provider);
            Assert.That(parser.Parse(input, format, provider), Is.EqualTo(expected));
            Assert.That(parser.Parse(input.AsSpan(), format, provider), Is.EqualTo(expected));
            Assert.That(parser.TryParse(input, format, provider, out var year, out var month), Is.True);
            Assert.That((year, month), Is.EqualTo(expected));
            Assert.That(parser.TryParse(input.AsSpan(), format, provider, out year, out month), Is.True);
            Assert.That((year, month), Is.EqualTo(expected));
        });
        Assert.That(pipelines.All(p => ReferenceEquals(p.Tokens, pipelines[0].Tokens)), Is.True);
    }

    [Test]
    public void CachedAndExplicitProviderPipelinesRejectTrailingInput()
    {
        var parser = new YearMonthParser();
        foreach (var provider in new IFormatProvider?[] { null, CultureInfo.InvariantCulture })
        {
            Assert.Throws<FormatException>(() => parser.Parse("2025-06-extra", "yyyy-MM", provider));
            Assert.Throws<FormatException>(() => parser.Parse("2025-06-extra".AsSpan(), "yyyy-MM", provider));
            Assert.That(parser.TryParse("2025-06-extra", "yyyy-MM", provider, out _, out _), Is.False);
            Assert.That(parser.TryParse("2025-06-extra".AsSpan(), "yyyy-MM", provider, out _, out _), Is.False);
        }
    }
    [Test]
    public void RepeatedFormatsReuseTokensAndParser()
    {
        var cache = new ParserPipelineCache();
        var first = cache.Get("yyyy-MM");
        var second = cache.Get(new string("yyyy-MM".ToCharArray()));
        Assert.That(second, Is.SameAs(first));
        Assert.That(cache.Get("yyyy-M"), Is.Not.SameAs(first));
    }

    [Test]
    public void RetentionIsBoundedAndOversizedFormatsAreNotRetained()
    {
        var cache = new ParserPipelineCache();
        var first = cache.Get("yyyy-MM");
        for (var i = 0; i < ParserPipelineCache.Capacity; i++)
            cache.Get($"yyyy-MM '{i}'");
        Assert.That(cache.Get("yyyy-MM"), Is.Not.SameAs(first));
        var large = "yyyy '" + new string('x', ParserPipelineCache.MaximumFormatLength) + "'";
        var oversized = cache.Get(large);
        Assert.That(cache.Get(large), Is.Not.SameAs(oversized));
    }

    [Test]
    public void LocalizedFormatsUseCurrentUICultureAndCanSwitchBack()
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            var cache = new ParserPipelineCache();
            var parser = new OlympiadParser();
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");
            var french = cache.Get("o {#Olympiad}");
            Assert.That(parser.Parse("24 Olympiade", "o {#Olympiad}", null), Is.EqualTo(24));
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("es-ES");
            Assert.That(cache.Get("o {#Olympiad}"), Is.Not.SameAs(french));
            Assert.That(parser.Parse("24 Olimpiada", "o {#Olympiad}", null), Is.EqualTo(24));
            Assert.That(parser.TryParse("24 Olympiade", "o {#Olympiad}", null, out _), Is.False);
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");
            Assert.That(cache.Get("o {#Olympiad}"), Is.SameAs(french));
        }
        finally { CultureInfo.CurrentUICulture = previous; }
    }

    [Test]
    public void StringAndSpanParseAndTryParseShareThePipeline()
    {
        var parser = new YearMonthParser();
        Assert.That(parser.Parse("2025-06", "yyyy-MM", null), Is.EqualTo((2025, 6)));
        Assert.That(parser.Parse("2025-06".AsSpan(), "yyyy-MM", null), Is.EqualTo((2025, 6)));
        Assert.That(parser.TryParse("2025-06", "yyyy-MM", null, out var year, out var month), Is.True);
        Assert.That((year, month), Is.EqualTo((2025, 6)));
        Assert.That(parser.TryParse("2025-06".AsSpan(), "yyyy-MM", null, out year, out month), Is.True);
        Assert.That((year, month), Is.EqualTo((2025, 6)));
        Assert.That(parser.TryParse("2025-99".AsSpan(), "yyyy-MM", null, out year, out month), Is.False);
        Assert.That((year, month), Is.EqualTo(((int?)null, (int?)null)));
    }

    [Test]
    public void ConcurrentMissesReuseOnePipelineAndParsingHasIndependentResults()
    {
        var cache = new ParserPipelineCache();
        var pipelines = new ParserPipelineCache.Pipeline[100];
        var parser = new YearMonthParser();
        Parallel.For(0, pipelines.Length, i =>
        {
            pipelines[i] = cache.Get("yyyy-MM");
            var month = i % 12 + 1;
            var input = $"2025-{month:00}";
            Assert.That(parser.Parse(input.AsSpan(), "yyyy-MM", null), Is.EqualTo((2025, month)));
            Assert.That(parser.TryParse(input, "yyyy-MM", null, out var year, out var parsedMonth), Is.True);
            Assert.That((year, parsedMonth), Is.EqualTo((2025, month)));
        });
        Assert.That(pipelines.All(p => ReferenceEquals(p, pipelines[0])), Is.True);
    }
}

