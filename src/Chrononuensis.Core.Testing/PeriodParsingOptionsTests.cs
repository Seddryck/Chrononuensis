using NUnit.Framework;

namespace Chrononuensis.Testing;

public class PeriodParsingOptionsTests
{
    [Test]
    public void Parse_CustomYearNormalizer_AppliesToNumericAndRomanShortForms()
    {
        var options = new PeriodParsingOptions(yearNormalizer: value => 2100 + value);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(Year.Parse("25", "yy", options), Is.EqualTo(new Year(2125)));
            Assert.That(Year.Parse("XXV", "{yy:RN}", options), Is.EqualTo(new Year(2125)));
        }
    }

    [Test]
    public void Parse_CustomDecadeNormalizer_AppliesToShortForm()
    {
        var options = new PeriodParsingOptions(decadeNormalizer: value => 2100 + value);

        Assert.That(Decade.Parse("20s", "tt's'", options), Is.EqualTo(new Decade(2120)));
    }

    [Test]
    public async Task Parse_ConcurrentPolicies_AreIsolated()
    {
        var recent = new PeriodParsingOptions(
            yearNormalizer: value => 2000 + value,
            decadeNormalizer: value => 2000 + value);
        var future = new PeriodParsingOptions(
            yearNormalizer: value => 2100 + value,
            decadeNormalizer: value => 2100 + value);

        var recentTask = Task.Run(() => Enumerable.Range(0, 100)
            .Select(_ => (Year.Parse("25", "yy", recent), Decade.Parse("20s", "tt's'", recent)))
            .ToArray());
        var futureTask = Task.Run(() => Enumerable.Range(0, 100)
            .Select(_ => (Year.Parse("25", "yy", future), Decade.Parse("20s", "tt's'", future)))
            .ToArray());

        await Task.WhenAll(recentTask, futureTask);

        Assert.That(recentTask.Result, Has.All.EqualTo((new Year(2025), new Decade(2020))));
        Assert.That(futureTask.Result, Has.All.EqualTo((new Year(2125), new Decade(2120))));
    }
}
