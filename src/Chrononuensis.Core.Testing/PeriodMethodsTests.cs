using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Chrononuensis;
using System.Text;

namespace Chrononuensis.Testing;

[TestFixture]
public class PeriodMethodsTests
{
    /// <summary>
    /// Manually defined test cases for Contains.
    /// </summary>
    public static IEnumerable<TestCaseData> ContainsTestCases()
    {
        var century = new Century(20);
        var decade = new Decade(2020);
        var year = new Year(2025);
        var semester = new YearSemester(2025, 1);
        var quarter = new YearQuarter(2025, 1);
        var month = new YearMonth(2025, 1);
        var week = new YearWeek(2025, 5);
        var day = new YearDay(2025, 15);

        yield return new TestCaseData(century, decade).Returns(false);
        yield return new TestCaseData(decade, year).Returns(true);
        yield return new TestCaseData(year, semester).Returns(true);
        yield return new TestCaseData(semester, quarter).Returns(true);
        yield return new TestCaseData(quarter, month).Returns(true);
        yield return new TestCaseData(month, week).Returns(false);
        yield return new TestCaseData(year, day).Returns(true);
        yield return new TestCaseData(quarter, day).Returns(true);
        yield return new TestCaseData(month, day).Returns(true);
        yield return new TestCaseData(week, day).Returns(false);
        yield return new TestCaseData(day, month).Returns(false);
    }

    [TestCaseSource(nameof(ContainsTestCases))]
    public bool Contains_Periods_Expected(IPeriod outer, IPeriod inner)
        => outer.Contains(inner);

    /// <summary>
    /// Manually defined test cases for Overlaps.
    /// </summary>
    public static IEnumerable<TestCaseData> OverlapsTestCases()
    {
        var century = new Century(20);
        var decade = new Decade(2020);
        var year = new Year(2025);
        var semester = new YearSemester(2025, 1);
        var quarter = new YearQuarter(2025, 1);
        var month = new YearMonth(2025, 1);
        var week = new YearWeek(2025, 5);
        var day = new YearDay(2025, 15);

        yield return new TestCaseData(century, decade).Returns(false);
        yield return new TestCaseData(decade, year).Returns(true);
        yield return new TestCaseData(year, semester).Returns(true);
        yield return new TestCaseData(semester, quarter).Returns(true);
        yield return new TestCaseData(quarter, month).Returns(true);
        yield return new TestCaseData(month, week).Returns(true);
        yield return new TestCaseData(year, day).Returns(true);
        yield return new TestCaseData(quarter, day).Returns(true);
        yield return new TestCaseData(month, day).Returns(true);
        yield return new TestCaseData(week, day).Returns(false);
        yield return new TestCaseData(day, month).Returns(true);
    }

    [TestCaseSource(nameof(OverlapsTestCases))]
    public bool Overlaps_Periods_Expected(IPeriod p1, IPeriod p2)
        => p1.Overlaps(p2);

    /// <summary>
    /// Manually defined test cases for Meets.
    /// </summary>
    public static IEnumerable<TestCaseData> MeetsTestCases()
    {
        var century = new Century(20);
        var decade = new Decade(2020);
        var year = new Year(2025);
        var semester = new YearSemester(2024, 2);
        var quarter = new YearQuarter(2025, 1);
        var month = new YearMonth(2025, 1);
        var week = new YearWeek(2025, 5);
        var day = new YearDay(2025, 15);

        yield return new TestCaseData(century, decade).Returns(false);
        yield return new TestCaseData(decade, year).Returns(false);
        yield return new TestCaseData(year, semester).Returns(true);
        yield return new TestCaseData(semester, quarter).Returns(true);
        yield return new TestCaseData(quarter, month).Returns(false);
        yield return new TestCaseData(month, week).Returns(false);
        yield return new TestCaseData(year, day).Returns(false);
        yield return new TestCaseData(quarter, day).Returns(false);
        yield return new TestCaseData(month, day).Returns(false);
        yield return new TestCaseData(week, day).Returns(false);
        yield return new TestCaseData(day, month).Returns(false);
    }

    [TestCaseSource(nameof(MeetsTestCases))]
    public bool Meets_Periods_Expected(IPeriod p1, IPeriod p2)
        => p1.Meets(p2);

    /// <summary>
    /// Manually defined test cases for Succeeds.
    /// </summary>
    public static IEnumerable<TestCaseData> PrecedesTestCases()
    {
        var century = new Century(20);
        var decade = new Decade(2020);
        var year = new Year(2025);
        var semester = new YearSemester(2025, 1);
        var quarter = new YearQuarter(2025, 1);
        var thirdQuarter = new YearQuarter(2025, 3);
        var month = new YearMonth(2025, 1);
        var week = new YearWeek(2025, 5);
        var day = new YearDay(2025, 15);

        yield return new TestCaseData(century, decade).Returns(true);
        yield return new TestCaseData(decade, year).Returns(false);
        yield return new TestCaseData(year, semester).Returns(false);
        yield return new TestCaseData(semester, thirdQuarter).Returns(true);
        yield return new TestCaseData(quarter, month).Returns(false);
        yield return new TestCaseData(month, week).Returns(false);
        yield return new TestCaseData(year, day).Returns(false);
        yield return new TestCaseData(quarter, day).Returns(false);
        yield return new TestCaseData(month, day).Returns(false);
        yield return new TestCaseData(week, day).Returns(false);
        yield return new TestCaseData(day, month).Returns(false);
    }

    [TestCaseSource(nameof(PrecedesTestCases))]
    public bool Precedes_Periods_Expected(IPeriod p1, IPeriod p2)
        => p1.Precedes(p2);

    /// <summary>
    /// Manually defined test cases for Succeeds.
    /// </summary>
    public static IEnumerable<TestCaseData> SucceedsTestCases()
    {
        var century = new Century(20);
        var decade = new Decade(2020);
        var year = new Year(2025);
        var semester = new YearSemester(2025, 1);
        var quarter = new YearQuarter(2025, 1);
        var thirdQuarter = new YearQuarter(2025, 3);
        var month = new YearMonth(2025, 1);
        var week = new YearWeek(2025, 5);
        var day = new YearDay(2025, 15);

        yield return new TestCaseData(century, decade).Returns(false);
        yield return new TestCaseData(decade, year).Returns(false);
        yield return new TestCaseData(year, semester).Returns(false);
        yield return new TestCaseData(semester, quarter).Returns(false);
        yield return new TestCaseData(quarter, month).Returns(false);
        yield return new TestCaseData(month, week).Returns(false);
        yield return new TestCaseData(year, day).Returns(false);
        yield return new TestCaseData(quarter, day).Returns(false);
        yield return new TestCaseData(month, day).Returns(false);
        yield return new TestCaseData(week, day).Returns(true);
        yield return new TestCaseData(day, month).Returns(false);
        yield return new TestCaseData(thirdQuarter, semester).Returns(true);
    }

    [TestCaseSource(nameof(SucceedsTestCases))]
    public bool Succeeds_Periods_Expected(IPeriod p1, IPeriod p2)
        => p1.Succeeds(p2);

    /// <summary>
    /// Manually defined test cases for Intersect.
    /// </summary>
    public static IEnumerable<TestCaseData> IntersectTestCases()
    {
        var century = new Century(20);
        var decade = new Decade(2020);
        var year = new Year(2025);
        var semester = new YearSemester(2025, 1);
        var quarter = new YearQuarter(2024, 1);
        var month = new YearMonth(2027, 1);
        var week = new YearWeek(2026, 5);
        var day = new YearDay(2015, 15);

        yield return new TestCaseData(century, decade).Returns(null);
        yield return new TestCaseData(decade, year).Returns(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 12, 31)));
        yield return new TestCaseData(year, semester).Returns(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 6, 30)));
        yield return new TestCaseData(semester, quarter).Returns(null);
        yield return new TestCaseData(quarter, month).Returns(null);
        yield return new TestCaseData(month, week).Returns(null);
        yield return new TestCaseData(year, day).Returns(null);
        yield return new TestCaseData(quarter, day).Returns(null);
        yield return new TestCaseData(month, day).Returns(null);
        yield return new TestCaseData(week, day).Returns(null);
        yield return new TestCaseData(day, month).Returns(null);
    }

    [TestCaseSource(nameof(IntersectTestCases))]
    public IPeriod? Intersect_Periods_Expected(IPeriod p1, IPeriod p2)
        => p1.Intersect(p2);

    /// <summary>
    /// Manually defined test cases for Intersect.
    /// </summary>
    public static IEnumerable<TestCaseData> SpanTestCases()
    {
        var century = new Century(20);
        var decade = new Decade(2020);
        var year = new Year(2025);
        var semester = new YearSemester(2025, 1);
        var quarter = new YearQuarter(2025, 1);
        var month = new YearMonth(2025, 1);
        var week = new YearWeek(2025, 5);
        var day = new YearDay(2025, 15);

        yield return new TestCaseData(century, decade).Returns(new CustomPeriod(new DateOnly(1901, 1, 1), new DateOnly(2029, 12, 31)));
        yield return new TestCaseData(decade, year).Returns(new CustomPeriod(new DateOnly(2020, 1, 1), new DateOnly(2029, 12, 31)));
        yield return new TestCaseData(year, semester).Returns(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 12, 31)));
        yield return new TestCaseData(semester, quarter).Returns(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 6, 30)));
        yield return new TestCaseData(quarter, month).Returns(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 3, 31)));
        yield return new TestCaseData(month, week).Returns(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 02, 2)));
        yield return new TestCaseData(year, day).Returns(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 12, 31)));
        yield return new TestCaseData(quarter, day).Returns(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 3, 31)));
        yield return new TestCaseData(month, day).Returns(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31)));
        yield return new TestCaseData(week, day).Returns(new CustomPeriod(new DateOnly(2025, 1, 15), new DateOnly(2025, 2, 2)));
        yield return new TestCaseData(day, month).Returns(new CustomPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31)));
    }

    [TestCaseSource(nameof(SpanTestCases))]
    public IPeriod? Span_Periods_Expected(IPeriod p1, IPeriod p2)
        => p1.Span(p2);

    /// <summary>
    /// Manually defined test cases for Intersect.
    /// </summary>
    public static IEnumerable<TestCaseData> GapTestCases()
    {
        var century = new Century(20);
        var decade = new Decade(2020);
        var year = new Year(2025);
        var semester = new YearSemester(2025, 1);
        var quarter = new YearQuarter(2025, 4);
        var month = new YearMonth(2025, 1);
        var week = new YearWeek(2025, 6);
        var day = new YearDay(2025, 15);

        yield return new TestCaseData(century, decade).Returns(19 * 365 + 4);
        yield return new TestCaseData(decade, year).Returns(0);
        yield return new TestCaseData(year, semester).Returns(0);
        yield return new TestCaseData(semester, quarter).Returns(92);
        yield return new TestCaseData(quarter, month).Returns(242);
        yield return new TestCaseData(month, week).Returns(2);
        yield return new TestCaseData(year, day).Returns(0);
        yield return new TestCaseData(quarter, day).Returns(258);
        yield return new TestCaseData(month, day).Returns(0);
        yield return new TestCaseData(week, day).Returns(18);
        yield return new TestCaseData(day, month).Returns(0);
    }

    [TestCaseSource(nameof(GapTestCases))]
    public int Gap_Periods_Expected(IPeriod p1, IPeriod p2)
        => p1.Gap(p2);
}
