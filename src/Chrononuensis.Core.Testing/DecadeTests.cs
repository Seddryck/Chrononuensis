using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Chrononuensis.Extensions;

namespace Chrononuensis.Testing;
public class DecadeTests
{
    [TestCase(2020)]
    public void Ctor_ValidValues_Expected(int decade)
        => Assert.DoesNotThrow(() => new Decade(decade));

    [TestCase(2021)]
    public void Ctor_InvalidValues_Throws(int decade)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Decade(decade));
        Assert.That(ex.ParamName, Is.EqualTo("Decade"));
        Assert.That(ex.Message, Does.StartWith($"Invalid decade: The value must be a multiple of 10."));
    }

    [Test]
    public void Parse_InputDefaultFormat_Equal()
        => Assert.That(Decade.Parse("2020s", "tttt's'"), Is.EqualTo(new Decade(2020)));

    private static IEnumerable<Decade> GetData()
    {
        yield return new(2010);
        yield return new(2020);
        yield return new(2030);
    }

    [TestCaseSource(nameof(GetData))]
    public void LessThan_Decade_Compared(Decade right)
        => Assert.That(new Decade(2000) < right, Is.True);

    [TestCaseSource(nameof(GetData))]
    public void LessThanOrEqual_Decade_Compared(Decade right)
        => Assert.That(new Decade(2010) <= right, Is.True);

    [Test]
    public void LessThanOrEqual_Decade_Compared()
        => Assert.That(new Decade(2020) <= new Decade(2020), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThan_Decade_Compared(Decade right)
        => Assert.That(new Decade(2040) > right, Is.True);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThanOrEqual_Decade_Compared(Decade right)
        => Assert.That(new Decade(2030) >= right, Is.True);

    [Test]
    public void GreaterThanOrEqual_Decade_Compared()
        => Assert.That(new Decade(2020) >= new Decade(2020), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void NotEqual_Decade_Compared(Decade right)
        => Assert.That(new Decade(2000) != right, Is.True);

    [Test]
    public void Equal_Decade_Compared()
        => Assert.That(new Decade(2020) == new Decade(2020), Is.True);

    [Test]
    public void CompareTo_Null_Compared()
        => Assert.That(new Decade(2020).CompareTo(null), Is.EqualTo(1));

    [Test]
    public void CompareTo_ItSelf_Compared()
        => Assert.That(new Decade(2020).CompareTo(new Decade(2020)), Is.EqualTo(0));

    [Test]
    public void CompareTo_SomethingElse_Compared()
        => Assert.That(new Decade(2020).CompareTo(new Decade(2030)), Is.EqualTo(-1));

    [Test]
    [TestCase("2020s", true)]
    [TestCase("2025s", false)]
    public void TryParse_SomeValue_Expected(string input, bool expected)
    {
        var result = Decade.TryParse(input, null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new Decade(2020)));
        });
    }

    [Test]
    [TestCase("20s", true)]
    [TestCase("X5s", false)]
    public void TryParse_SomeValueWithFormat_Expected(string input, bool expected)
    {
        var result = Decade.TryParse(input, "tt's'", null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new Decade(2020)));
        });
    }

    [Test]
    [TestCase("2020s")]
    public void Parse_SomeValueAsSpan_Expected(string input)
    {
        var value = Decade.Parse(input.AsSpan(), null);
        Assert.That(value, Is.EqualTo(new Decade(2020)));
    }

    [Test]
    [TestCase("20s")]
    public void Parse_SomeValueFormatAsSpan_Expected(string input)
    {
        var value = Decade.Parse(input.AsSpan(), "tt's'", null);
        Assert.That(value, Is.EqualTo(new Decade(2020)));
    }

    [Test]
    [TestCase("2020s", true)]
    [TestCase("2025s", false)]
    public void TryParse_SomeValueAsSpan_Expected(string input, bool expected)
    {
        var result = Decade.TryParse(input.AsSpan(), null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new Decade(2020)));
        });
    }

    [Test]
    [TestCase("20s", true)]
    [TestCase("25s", false)]
    public void TryParse_SomeValueWithFormatAsSpan_Expected(string input, bool expected)
    {
        var result = Decade.TryParse(input.AsSpan(), "tt's'", null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new Decade(2020)));
        });
    }

    [TestCase("2020s", 1, "2030s")]
    [TestCase("2030s", 5, "2080s")]
    public void AddYear_SomeValue_Expected(string input, int value, string expected)
    {
        var result = Decade.Parse(input, null).AddDecade(value);
        Assert.That(result, Is.EqualTo(Decade.Parse(expected, null)));
    }

    [Test]
    [TestCase("2020s", 3653)]
    [TestCase("2030s", 3652)]
    public void Days_SomeValue_Expected(string input, int expected)
    {
        var value = Decade.Parse(input, null);
        Assert.That(value.Days, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("2020s", "2020-01-01")]
    public void FirstDate_SomeValue_Expected(string input, DateOnly expected)
    {
        var value = Decade.Parse(input, null);
        Assert.That(value.FirstDate, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("2020s", "2029-12-31")]
    public void LastDate_SomeValue_Expected(string input, DateOnly expected)
    {
        var value = Decade.Parse(input, null);
        Assert.That(value.LastDate, Is.EqualTo(expected));
    }

    [TestCase("2020s", "2020-01-01")]
    public void LowerBound_SomeValue_Expected(string input, DateTime expected)
    {
        var value = Decade.Parse(input, null);
        Assert.That(value.LowerBound, Is.EqualTo(expected));
    }

    [TestCase("2020s", "2030-01-01")]
    public void UpperBound_SomeValue_Expected(string input, DateTime expected)
    {
        var value = Decade.Parse(input, null);
        Assert.That(value.UpperBound, Is.EqualTo(expected));
    }
}
