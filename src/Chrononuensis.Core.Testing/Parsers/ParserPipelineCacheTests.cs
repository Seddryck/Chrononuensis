using System.Globalization;
using Chrononuensis.Parsers;
using NUnit.Framework;

namespace Chrononuensis.Testing.Parsers;

public class ParserPipelineCacheTests
{
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

