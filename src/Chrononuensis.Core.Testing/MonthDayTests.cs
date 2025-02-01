using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chrononuensis.Testing;
public class MonthDayTests
{
    [Test]
    public void Parse_InputDefaultFormat_Equal()
        => Assert.That(MonthDay.Parse("Jan-01", "MMM-dd"), Is.EqualTo(new MonthDay(1, 1)));

    private static IEnumerable<MonthDay> GetData()
    {
        yield return new(1, 6);
        yield return new(2, 25);
        yield return new(1, 25);
    }

    [TestCaseSource(nameof(GetData))]
    public void LessThan_MonthDay_Compared(MonthDay right)
        => Assert.That(new MonthDay(1, 5) < right, Is.True);

    [TestCaseSource(nameof(GetData))]
    public void LessThanOrEqual_MonthDay_Compared(MonthDay right)
        => Assert.That(new MonthDay(1, 5) <= right, Is.True);

    [Test]
    public void LessThanOrEqual_MonthDay_Compared()
        => Assert.That(new MonthDay(1, 6) <= new MonthDay(1, 6), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThan_MonthDay_Compared(MonthDay right)
        => Assert.That(new MonthDay(3, 1) > right, Is.True);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThanOrEqual_MonthDay_Compared(MonthDay right)
        => Assert.That(new MonthDay(3, 1) >= right, Is.True);

    [Test]
    public void GreaterThanOrEqual_MonthDay_Compared()
        => Assert.That(new MonthDay(1, 6) >= new MonthDay(1, 6), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void NotEqual_MonthDay_Compared(MonthDay right)
        => Assert.That(new MonthDay(3, 1) != right, Is.True);

    [Test]
    public void Equal_MonthDay_Compared()
        => Assert.That(new MonthDay(1, 6) == new MonthDay(1, 6), Is.True);

    [Test]
    public void CompareTo_Null_Compared()
        => Assert.That(new MonthDay(1, 6).CompareTo(null), Is.EqualTo(1));

    [Test]
    public void CompareTo_ItSelf_Compared()
        => Assert.That(new MonthDay(1, 6).CompareTo(new MonthDay(1, 6)), Is.EqualTo(0));

    [Test]
    public void CompareTo_SomethingElse_Compared()
        => Assert.That(new MonthDay(1, 6).CompareTo(new MonthDay(3, 1)), Is.EqualTo(-1));

    [Test]
    [TestCase("12-01", true)]
    [TestCase("12-999", false)]
    public void TryParse_SomeValue_Expected(string input, bool expected)
    {
        var result = MonthDay.TryParse(input, null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new MonthDay(12, 1)));
        });
    }

    [Test]
    [TestCase("12-1", true)]
    [TestCase("12-X", false)]
    public void TryParse_SomeValueWithFormat_Expected(string input, bool expected)
    {
        var result = MonthDay.TryParse(input, "MM-d", null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new MonthDay(12, 1)));
        });
    }

    [Test]
    [TestCase("12-01")]
    public void Parse_SomeValueAsSpan_Expected(string input)
    {
        var value = MonthDay.Parse(input.AsSpan(), null);
        Assert.That(value, Is.EqualTo(new MonthDay(12, 1)));
    }

    [Test]
    [TestCase("Dec-01")]
    public void Parse_SomeValueFormatAsSpan_Expected(string input)
    {
        var value = MonthDay.Parse(input.AsSpan(), "MMM-dd", null);
        Assert.That(value, Is.EqualTo(new MonthDay(12, 1)));
    }

    [Test]
    [TestCase("12-01", true)]
    [TestCase("2025-Q5", false)]
    public void TryParse_SomeValueAsSpan_Expected(string input, bool expected)
    {
        var result = MonthDay.TryParse(input.AsSpan(), null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new MonthDay(12, 1)));
        });
    }

    [Test]
    [TestCase("Dec-01", true)]
    [TestCase("12-X", false)]
    public void TryParse_SomeValueWithFormatAsSpan_Expected(string input, bool expected)
    {
        var result = MonthDay.TryParse(input.AsSpan(), "MMM-dd", null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new MonthDay(12, 1)));
        });
    }
}
