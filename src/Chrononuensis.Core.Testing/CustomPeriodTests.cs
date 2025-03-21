﻿using System;
using System.Collections;
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
    {
        var period = new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 10));
        Assert.Multiple(() =>
        {
            Assert.That(period.FirstDate, Is.EqualTo(new DateOnly(2025, 1, 1)));
            Assert.That(period.LastDate, Is.EqualTo(new DateOnly(2025, 1, 10)));
        });
    }

    [Test]
    public void Ctor_InvalidValues_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new CustomPeriod(new DateOnly(2025, 1, 10), new DateOnly(2025, 1, 1)));
        Assert.Multiple(() =>
        {
            Assert.That(ex.Message, Does.Contain("Invalid period"));
            Assert.That(ex.Message, Does.Contain("Start date"));
            Assert.That(ex.Message, Does.Contain("must be on or before end date"));
            Assert.That(ex.Message, Does.Contain("(Parameter 'firstDate')"));
        });
    }

    [TestCase("2025-01-01", "2025-01-01", 1, Description = "Single day")]
    [TestCase("2025-01-31", "2025-02-01", 2, Description = "Month boundary")]
    [TestCase("2024-02-28", "2024-03-01", 3, Description = "Month boundary in leap year")]
    public void Days_ValidValues_Expected(string start, string end, int expectedDays)
    {
        var period = new CustomPeriod(DateOnly.Parse(start), DateOnly.Parse(end));
        Assert.That(period.Days, Is.EqualTo(expectedDays));
    }

    [Test]
    public void FirstDate_ValidValues_Expected()
        => Assert.That(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 10)).FirstDate, Is.EqualTo(new DateOnly(2025, 1, 1)));

    [Test]
    public void LastDate_ValidValues_Expected()
        => Assert.That(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 10)).LastDate, Is.EqualTo(new DateOnly(2025, 1, 10)));

    [Test]
    public void LowerBound_ValidValues_Expected()
        => Assert.That(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 10)).LowerBound, Is.EqualTo(new DateTime(2025, 1, 1)));

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
            == new YearMonth(2024, 1), Is.True);

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

    [Test]
    public void Equal_Null_False()
    {
        var period = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31));
        Assert.That(period.Equals(null), Is.False);
    }

    [Test]
    public void Equal_DifferentType_False()
    {
        var period = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31));
        Assert.That(period.Equals("not a period"), Is.False);
    }

    [Test]
    public void Contains_Distinct_Expected()
    {
        var left = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 20));
        var right = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 10));
        Assert.That(left.Contains(right), Is.True);
    }

    [Test]
    public void Overlaps_Distinct_Expected()
    {
        var left = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 20));
        var right = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 2, 10));
        Assert.That(left.Overlaps(right), Is.True);
    }

    [Test]
    public void Meets_Distinct_Expected()
    {
        var left = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 20));
        var right = new CustomPeriod(new DateOnly(2024, 1, 21), new DateOnly(2024, 2, 10));
        Assert.That(left.Meets(right), Is.True);
    }

    [Test]
    public void Succeeds_Distinct_Expected()
    {
        var left = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 20));
        var right = new CustomPeriod(new DateOnly(2024, 1, 25), new DateOnly(2024, 2, 10));
        Assert.That(left.Succeeds(right), Is.False);
    }

    [Test]
    public void Precedes_Distinct_Expected()
    {
        var left = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 20));
        var right = new CustomPeriod(new DateOnly(2024, 1, 25), new DateOnly(2024, 2, 10));
        Assert.That(left.Precedes(right), Is.True);
    }

    [Test]
    public void Intersect_Distinct_Expected()
    {
        var left = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 20));
        var right = new CustomPeriod(new DateOnly(2024, 1, 15), new DateOnly(2024, 2, 10));
        Assert.That(left.Intersect(right), Is.EqualTo(new CustomPeriod(new DateOnly(2024, 1, 15), new DateOnly(2024, 1, 20))));
    }

    [Test]
    public void Span_Distinct_Expected()
    {
        var left = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 20));
        var right = new CustomPeriod(new DateOnly(2024, 1, 25), new DateOnly(2024, 2, 10));
        Assert.That(left.Span(right), Is.EqualTo(new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 2, 10))));
    }

    [Test]
    public void Gap_Distinct_Expected()
    {
        var left = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 20));
        var right = new CustomPeriod(new DateOnly(2024, 1, 25), new DateOnly(2024, 2, 10));
        Assert.That(left.Gap(right), Is.EqualTo(4));
    }
}
