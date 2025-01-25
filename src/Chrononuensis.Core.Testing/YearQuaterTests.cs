using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chrononuensis.Testing;
public class YearQuarterTests
{
    [Test]
    public void Parse_InputDefaultFormat_Equal()
        => Assert.That(YearQuarter.Parse("2021-Q1", "yyyy-Qq"), Is.EqualTo(new YearQuarter(2021,1)));

    private static IEnumerable<YearQuarter> GetData()
    {
        yield return new(2022, 1);
        yield return new(2021, 2);
        yield return new(2022, 2);
    }
        
    [TestCaseSource(nameof(GetData))]
    public void LessThan_YearQuarter_Compared(YearQuarter right)
        => Assert.That(new YearQuarter(2021, 1) < right, Is.True);

    [TestCaseSource(nameof(GetData))]
    public void LessThanOrEqual_YearQuarter_Compared(YearQuarter right)
        => Assert.That(new YearQuarter(2021, 1) <= right, Is.True);

    [Test]
    public void LessThanOrEqual_YearQuarter_Compared()
        => Assert.That(new YearQuarter(2021, 1) <= new YearQuarter(2021, 1), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThan_YearQuarter_Compared(YearQuarter right)
        => Assert.That(new YearQuarter(2021, 1) > right, Is.False);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThanOrEqual_YearQuarter_Compared(YearQuarter right)
        => Assert.That(new YearQuarter(2021, 1) >= right, Is.False);

    [Test]
    public void GreaterThanOrEqual_YearQuarter_Compared()
        => Assert.That(new YearQuarter(2021, 1) >= new YearQuarter(2021, 1), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void NotEqual_YearQuarter_Compared(YearQuarter right)
        => Assert.That(new YearQuarter(2021, 1) != right, Is.True);

    [Test]
    public void Equal_YearQuarter_Compared()
        => Assert.That(new YearQuarter(2021, 1) == new YearQuarter(2021, 1), Is.True);
    [Test]
    public void CompareTo_Null_Compared()
        => Assert.That(new YearQuarter(2021, 1).CompareTo(null), Is.EqualTo(1));

    [Test]
    public void CompareTo_ItSelf_Compared()
        => Assert.That(new YearQuarter(2021, 1).CompareTo(new YearQuarter(2021, 1)), Is.EqualTo(0));

    [Test]
    public void CompareTo_SomethingElse_Compared()
        => Assert.That(new YearQuarter(2021, 1).CompareTo(new YearQuarter(2022, 3)), Is.EqualTo(-1));
}
