using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chrononuensis.Testing;
public class CustomPeriodTests
{
    [Test]
    public void Ctor_ValidValues_Expected()
        => Assert.DoesNotThrow(() => new CustomPeriod(new DateOnly(2025,1,1), new DateOnly(2025, 1, 10)));

    [Test]
    public void Ctor_InvalidValues_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() => new CustomPeriod(new DateOnly(2025, 1, 10), new DateOnly(2025, 1, 1)));
    }

    [Test]
    public void Days_ValidValues_Expected()
        => Assert.That(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 10)).Days, Is.EqualTo(10));

    [Test]
    public void FirsDate_ValidValues_Expected()
        => Assert.That(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 10)).FirstDate, Is.EqualTo(new DateOnly(2025, 1, 1)));

    [Test]
    public void LastDate_ValidValues_Expected()
        => Assert.That(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 10)).LastDate, Is.EqualTo(new DateOnly(2025, 1, 10)));

    [Test]
    public void LowerBound_ValidValues_Expected()
        => Assert.That(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 10)).LowerBound, Is.EqualTo(new DateTime(2025,1,1)));

    [Test]
    public void UpperBound_ValidValues_Expected()
        => Assert.That(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 10)).UpperBound, Is.EqualTo(new DateTime(2025, 1, 11)));


    [Test]
    public void LessThanOrEqual_CustomPeriod_True()
        => Assert.That(new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 10))
            <= new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 10)), Is.True);

    [Test]
    public void LessThan_CustomPeriod_False()
        => Assert.That(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 10))
            < new CustomPeriod(new DateOnly(2025, 1, 10), new DateOnly(2025, 1, 20)), Is.False);

    [Test]
    public void LessThanOrEqual_CustomPeriod_False()
        => Assert.That(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 10))
            <= new CustomPeriod(new DateOnly(2025, 1, 10), new DateOnly(2025, 1, 20)), Is.True);

    [Test]
    public void Equal_CustomPeriod_True()
        => Assert.That(new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31))
            == new YearMonth(2024,1), Is.True);

    [Test]
    public void Equal_CustomPeriod_False()
        => Assert.That(new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31))
            == new YearMonth(2025, 1), Is.False);

    [Test]
    public void NotEqual_CustomPeriod_False()
        => Assert.That(new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31))
            != new YearMonth(2024, 1), Is.False);

    [Test]
    public void NotEqual_CustomPeriod_True()
        => Assert.That(new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31))
            != new YearMonth(2025, 1), Is.True);

    [Test]
    public void Equal_Identical_True()
        => Assert.That(new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31)).Equals(
            new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31))), Is.True);

    [Test]
    public void Equal_SameSpan_True()
        => Assert.That(new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31)).Equals(
            new YearMonth(2024, 1)), Is.True);

    [Test]
    public void Equal_SameRef_True()
    {
        var period = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31));
        Assert.That(period.Equals(period), Is.True);
    }
}
