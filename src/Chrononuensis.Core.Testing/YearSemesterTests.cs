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
}
