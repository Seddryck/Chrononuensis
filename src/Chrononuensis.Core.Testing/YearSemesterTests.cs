using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chrononuensis.Testing;
public class YearSemesterTests
{
    [Test]
    public void Parse_InputDefaultFormat_Equal()
        => Assert.That(YearSemester.Parse("2021-S1", "yyyy-'S'S"), Is.EqualTo(new YearSemester(2021,1)));

    private static IEnumerable<YearSemester> GetData()
    {
        yield return new(2022, 1);
        yield return new(2021, 2);
        yield return new(2022, 2);
    }
        
    [TestCaseSource(nameof(GetData))]
    public void LessThan_YearSemester_Compared(YearSemester right)
        => Assert.That(new YearSemester(2021, 1) < right, Is.True);

    [TestCaseSource(nameof(GetData))]
    public void LessThanOrEqual_YearSemester_Compared(YearSemester right)
        => Assert.That(new YearSemester(2021, 1) <= right, Is.True);

    [Test]
    public void LessThanOrEqual_YearSemester_Compared()
        => Assert.That(new YearSemester(2021, 1) <= new YearSemester(2021, 1), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThan_YearSemester_Compared(YearSemester right)
        => Assert.That(new YearSemester(2021, 1) > right, Is.False);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThanOrEqual_YearSemester_Compared(YearSemester right)
        => Assert.That(new YearSemester(2021, 1) >= right, Is.False);

    [Test]
    public void GreaterThanOrEqual_YearSemester_Compared()
        => Assert.That(new YearSemester(2021, 1) >= new YearSemester(2021, 1), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void NotEqual_YearSemester_Compared(YearSemester right)
        => Assert.That(new YearSemester(2021, 1) != right, Is.True);

    [Test]
    public void Equal_YearSemester_Compared()
        => Assert.That(new YearSemester(2021, 1) == new YearSemester(2021, 1), Is.True);
    [Test]
    public void CompareTo_Null_Compared()
        => Assert.That(new YearSemester(2021, 1).CompareTo(null), Is.EqualTo(1));

    [Test]
    public void CompareTo_ItSelf_Compared()
        => Assert.That(new YearSemester(2021, 1).CompareTo(new YearSemester(2021, 1)), Is.EqualTo(0));

    [Test]
    public void CompareTo_SomethingElse_Compared()
        => Assert.That(new YearSemester(2021, 1).CompareTo(new YearSemester(2022, 2)), Is.EqualTo(-1));

    [Test]
    [TestCase("2025-H1", true)]
    [TestCase("2025-Q5", false)]
    public void TryParse_SomeValue_Expected(string input, bool expected)
    {
        var result = YearSemester.TryParse(input, null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new YearSemester(2025, 1)));
        });
    }

    [Test]
    [TestCase("H1-25", true)]
    [TestCase("12-X", false)]
    public void TryParse_SomeValueWithFormat_Expected(string input, bool expected)
    {
        var result = YearSemester.TryParse(input, "'H'S-yy", null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new YearSemester(2025, 1)));
        });
    }

    [Test]
    [TestCase("2025-H1")]
    public void Parse_SomeValueAsSpan_Expected(string input)
    {
        var value = YearSemester.Parse(input.AsSpan(), null);
        Assert.That(value, Is.EqualTo(new YearSemester(2025, 1)));
    }

    [Test]
    [TestCase("H1-25")]
    public void Parse_SomeValueFormatAsSpan_Expected(string input)
    {
        var value = YearSemester.Parse(input.AsSpan(), "'H'S-yy", null);
        Assert.That(value, Is.EqualTo(new YearSemester(2025, 1)));
    }

    [Test]
    [TestCase("2025-H1", true)]
    [TestCase("2025-Q5", false)]
    public void TryParse_SomeValueAsSpan_Expected(string input, bool expected)
    {
        var result = YearSemester.TryParse(input.AsSpan(), null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new YearSemester(2025, 1)));
        });
    }

    [Test]
    [TestCase("H1-25", true)]
    [TestCase("12-X", false)]
    public void TryParse_SomeValueWithFormatAsSpan_Expected(string input, bool expected)
    {
        var result = YearSemester.TryParse(input.AsSpan(), "'H'S-yy", null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new YearSemester(2025, 1)));
        });
    }

    [Test]
    [TestCase("2000-H1", 182)]  // Leap year
    [TestCase("1900-H1", 181)]  // Non-leap year
    [TestCase("2024-H1", 182)]  // Leap year
    [TestCase("2025-H1", 181)]  // Non-leap year
    [TestCase("2024-H2", 184)]  // Leap year (Jul-Dec: 31+31+30+31+30+31)
    [TestCase("2025-H2", 184)]  // Non-leap year (Jul-Dec: 31+31+30+31+30+31)
    public void Days_SomeValue_Expected(string input, int expected)
    {
        var value = YearSemester.Parse(input, null);
        Assert.That(value.Days, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("2024-H1", "2024-01-01")]
    public void FirstDate_SomeValue_Expected(string input, DateOnly expected)
    {
        var value = YearSemester.Parse(input, null);
        Assert.That(value.FirstDate, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("2024-H1", "2024-06-30")]
    public void LastDate_SomeValue_Expected(string input, DateOnly expected)
    {
        var value = YearSemester.Parse(input, null);
        Assert.That(value.LastDate, Is.EqualTo(expected));
    }

    [TestCase("2024-H1", "2024-01-01")]
    public void LowerBound_SomeValue_Expected(string input, DateTime expected)
    {
        var value = YearSemester.Parse(input, null);
        Assert.That(value.LowerBound, Is.EqualTo(expected));
    }

    [TestCase("2024-H1", "2024-07-01")]
    public void UpperBound_SomeValue_Expected(string input, DateTime expected)
    {
        var value = YearSemester.Parse(input, null);
        Assert.That(value.UpperBound, Is.EqualTo(expected));
    }
}
