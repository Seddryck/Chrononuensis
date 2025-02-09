using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Chrononuensis.Extensions;

namespace Chrononuensis.Testing;
public class YearMonthTests
{
    [Test]
    public void Parse_InputDefaultFormat_Equal()
        => Assert.That(YearMonth.Parse("2021-01", "yyyy-MM"), Is.EqualTo(new YearMonth(2021, 1)));

    private static IEnumerable<YearMonth> GetData()
    {
        yield return new(2022, 1);
        yield return new(2021, 2);
        yield return new(2022, 2);
    }

    [TestCaseSource(nameof(GetData))]
    public void LessThan_YearMonth_Compared(YearMonth right)
        => Assert.That(new YearMonth(2021, 1) < right, Is.True);

    [TestCaseSource(nameof(GetData))]
    public void LessThanOrEqual_YearMonth_Compared(YearMonth right)
        => Assert.That(new YearMonth(2021, 1) <= right, Is.True);

    [Test]
    public void LessThanOrEqual_YearMonth_Compared()
        => Assert.That(new YearMonth(2021, 1) <= new YearMonth(2021, 1), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThan_YearMonth_Compared(YearMonth right)
        => Assert.That(new YearMonth(2021, 1) > right, Is.False);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThanOrEqual_YearMonth_Compared(YearMonth right)
        => Assert.That(new YearMonth(2021, 1) >= right, Is.False);

    [Test]
    public void GreaterThanOrEqual_YearMonth_Compared()
        => Assert.That(new YearMonth(2021, 1) >= new YearMonth(2021, 1), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void NotEqual_YearMonth_Compared(YearMonth right)
        => Assert.That(new YearMonth(2021, 1) != right, Is.True);

    [Test]
    public void Equal_YearMonth_Compared()
        => Assert.That(new YearMonth(2021, 1) == new YearMonth(2021, 1), Is.True);

    [Test]
    public void CompareTo_Null_Compared()
        => Assert.That(new YearMonth(2021, 1).CompareTo(null), Is.EqualTo(1));

    [Test]
    public void CompareTo_ItSelf_Compared()
        => Assert.That(new YearMonth(2021, 1).CompareTo(new YearMonth(2021, 1)), Is.EqualTo(0));

    [Test]
    public void CompareTo_SomethingElse_Compared()
        => Assert.That(new YearMonth(2021, 1).CompareTo(new YearMonth(2022, 3)), Is.EqualTo(-1));

    [Test]
    [TestCase("2025-01", true)]
    [TestCase("2025-99", false)]
    public void TryParse_SomeValue_Expected(string input, bool expected)
    {
        var result = YearMonth.TryParse(input, null, out var yearMonth);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(yearMonth, Is.EqualTo(new YearMonth(2025, 1)));
        });
    }

    [Test]
    [TestCase("2025-Jan", true)]
    [TestCase("2025-Xyz", false)]
    public void TryParse_SomeValueWithFormat_Expected(string input, bool expected)
    {
        var result = YearMonth.TryParse(input, "yyyy-MMM", null, out var yearMonth);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(yearMonth, Is.EqualTo(new YearMonth(2025, 1)));
        });
    }

    [Test]
    [TestCase("Jan-25")]
    public void Parse_SomeValueFormatAsSpan_Expected(string input)
    {
        var value = YearMonth.Parse(input.AsSpan(), "MMM-yy", null);
        Assert.That(value, Is.EqualTo(new YearMonth(2025, 1)));
    }

    [Test]
    [TestCase("2025-01", true)]
    [TestCase("2025-Q5", false)]
    public void TryParse_SomeValueAsSpan_Expected(string input, bool expected)
    {
        var result = YearMonth.TryParse(input.AsSpan(), null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new YearMonth(2025, 1)));
        });
    }

    [Test]
    [TestCase("Jan-25", true)]
    [TestCase("12-X", false)]
    public void TryParse_SomeValueWithFormatAsSpan_Expected(string input, bool expected)
    {
        var result = YearMonth.TryParse(input.AsSpan(), "MMM-yy", null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new YearMonth(2025, 1)));
        });
    }

    [Test]
    public void Parse_InvalidFormat_ValuesNotSpecified()
    {
        var ex = Assert.Throws<FormatException>(() => YearMonth.Parse("Q1.25", "Qq.yy", CultureInfo.InvariantCulture.DateTimeFormat));
        Assert.That(ex.Message, Is.EqualTo("Token 'month' not found in the format"));
    }

    [TestCase("2025-01", 1, "2026-01")]
    [TestCase("2025-01", 5, "2030-01")]
    public void AddYear_SomeValue_Expected(string input, int value, string expected)
    {
        var result = YearMonth.Parse(input, null).AddYear(value);
        Assert.That(result, Is.EqualTo(YearMonth.Parse(expected, null)));
    }

    [TestCase("2025-01", 5, "2025-06")]
    [TestCase("2025-12", 4, "2026-04")]
    [TestCase("2025-12", 14, "2027-02")]
    public void AddMonth_SomeValue_Expected(string input, int value, string expected)
    {
        var result = YearMonth.Parse(input, null).AddMonth(value);
        Assert.That(result, Is.EqualTo(YearMonth.Parse(expected, null)));
    }

    [Test]
    [TestCase("25-02", "yy-MM")]
    [TestCase("2025-II", "yyyy'-'{MM:RN}")]
    [TestCase("MMXXV-Feb", "{yyyy:RN}-{MMM}")]
    public void Parse_SomeValueFormatAsSpan_Expected(string input, string format)
    {
        var value = YearMonth.Parse(input.AsSpan(), format, null);
        Assert.That(value, Is.EqualTo(new YearMonth(2025, 2)));
    }
}
