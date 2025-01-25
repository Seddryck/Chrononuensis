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
        => Assert.That(YearMonth.Parse("2021-01", "yyyy-MM"), Is.EqualTo(new YearMonth(2021,1)));

    [Test]
    public void Equals_Distinct_False()
        => Assert.That(new YearMonth(2021, 1) == new YearMonth(2021, 2), Is.False);

    [Test]
    public void Equals_Same_True()
        => Assert.That(new YearMonth(2021, 1) == new YearMonth(2021, 1), Is.True);

    [Test]
    public void Compare_Distinct_Compared()
        => Assert.That(new YearMonth(2021, 1) < new YearMonth(2021, 2), Is.True);
}
