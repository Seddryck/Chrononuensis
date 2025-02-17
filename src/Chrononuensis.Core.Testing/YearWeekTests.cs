using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chrononuensis.Testing;
public class YearWeekTests
{
    [TestCase(2004, 12)]
    [TestCase(2004, 53)]
    [TestCase(2005, 52)]
    public void Ctor_ValidValues_Expected(int year, int week)
        => Assert.DoesNotThrow(() => new YearWeek(year, week));

    [TestCase(2004, 54, 53)]
    [TestCase(2005, 53, 52)]
    public void Ctor_InvalidValues_Throws(int year, int week, int max)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new YearWeek(year, week));
        Assert.That(ex.ParamName, Is.EqualTo("Week"));
        Assert.That(ex.Message, Does.StartWith($"When year is {year}"));
        Assert.That(ex.Message, Does.Contain($"1 and {max}"));
    }

    [Test]
    public void Parse_InputDefaultFormat_Equal()
        => Assert.That(YearWeek.Parse("2021-W16", "yyyy-'W'ww"), Is.EqualTo(new YearWeek(2021, 16)));

    private static IEnumerable<YearWeek> GetData()
    {
        yield return new(2022, 16);
        yield return new(2021, 27);
        yield return new(2022, 27);
    }

    [TestCaseSource(nameof(GetData))]
    public void LessThan_YearWeek_Compared(YearWeek right)
        => Assert.That(new YearWeek(2021, 17) < right, Is.True);

    [TestCaseSource(nameof(GetData))]
    public void LessThanOrEqual_YearWeek_Compared(YearWeek right)
        => Assert.That(new YearWeek(2021, 17) <= right, Is.True);

    [Test]
    public void LessThanOrEqual_YearWeek_Compared()
        => Assert.That(new YearWeek(2021, 17) <= new YearWeek(2021, 17), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThan_YearWeek_Compared(YearWeek right)
        => Assert.That(new YearWeek(2021, 1) > right, Is.False);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThanOrEqual_YearWeek_Compared(YearWeek right)
        => Assert.That(new YearWeek(2021, 1) >= right, Is.False);

    [Test]
    public void GreaterThanOrEqual_YearWeek_Compared()
        => Assert.That(new YearWeek(2021, 1) >= new YearWeek(2021, 1), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void NotEqual_YearWeek_Compared(YearWeek right)
        => Assert.That(new YearWeek(2021, 1) != right, Is.True);

    [Test]
    public void Equal_YearWeek_Compared()
        => Assert.That(new YearWeek(2021, 1) == new YearWeek(2021, 1), Is.True);

    [Test]
    public void CompareTo_Null_Compared()
        => Assert.That(new YearWeek(2021, 1).CompareTo(null), Is.EqualTo(1));

    [Test]
    public void CompareTo_ItSelf_Compared()
        => Assert.That(new YearWeek(2021, 1).CompareTo(new YearWeek(2021, 1)), Is.EqualTo(0));

    [Test]
    public void CompareTo_SomethingElse_Compared()
        => Assert.That(new YearWeek(2021, 1).CompareTo(new YearWeek(2022, 3)), Is.EqualTo(-1));

    [Test]
    [TestCase("2025-W01", true)]
    [TestCase("2025-Q5", false)]
    public void TryParse_SomeValue_Expected(string input, bool expected)
    {
        var result = YearWeek.TryParse(input, null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new YearWeek(2025, 1)));
        });
    }

    [Test]
    [TestCase("W1-25", true)]
    [TestCase("12-X", false)]
    public void TryParse_SomeValueWithFormat_Expected(string input, bool expected)
    {
        var result = YearWeek.TryParse(input, "'W'w-yy", null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new YearWeek(2025, 1)));
        });
    }

    [Test]
    [TestCase("2025-W01")]
    public void Parse_SomeValueAsSpan_Expected(string input)
    {
        var value = YearWeek.Parse(input.AsSpan(), null);
        Assert.That(value, Is.EqualTo(new YearWeek(2025, 1)));
    }

    [Test]
    [TestCase("W01-25")]
    public void Parse_SomeValueFormatAsSpan_Expected(string input)
    {
        var value = YearWeek.Parse(input.AsSpan(), "'W'ww-yy", null);
        Assert.That(value, Is.EqualTo(new YearWeek(2025, 1)));
    }

    [Test]
    [TestCase("2025-W01", true)]
    [TestCase("2025-Q5", false)]
    public void TryParse_SomeValueAsSpan_Expected(string input, bool expected)
    {
        var result = YearWeek.TryParse(input.AsSpan(), null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new YearWeek(2025, 1)));
        });
    }

    [Test]
    [TestCase("W1-25", true)]
    [TestCase("12-X", false)]
    public void TryParse_SomeValueWithFormatAsSpan_Expected(string input, bool expected)
    {
        var result = YearWeek.TryParse(input.AsSpan(), "'W'w-yy", null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new YearWeek(2025, 1)));
        });
    }

    [Test]
    [TestCase("2024-W52")]
    [TestCase("2025-W01")]
    [TestCase("2025-W52")]
    [TestCase("2026-W53")]
    [TestCase("2027-W01")]
    public void Days_SomeValue_Expected(string input)
    {
        var value = YearWeek.Parse(input, null);
        Assert.That(value.Days, Is.EqualTo(7));
    }

    [Test]
    [TestCase("2025-W01", "2024-12-30")]
    [TestCase("2026-W01", "2025-12-29")]
    [TestCase("2027-W01", "2027-01-04")]
    public void FirstDate_SomeValue_Expected(string input, DateOnly expected)
    {
        var value = YearWeek.Parse(input, null);
        Assert.That(value.FirstDate, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("2024-W52", "2024-12-29")]
    [TestCase("2025-W01", "2025-01-05")]
    [TestCase("2025-W52", "2025-12-28")]
    [TestCase("2026-W53", "2027-01-03")]
    [TestCase("2027-W01", "2027-01-10")]
    public void LastDate_SomeValue_Expected(string input, DateOnly expected)
    {
        var value = YearWeek.Parse(input, null);
        Assert.That(value.LastDate, Is.EqualTo(expected));
    }

    [TestCase("2024-W52", "2024-12-23")]
    public void LowerBound_SomeValue_Expected(string input, DateTime expected)
    {
        var value = YearWeek.Parse(input, null);
        Assert.That(value.LowerBound, Is.EqualTo(expected));
    }

    [TestCase("2024-W52", "2024-12-30")]
    public void UpperBound_SomeValue_Expected(string input, DateTime expected)
    {
        var value = YearWeek.Parse(input, null);
        Assert.That(value.UpperBound, Is.EqualTo(expected));
    }
}
