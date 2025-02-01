using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chrononuensis.Testing;
public class YearWeekTests
{
    [Test]
    public void Parse_InputDefaultFormat_Equal()
        => Assert.That(YearWeek.Parse("2021-W16", "yyyy-'W'ww"), Is.EqualTo(new YearWeek(2021,16)));

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
}
