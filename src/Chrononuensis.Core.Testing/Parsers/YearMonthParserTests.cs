using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Parsers;
using NUnit.Framework;

namespace Chrononuensis.Testing.Parsers;
public class YearMonthParserTests
{
    [TestCase("25-1", "yy-M")]
    [TestCase("25-01", "yy-MM")]
    [TestCase("25-Jan", "yy-MMM")]
    [TestCase("25 January", "yy MMMM")]
    [TestCase("2025-1", "yyyy-M")]
    [TestCase("2025-01", "yyyy-MM")]
    [TestCase("2025-Jan", "yyyy-MMM")]
    [TestCase("2025 January", "yyyy MMMM")]
    public void Parse_InputFormat_CorrectValue(string input, string format)
    {
        var parser = new YearMonthParser();
        var result = parser.Parse(input, format, null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Year, Is.EqualTo(2025));
            Assert.That(result.Month, Is.EqualTo(1));
        });
    }

    [Test]
    public void Parse_UnexpectedLiteral_Throws()
    {
        var parser = new YearMonthParser();
        var ex = Assert.Throws<FormatException>(() => parser.Parse("2025*Jan", "yyyy-MM", null));
        Assert.That(ex.Message, Is.EqualTo("Parsing error at character 5: Expected '-' but found '*'."));
    }

    [Test]
    public void Parse_OverMax_Throws()
    {
        var parser = new YearMonthParser();
        var ex = Assert.Throws<FormatException>(() => parser.Parse("2025-56", "yyyy-MM", null));
        Assert.That(ex.Message, Is.EqualTo("Parsing error at character 8: Value must be between 1 and 12."));
    }

    [Test]
    public void Parse_UnexpectedLiteralForDigit_Throws()
    {
        var parser = new YearMonthParser();
        var ex = Assert.Throws<FormatException>(() => parser.Parse("2025-Jan", "yyyy-MM", null));
        Assert.That(ex.Message, Is.EqualTo("Parsing error at character 6: Unexpected 'J'."));
    }

    [Test]
    public void TryParse_UnexpectedLiteralForDigit_DoesNotThrow()
    {
        var parser = new YearMonthParser();
        var result = parser.TryParse("2025-XYZ", "yyyy-MM", null, out var year, out var month);
        Assert.That(result, Is.False);
        Assert.That(year, Is.Null);
        Assert.That(month, Is.Null);
    }

    [Test]
    public void TryParse_ValidFormat_ValuesSpecified()
    {
        var parser = new YearMonthParser();
        var result = parser.TryParse("2025-Jan", "yyyy-MMM", null, out var year, out var month);
        Assert.That(result, Is.True);
        Assert.That(year, Is.EqualTo(2025));
        Assert.That(month, Is.EqualTo(1));
    }
}
