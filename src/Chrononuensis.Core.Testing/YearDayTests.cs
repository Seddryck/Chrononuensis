using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chrononuensis.Testing;
public class YearDayTests
{
    [TestCase(2004, 10)]
    [TestCase(2004, 365)]
    [TestCase(2004, 366)]
    [TestCase(2005, 365)]
    public void Ctor_ValidValues_Expected(int year, int day)
        => Assert.DoesNotThrow(() => new YearDay(year, day));

    [TestCase(2004, 367, 366)]
    [TestCase(2005, 366, 365)]
    public void Ctor_InvalidValues_Throws(int year, int day, int max)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new YearDay(year, day));
        Assert.That(ex.ParamName, Is.EqualTo("DayOfYear"));
        Assert.That(ex.Message, Does.StartWith($"When year is {year}"));
        Assert.That(ex.Message, Does.Contain($"1 and {max}"));
    }

    [Test]
    public void Parse_InputDefaultFormat_Equal()
        => Assert.That(YearDay.Parse("2021-116", "yyyy-jjj"), Is.EqualTo(new YearDay(2021, 116)));

    private static IEnumerable<YearDay> GetData()
    {
        yield return new(2022, 116);
        yield return new(2021, 217);
        yield return new(2022, 217);
    }

    [TestCaseSource(nameof(GetData))]
    public void LessThan_YearDay_Compared(YearDay right)
        => Assert.That(new YearDay(2021, 17) < right, Is.True);

    [TestCaseSource(nameof(GetData))]
    public void LessThanOrEqual_YearDay_Compared(YearDay right)
        => Assert.That(new YearDay(2021, 17) <= right, Is.True);

    [Test]
    public void LessThanOrEqual_YearDay_Compared()
        => Assert.That(new YearDay(2021, 117) <= new YearDay(2021, 117), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThan_YearDay_Compared(YearDay right)
        => Assert.That(new YearDay(2021, 1) > right, Is.False);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThanOrEqual_YearDay_Compared(YearDay right)
        => Assert.That(new YearDay(2021, 1) >= right, Is.False);

    [Test]
    public void GreaterThanOrEqual_YearDay_Compared()
        => Assert.That(new YearDay(2021, 1) >= new YearDay(2021, 1), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void NotEqual_YearDay_Compared(YearDay right)
        => Assert.That(new YearDay(2021, 1) != right, Is.True);

    [Test]
    public void Equal_YearDay_Compared()
        => Assert.That(new YearDay(2021, 1) == new YearDay(2021, 1), Is.True);

    [Test]
    public void CompareTo_Null_Compared()
        => Assert.That(new YearDay(2021, 1).CompareTo(null), Is.EqualTo(1));

    [Test]
    public void CompareTo_ItSelf_Compared()
        => Assert.That(new YearDay(2021, 1).CompareTo(new YearDay(2021, 1)), Is.EqualTo(0));

    [Test]
    public void CompareTo_SomethingElse_Compared()
        => Assert.That(new YearDay(2021, 1).CompareTo(new YearDay(2022, 3)), Is.EqualTo(-1));

    [Test]
    [TestCase("2025-001", true)]
    [TestCase("2025-999", false)]
    public void TryParse_SomeValue_Expected(string input, bool expected)
    {
        var result = YearDay.TryParse(input, null, out var yearDay);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(yearDay, Is.EqualTo(new YearDay(2025, 1)));
        });
    }

    [Test]
    [TestCase("2025-1", true)]
    [TestCase("2025-X", false)]
    public void TryParse_SomeValueWithFormat_Expected(string input, bool expected)
    {
        var result = YearDay.TryParse(input, "yyyy-j", null, out var yearDay);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(yearDay, Is.EqualTo(new YearDay(2025, 1)));
        });
    }

    [Test]
    [TestCase("2025-001")]
    public void Parse_SomeValueAsSpan_Expected(string input)
    {
        var value = YearDay.Parse(input.AsSpan(), null);
        Assert.That(value, Is.EqualTo(new YearDay(2025, 1)));
    }

    [Test]
    [TestCase("001-25")]
    public void Parse_SomeValueFormatAsSpan_Expected(string input)
    {
        var value = YearDay.Parse(input.AsSpan(), "jjj-yy", null);
        Assert.That(value, Is.EqualTo(new YearDay(2025, 1)));
    }

    [Test]
    [TestCase("2025-001", true)]
    [TestCase("2025-Q5", false)]
    public void TryParse_SomeValueAsSpan_Expected(string input, bool expected)
    {
        var result = YearDay.TryParse(input.AsSpan(), null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new YearDay(2025, 1)));
        });
    }

    [Test]
    [TestCase("001-25", true)]
    [TestCase("12-X", false)]
    public void TryParse_SomeValueWithFormatAsSpan_Expected(string input, bool expected)
    {
        var result = YearDay.TryParse(input.AsSpan(), "jjj-yy", null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new YearDay(2025, 1)));
        });
    }

    [Test]
    [TestCase("2024-152", 1)]
    public void Days_SomeValue_Expected(string input, int expected)
    {
        var value = YearDay.Parse(input, null);
        Assert.That(value.Days, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("2024-033", "2024-02-02")]
    public void FirstDate_SomeValue_Expected(string input, DateOnly expected)
    {
        var value = YearDay.Parse(input, null);
        Assert.That(value.FirstDate, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("2024-033", "2024-02-02")]
    public void LastDate_SomeValue_Expected(string input, DateOnly expected)
    {
        var value = YearDay.Parse(input, null);
        Assert.That(value.LastDate, Is.EqualTo(expected));
    }

    [TestCase("2024-033", "2024-02-02")]
    public void LowerBound_SomeValue_Expected(string input, DateTime expected)
    {
        var value = YearDay.Parse(input, null);
        Assert.That(value.LowerBound, Is.EqualTo(expected));
    }

    [TestCase("2024-033", "2024-02-03")]
    public void UpperBound_SomeValue_Expected(string input, DateTime expected)
    {
        var value = YearDay.Parse(input, null);
        Assert.That(value.UpperBound, Is.EqualTo(expected));
    }
}
