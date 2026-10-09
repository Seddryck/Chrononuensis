using System.Globalization;
using NUnit.Framework;

namespace Chrononuensis.Testing;

public class ParsingCultureTests
{
    [TestCase("2025-Jan", "yyyy-MMM", "", 1)]
    [TestCase("2025-January", "yyyy-MMMM", "en-US", 1)]
    [TestCase("2025-janv.", "yyyy-MMM", "fr-FR", 1)]
    [TestCase("2025-janvier", "yyyy-MMMM", "fr-FR", 1)]
    public void Parse_MonthName_UsesProvider(string input, string format, string cultureName, int expectedMonth)
    {
        var culture = string.IsNullOrEmpty(cultureName)
            ? CultureInfo.InvariantCulture
            : CultureInfo.GetCultureInfo(cultureName);

        Assert.That(YearMonth.Parse(input, format, culture), Is.EqualTo(new YearMonth(2025, expectedMonth)));
    }

    [Test]
    public void Parse_StringAndSpanOverloads_UseSameProvider()
    {
        var culture = CultureInfo.GetCultureInfo("fr-FR");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(YearMonth.Parse("2025-janvier", "yyyy-MMMM", culture),
                Is.EqualTo(new YearMonth(2025, 1)));
            Assert.That(YearMonth.Parse("2025-janvier".AsSpan(), "yyyy-MMMM", culture),
                Is.EqualTo(new YearMonth(2025, 1)));
            Assert.That(YearMonth.TryParse("2025-janv.", "yyyy-MMM", culture, out var fromString), Is.True);
            Assert.That(fromString, Is.EqualTo(new YearMonth(2025, 1)));
            Assert.That(YearMonth.TryParse("2025-janv.".AsSpan(), "yyyy-MMM", culture, out var fromSpan), Is.True);
            Assert.That(fromSpan, Is.EqualTo(new YearMonth(2025, 1)));
        }
    }

    [Test]
    public void Parse_LocalizedToken_UsesProvider()
        => Assert.That(Olympiad.Parse("I Olympiade", "{o:RN} {#Olympiad}", CultureInfo.GetCultureInfo("fr-FR")),
            Is.EqualTo(new Olympiad(1)));

    [Test]
    public void Parse_NullProvider_UsesCurrentUiCulture()
    {
        var originalCulture = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");
            Assert.That(YearMonth.Parse("2025-janv.", "yyyy-MMM", null),
                Is.EqualTo(new YearMonth(2025, 1)));
        }
        finally
        {
            CultureInfo.CurrentUICulture = originalCulture;
        }
    }

    [Test]
    public async Task Parse_ParallelCultures_AreIsolated()
    {
        var english = CultureInfo.GetCultureInfo("en-US");
        var french = CultureInfo.GetCultureInfo("fr-FR");

        var englishTask = Task.Run(() => Enumerable.Range(0, 100)
            .Select(_ => YearMonth.Parse("2025-January", "yyyy-MMMM", english))
            .ToArray());
        var frenchTask = Task.Run(() => Enumerable.Range(0, 100)
            .Select(_ => YearMonth.Parse("2025-janvier", "yyyy-MMMM", french))
            .ToArray());

        await Task.WhenAll(englishTask, frenchTask);

        Assert.That(englishTask.Result, Has.All.EqualTo(new YearMonth(2025, 1)));
        Assert.That(frenchTask.Result, Has.All.EqualTo(new YearMonth(2025, 1)));
    }
}
