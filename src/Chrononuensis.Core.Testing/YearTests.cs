﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Chrononuensis.Testing;
public class YearTests
{
    [Test]
    public void Parse_InputDefaultFormat_Equal()
        => Assert.That(Year.Parse("2021", "yyyy"), Is.EqualTo(new Year(2021)));

    private static IEnumerable<Year> GetData()
    {
        yield return new(2022);
        yield return new(2021);
        yield return new(2023);
    }

    [TestCaseSource(nameof(GetData))]
    public void LessThan_Year_Compared(Year right)
        => Assert.That(new Year(2020) < right, Is.True);

    [TestCaseSource(nameof(GetData))]
    public void LessThanOrEqual_Year_Compared(Year right)
        => Assert.That(new Year(2020) <= right, Is.True);

    [Test]
    public void LessThanOrEqual_Year_Compared()
        => Assert.That(new Year(2021) <= new Year(2021), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThan_Year_Compared(Year right)
        => Assert.That(new Year(2025) > right, Is.True);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThanOrEqual_Year_Compared(Year right)
        => Assert.That(new Year(2025) >= right, Is.True);

    [Test]
    public void GreaterThanOrEqual_Year_Compared()
        => Assert.That(new Year(2025) >= new Year(2025), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void NotEqual_Year_Compared(Year right)
        => Assert.That(new Year(2025) != right, Is.True);

    [Test]
    public void Equal_Year_Compared()
        => Assert.That(new Year(2025) == new Year(2025), Is.True);

    [Test]
    public void CompareTo_Null_Compared()
        => Assert.That(new Year(2025).CompareTo(null), Is.EqualTo(1));

    [Test]
    public void CompareTo_ItSelf_Compared()
        => Assert.That(new Year(2025).CompareTo(new Year(2025)), Is.EqualTo(0));

    [Test]
    public void CompareTo_SomethingElse_Compared()
        => Assert.That(new Year(2022).CompareTo(new Year(2025)), Is.EqualTo(-1));

    [Test]
    [TestCase("2025", true)]
    [TestCase("2X25", false)]
    public void TryParse_SomeValue_Expected(string input, bool expected)
    {
        var result = Year.TryParse(input, null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new Year(2025)));
        });
    }

    [Test]
    [TestCase("25", true)]
    [TestCase("2X", false)]
    public void TryParse_SomeValueWithFormat_Expected(string input, bool expected)
    {
        var result = Year.TryParse(input, "yy", null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new Year(2025)));
        });
    }

    [Test]
    [TestCase("2025-001")]
    public void Parse_SomeValueAsSpan_Expected(string input)
    {
        var value = Year.Parse(input.AsSpan(), null);
        Assert.That(value, Is.EqualTo(new Year(2025)));
    }

    [Test]
    [TestCase("25")]
    public void Parse_SomeValueFormatAsSpan_Expected(string input)
    {
        var value = Year.Parse(input.AsSpan(), "yy", null);
        Assert.That(value, Is.EqualTo(new Year(2025)));
    }

    [Test]
    [TestCase("2025", true)]
    [TestCase("202X", false)]
    public void TryParse_SomeValueAsSpan_Expected(string input, bool expected)
    {
        var result = Year.TryParse(input.AsSpan(), null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new Year(2025)));
        });
    }

    [Test]
    [TestCase("25", true)]
    [TestCase("2X", false)]
    public void TryParse_SomeValueWithFormatAsSpan_Expected(string input, bool expected)
    {
        var result = Year.TryParse(input.AsSpan(), "yy", null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new Year(2025)));
        });
    }
}
