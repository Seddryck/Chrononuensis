using NUnit.Framework;

namespace Chrononuensis.Testing;

public class CustomPeriodTests
{
    [Test]
    public void Ctor_HalfOpenBounds_StoresCanonicalValues()
    {
        var period = new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 11));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(period.StartDate, Is.EqualTo(new DateOnly(2025, 1, 1)));
            Assert.That(period.EndDateExclusive, Is.EqualTo(new DateOnly(2025, 1, 11)));
            Assert.That(period.FirstDate, Is.EqualTo(period.StartDate));
            Assert.That(period.LastDate, Is.EqualTo(new DateOnly(2025, 1, 10)));
            Assert.That(period.LowerBound, Is.EqualTo(new DateTime(2025, 1, 1)));
            Assert.That(period.UpperBound, Is.EqualTo(new DateTime(2025, 1, 11)));
        }
    }

    [Test]
    public void Ctor_EmptyPeriod_Throws()
        => Assert.That(
            (Action)(() => { _ = new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 1)); }),
            Throws.TypeOf<ArgumentOutOfRangeException>()
                .With.Property("ParamName").EqualTo("endDateExclusive"));

    [Test]
    public void Ctor_ReversedBounds_Throws()
        => Assert.That(
            (Action)(() => { _ = new CustomPeriod(new DateOnly(2025, 1, 2), new DateOnly(2025, 1, 1)); }),
            Throws.TypeOf<ArgumentOutOfRangeException>()
                .With.Message.Contains("exclusive end date")
                .And.Property("ParamName").EqualTo("endDateExclusive"));

    [TestCase("2025-01-01", "2025-01-02", 1, Description = "Single day")]
    [TestCase("2025-01-31", "2025-02-02", 2, Description = "Month boundary")]
    [TestCase("2024-02-28", "2024-03-02", 3, Description = "Leap day")]
    public void Days_HalfOpenBounds_Expected(string start, string endExclusive, int expectedDays)
    {
        var period = new CustomPeriod(DateOnly.Parse(start), DateOnly.Parse(endExclusive));
        Assert.That(period.Days, Is.EqualTo(expectedDays));
    }

    [Test]
    public void FromInclusiveDates_ConvertsToExclusiveEnd()
    {
        var period = CustomPeriod.FromInclusiveDates(new DateOnly(2024, 2, 28), new DateOnly(2024, 2, 29));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(period.StartDate, Is.EqualTo(new DateOnly(2024, 2, 28)));
            Assert.That(period.EndDateExclusive, Is.EqualTo(new DateOnly(2024, 3, 1)));
        }
    }

    [Test]
    public void FromInclusiveDates_MaxValue_ThrowsWithoutOverflow()
        => Assert.That(
            (Action)(() => { _ = CustomPeriod.FromInclusiveDates(DateOnly.MaxValue.AddDays(-1), DateOnly.MaxValue); }),
            Throws.TypeOf<ArgumentOutOfRangeException>()
                .With.Property("ParamName").EqualTo("lastDate"));

    [Test]
    public void Ctor_ExclusiveEndAtMaxValue_IsSupported()
    {
        var period = new CustomPeriod(DateOnly.MaxValue.AddDays(-1), DateOnly.MaxValue);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(period.Days, Is.EqualTo(1));
            Assert.That(period.LastDate, Is.EqualTo(DateOnly.MaxValue.AddDays(-1)));
            Assert.That(period.UpperBound, Is.EqualTo(DateOnly.MaxValue.ToDateTime(TimeOnly.MinValue)));
        }
    }

    [Test]
    public void LessThanOrEqual_EarlierPeriod_True()
        => Assert.That(
            new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11))
            <= new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 11)),
            Is.True);

    [Test]
    public void LessThan_OverlappingPeriod_False()
        => Assert.That(
            new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 11))
            < new CustomPeriod(new DateOnly(2025, 1, 10), new DateOnly(2025, 1, 21)),
            Is.False);

    [Test]
    public void LessThanOrEqual_LaterEnd_True()
        => Assert.That(
            new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 11))
            <= new CustomPeriod(new DateOnly(2025, 1, 10), new DateOnly(2025, 1, 21)),
            Is.True);

    [Test]
    public void Equal_IdenticalCustomPeriods_True()
        => Assert.That(
            new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 2, 1)).Equals(
                new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 2, 1))),
            Is.True);

    [Test]
    public void Equal_SameYearBounds_IsSymmetricAndHashCompatible()
    {
        IPeriod custom = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2025, 1, 1));
        IPeriod year = new Year(2024);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(custom.Equals(year), Is.True);
            Assert.That(year.Equals(custom), Is.True);
            Assert.That(custom.GetHashCode(), Is.EqualTo(year.GetHashCode()));
        }
    }

    [Test]
    public void Equal_SameDayBounds_IsSymmetricAndHashCompatible()
    {
        IPeriod custom = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 2));
        IPeriod day = new YearDay(2024, 1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(custom.Equals(day), Is.True);
            Assert.That(day.Equals(custom), Is.True);
            Assert.That(custom.GetHashCode(), Is.EqualTo(day.GetHashCode()));
        }
    }

    [Test]
    public void Equal_SameNamedPeriodBounds_IsSymmetricAndHashCompatible()
    {
        object custom = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 2, 1));
        object month = new YearMonth(2024, 1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(custom.Equals(month), Is.True);
            Assert.That(month.Equals(custom), Is.True);
            Assert.That(custom.GetHashCode(), Is.EqualTo(month.GetHashCode()));
        }
    }

    [Test]
    public void Equal_SameBounds_IsTransitive()
    {
        IPeriod first = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 2, 1));
        IPeriod second = new YearMonth(2024, 1);
        IPeriod third = CustomPeriod.FromInclusiveDates(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first.Equals(second), Is.True);
            Assert.That(second.Equals(third), Is.True);
            Assert.That(first.Equals(third), Is.True);
        }
    }

    [Test]
    public void Equal_DifferentBounds_IsSymmetric()
    {
        IPeriod custom = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 2, 1));
        IPeriod month = new YearMonth(2024, 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(custom.Equals(month), Is.False);
            Assert.That(month.Equals(custom), Is.False);
        }
    }

    [Test]
    public void Equal_OperatorsAreSymmetric()
    {
        var custom = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 2, 1));
        IPeriod month = new YearMonth(2024, 1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(custom == month, Is.True);
            Assert.That(month == custom, Is.True);
            Assert.That(custom != month, Is.False);
            Assert.That(month != custom, Is.False);
        }
    }

    [Test]
    public void Equal_SameValue_True()
    {
        var period = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 2, 1));
        Assert.That(period.Equals(period), Is.True);
    }

    [Test]
    public void Equal_Null_False()
    {
        var period = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 2, 1));
        Assert.That(period.Equals(null), Is.False);
    }

    [Test]
    public void Equal_DifferentType_False()
    {
        var period = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 2, 1));
        Assert.That(period.Equals("not a period"), Is.False);
    }

    [Test]
    public void Contains_NestedPeriod_True()
    {
        var outer = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 2, 1));
        var inner = new CustomPeriod(new DateOnly(2024, 1, 10), new DateOnly(2024, 1, 20));
        Assert.That(outer.Contains(inner), Is.True);
    }

    [Test]
    public void Overlaps_IntersectingPeriod_True()
    {
        var left = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 21));
        var right = new CustomPeriod(new DateOnly(2024, 1, 10), new DateOnly(2024, 2, 11));
        Assert.That(left.Overlaps(right), Is.True);
    }

    [Test]
    public void Precedes_SeparatedPeriod_True()
    {
        var left = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 21));
        var right = new CustomPeriod(new DateOnly(2024, 1, 25), new DateOnly(2024, 2, 11));
        Assert.That(left.Precedes(right), Is.True);
    }

    [Test]
    public void Succeeds_LaterPeriod_True()
    {
        var left = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 21));
        var right = new CustomPeriod(new DateOnly(2024, 1, 25), new DateOnly(2024, 2, 11));
        Assert.That(right.Succeeds(left), Is.True);
    }

    [Test]
    public void AdjacentPeriods_MeetWithoutOverlapping()
    {
        var left = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 21));
        var right = new CustomPeriod(new DateOnly(2024, 1, 21), new DateOnly(2024, 2, 11));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(left.Meets(right), Is.True);
            Assert.That(left.Overlaps(right), Is.False);
            Assert.That(left.Precedes(right), Is.True);
            Assert.That(right.Succeeds(left), Is.True);
            Assert.That(left.Gap(right), Is.Zero);
        }
    }

    [Test]
    public void Intersect_OverlappingPeriods_UsesCanonicalBounds()
    {
        var left = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 21));
        var right = new CustomPeriod(new DateOnly(2024, 1, 15), new DateOnly(2024, 2, 11));
        Assert.That(left.Intersect(right), Is.EqualTo(
            new CustomPeriod(new DateOnly(2024, 1, 15), new DateOnly(2024, 1, 21))));
    }

    [Test]
    public void Span_DistinctPeriods_UsesCanonicalBounds()
    {
        var left = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 21));
        var right = new CustomPeriod(new DateOnly(2024, 1, 25), new DateOnly(2024, 2, 11));
        Assert.That(left.Span(right), Is.EqualTo(
            new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 2, 11))));
    }

    [Test]
    public void Gap_SeparatedPeriods_CountsExcludedDates()
    {
        var left = new CustomPeriod(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 21));
        var right = new CustomPeriod(new DateOnly(2024, 1, 25), new DateOnly(2024, 2, 11));
        Assert.That(left.Gap(right), Is.EqualTo(4));
    }
}
