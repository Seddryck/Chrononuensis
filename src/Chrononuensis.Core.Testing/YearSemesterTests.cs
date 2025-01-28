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
}
