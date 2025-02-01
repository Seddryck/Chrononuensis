using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

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
}
