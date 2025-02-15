using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Parsers;
using NUnit.Framework;

namespace Chrononuensis.Testing.Parsers;
public class YearQuarterParserTests
{
    [TestCase("25-Q1", "yy-Qq")]
    [TestCase("25-1", "yy-q")]
    [TestCase("Q1.2025", "Qq.yyyy")]
    public void Parse_InputFormat_CorrectValue(string input, string format)
    {
        var parser = new YearQuarterParser();
        var result = parser.Parse(input, format, null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Year, Is.EqualTo(2025));
            Assert.That(result.Quarter, Is.EqualTo(1));
        });
    }

    [TestCase("1st quarter of 2025", "{q}['st'|'nd'|'rd'|'th']' quarter of '{yyyy}")]
    [TestCase("2nd quarter of 2025", "{q}['st'|'nd'|'rd'|'th']' quarter of '{yyyy}")]
    [TestCase("3rd quarter of 2025", "{q}['st'|'nd'|'rd'|'th']' quarter of '{yyyy}")]
    [TestCase("4th quarter of 2025", "{q}['st'|'nd'|'rd'|'th']' quarter of '{yyyy}")]
    public void Parse_InputFormatWithMutuallyExclusive_CorrectValue(string input, string format)
    {
        var parser = new YearQuarterParser();
        var result = parser.Parse(input, format, null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Year, Is.EqualTo(2025));
            Assert.That(result.Quarter, Is.GreaterThanOrEqualTo(1).And.LessThanOrEqualTo(4));
        });
    }

    [TestCase("2kx quarter of 2025", "{q}['st'|'nd'|'rd'|'th']' quarter of '{yyyy}")]
    public void Parse_InputFormatWithMutuallyExclusive_IncorrectValue(string input, string format)
    {
        var parser = new YearQuarterParser();
        Assert.Throws<FormatException>(() => parser.Parse(input, format, null));
    }
}
