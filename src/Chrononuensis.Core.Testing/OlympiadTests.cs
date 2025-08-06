using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Chrononuensis.Testing;
public class OlympiadTests
{
    [Test]
    public void Parse_InputDefaultFormat_Equal()
        => Assert.That(Olympiad.Parse("IV Olympiad", "{o:RN} 'Olympiad'"), Is.EqualTo(new Olympiad(4)));

    private static IEnumerable<Olympiad> GetData()
    {
        yield return new(1);
        yield return new(6);
        yield return new(11);
        yield return new(24);
    }

    [TestCaseSource(nameof(GetData))]
    public void LessThan_Olympiad_Compared(Olympiad right)
        => Assert.That(right < new Olympiad(25), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void LessThanOrEqual_Olympiad_Compared(Olympiad right)
        => Assert.That(right <= new Olympiad(24), Is.True);

    [Test]
    public void LessThanOrEqual_Olympiad_Compared()
        => Assert.That(new Olympiad(24) <= new Olympiad(24), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThan_Olympiad_Compared(Olympiad right)
        => Assert.That(new Olympiad(25) > right, Is.True);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThanOrEqual_Olympiad_Compared(Olympiad right)
        => Assert.That(new Olympiad(24) >= right, Is.True);

    [Test]
    public void GreaterThanOrEqual_Olympiad_Compared()
        => Assert.That(new Olympiad(25) >= new Olympiad(25), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void NotEqual_Olympiad_Compared(Olympiad right)
        => Assert.That(new Olympiad(25) != right, Is.True);

    [Test]
    public void Equal_Olympiad_Compared()
        => Assert.That(new Olympiad(25) == new Olympiad(25), Is.True);

    [Test]
    public void CompareTo_Null_Compared()
        => Assert.That(new Olympiad(25).CompareTo(null), Is.EqualTo(1));

    [Test]
    public void CompareTo_ItSelf_Compared()
        => Assert.That(new Olympiad(25).CompareTo(new Olympiad(25)), Is.EqualTo(0));

    [Test]
    public void CompareTo_SomethingElse_Compared()
        => Assert.That(new Olympiad(22).CompareTo(new Olympiad(25)), Is.EqualTo(-1));

    [Test]
    [TestCase("XXV Olympiad", true)]
    [TestCase("Paris Olympiad", false)]
    public void TryParse_SomeValue_Expected(string input, bool expected)
    {
        var result = Olympiad.TryParse(input, null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new Olympiad(25)));
        });
    }

    [Test]
    [TestCase("XXV Olympiad", true)]
    [TestCase("Paris", false)]
    public void TryParse_SomeValueWithFormat_Expected(string input, bool expected)
    {
        var result = Olympiad.TryParse(input, "{o:RN} 'Olympiad'", null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new Olympiad(25)));
        });
    }

    [Test]
    [TestCase("XXV Olympiad")]
    public void Parse_SomeValueAsSpan_Expected(string input)
    {
        var value = Olympiad.Parse(input.AsSpan(), null);
        Assert.That(value, Is.EqualTo(new Olympiad(25)));
    }

    [Test]
    [TestCase("XXV Olympiad", true)]
    [TestCase("Paris", false)]
    public void TryParse_SomeValueAsSpan_Expected(string input, bool expected)
    {
        var result = Olympiad.TryParse(input.AsSpan(), null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new Olympiad(25)));
        });
    }

    [Test]
    [TestCase("25th Olympiad", true)]
    [TestCase("Paris", false)]
    public void TryParse_SomeValueWithFormatAsSpan_Expected(string input, bool expected)
    {
        var result = Olympiad.TryParse(input.AsSpan(), "o[st|nd|rd|th] 'Olympiad'", null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new Olympiad(25)));
        });
    }

    [Test]
    [TestCase("XXV Olympiad", "{o:RN} 'Olympiad'")]
    [TestCase("25 Olympiad", "o 'Olympiad'")]
    [TestCase("25th Olympiad", "o[st|nd|rd|th] 'Olympiad'")]
    public void Parse_SomeValueFormatAsSpan_Expected(string input, string format)
    {
        var value = Olympiad.Parse(input.AsSpan(), format, null);
        Assert.That(value, Is.EqualTo(new Olympiad(25)));
    }

    [Test]
    [TestCase("I Olympiad", 365 * 4 + 1)]
    [TestCase("II Olympiad", 365 * 4)]
    [TestCase("XXVII Olympiad", 365 * 4 + 1)]
    public void Days_SomeValue_Expected(string input, int expected)
    {
        var value = Olympiad.Parse(input, null);
        Assert.That(value.Days, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("XXVII Olympiad", "2000-01-01")]
    public void FirstDate_SomeValue_Expected(string input, DateOnly expected)
    {
        var value = Olympiad.Parse(input, null);
        Assert.That(value.FirstDate, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("XXVII Olympiad", "2003-12-31")]
    public void LastDate_SomeValue_Expected(string input, DateOnly expected)
    {
        var value = Olympiad.Parse(input, null);
        Assert.That(value.LastDate, Is.EqualTo(expected));
    }

    [TestCase("XXVII Olympiad", "2000-01-01")]
    public void LowerBound_SomeValue_Expected(string input, DateTime expected)
    {
        var value = Olympiad.Parse(input, null);
        Assert.That(value.LowerBound, Is.EqualTo(expected));
    }

    [TestCase("XXVII Olympiad", "2004-01-01")]
    public void UpperBound_SomeValue_Expected(string input, DateTime expected)
    {
        var value = Olympiad.Parse(input, null);
        Assert.That(value.UpperBound, Is.EqualTo(expected));
    }
}
