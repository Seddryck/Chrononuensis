using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Chrononuensis.Extensions;

namespace Chrononuensis.Testing;
public class CenturyTests
{
    [TestCase(20)]
    public void Ctor_ValidValues_Expected(int century)
        => Assert.DoesNotThrow(() => new Century(century));

    [Test]
    public void Parse_InputDefaultFormat_Equal()
        => Assert.That(Century.Parse("20", "c"), Is.EqualTo(new Century(20)));

    private static IEnumerable<Century> GetData()
    {
        yield return new(17);
        yield return new(18);
        yield return new(19);
    }

    [TestCaseSource(nameof(GetData))]
    public void LessThan_Century_Compared(Century right)
        => Assert.That(new Century(16) < right, Is.True);

    [TestCaseSource(nameof(GetData))]
    public void LessThanOrEqual_Century_Compared(Century right)
        => Assert.That(new Century(16) <= right, Is.True);

    [Test]
    public void LessThanOrEqual_Century_Compared()
        => Assert.That(new Century(20) <= new Century(20), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThan_Century_Compared(Century right)
        => Assert.That(new Century(21) > right, Is.True);

    [TestCaseSource(nameof(GetData))]
    public void GreaterThanOrEqual_Century_Compared(Century right)
        => Assert.That(new Century(21) >= right, Is.True);

    [Test]
    public void GreaterThanOrEqual_Century_Compared()
        => Assert.That(new Century(21) >= new Century(21), Is.True);

    [TestCaseSource(nameof(GetData))]
    public void NotEqual_Century_Compared(Century right)
        => Assert.That(new Century(20) != right, Is.True);

    [Test]
    public void Equal_Century_Compared()
        => Assert.That(new Century(20) == new Century(20), Is.True);

    [Test]
    public void CompareTo_Null_Compared()
        => Assert.That(new Century(2020).CompareTo(null), Is.EqualTo(1));

    [Test]
    public void CompareTo_ItSelf_Compared()
        => Assert.That(new Century(20).CompareTo(new Century(20)), Is.EqualTo(0));

    [Test]
    public void CompareTo_SomethingElse_Compared()
        => Assert.That(new Century(20).CompareTo(new Century(21)), Is.EqualTo(-1));

    [Test]
    [TestCase("20", true)]
    [TestCase("XX", false)]
    public void TryParse_SomeValue_Expected(string input, bool expected)
    {
        var result = Century.TryParse(input, null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            if (expected)
                Assert.That(value, Is.EqualTo(new Century(20)));
        });
    }

    [Test]
    [TestCase("20", "c", 20)]
    [TestCase("20", "cc", 20)]
    [TestCase("1", "c", 1)]
    [TestCase("01", "cc", 1)]
    [TestCase("XX", "{c:RN}", 20)]
    [TestCase("I", "{c:RN}", 1)]
    [TestCase("XXe", "{c:RN}['er'|'e']", 20)]
    [TestCase("XXth", "{c:RN}['st'|'nd'|'rd'|'th']", 20)]
    [TestCase("Ier", "{c:RN}['er'|'e']", 1)]
    [TestCase("Ist", "{c:RN}['st'|'nd'|'rd'|'th']", 1)]
    [TestCase("XXe siècle", "{c:RN}['er'|'e']' siècle'", 20)]
    [TestCase("Ier siècle", "{c:RN}['er'|'e']' siècle'", 1)]
    [TestCase("Ist century", "{c:RN}['st'|'nd'|'rd'|'th']' century'", 1)]
    [TestCase("XXth century", "{c:RN}['st'|'nd'|'rd'|'th']' century'", 20)]
    public void TryParse_SomeValueWithFormat_Expected(string input, string format, int expected)
    {
        var result = Century.TryParse(input, format, null, out var value);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(value, Is.EqualTo(new Century(expected)));
        });
    }

    [Test]
    [TestCase("20")]
    public void Parse_SomeValueAsSpan_Expected(string input)
    {
        var value = Century.Parse(input.AsSpan(), null);
        Assert.That(value, Is.EqualTo(new Century(20)));
    }

    [Test]
    [TestCase("20e")]
    public void Parse_SomeValueFormatAsSpan_Expected(string input)
    {
        var value = Century.Parse(input.AsSpan(), "{c}['e'|'er']'", null);
        Assert.That(value, Is.EqualTo(new Century(20)));
    }

    [TestCase("12", 1, "13")]
    [TestCase("12", 5, "17")]
    public void AddCentury_SomeValue_Expected(string input, int value, string expected)
    {
        var result = Century.Parse(input, null).AddCentury(value);
        Assert.That(result, Is.EqualTo(Century.Parse(expected, null)));
    }
}
