using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Parsers;
using NUnit.Framework;

namespace Chrononuensis.Testing.Parsers;
public class MonthDayParserTests
{
    [TestCase("25-Jan", "d-MMM")]
    [TestCase("25 January", "d MMMM")]
    [TestCase("25-1", "dd-M")]
    [TestCase("25-01", "dd-MM")]
    [TestCase("25-Jan", "dd-MMM")]
    [TestCase("25 January", "dd MMMM")]
    public void Parse_InputFormat_CorrectValue(string input, string format)
    {
        var parser = new MonthDayParser();
        var result = parser.Parse(input, format, null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Month, Is.EqualTo(1));
            Assert.That(result.Day, Is.EqualTo(25));
        });
    }

    [Test]
    public void Parse_UnexpectedLiteral_Throws()
    {
        var parser = new MonthDayParser();
        var ex = Assert.Throws<FormatException>(() => parser.Parse("25*Jan", "dd-MM", null));
        Assert.That(ex.Message, Is.EqualTo("Parsing error at character 3: Expected '-' but found '*'."));
    }

    [Test]
    public void Parse_OverMax_Throws()
    {
        var parser = new MonthDayParser();
        var ex = Assert.Throws<FormatException>(() => parser.Parse("32-07", "dd-MM", null));
        Assert.That(ex.Message, Is.EqualTo("Parsing error at character 3: Value must be between 1 and 31."));
    }

    [Test]
    public void Parse_UnexpectedLiteralForDigit_Throws()
    {
        var parser = new MonthDayParser();
        var ex = Assert.Throws<FormatException>(() => parser.Parse("25-Jan", "dd-MM", null));
        Assert.That(ex.Message, Is.EqualTo("Parsing error at character 4: Unexpected 'J'."));
    }
}
