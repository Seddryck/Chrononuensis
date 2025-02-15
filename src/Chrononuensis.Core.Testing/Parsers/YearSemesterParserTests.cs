using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Parsers;
using NUnit.Framework;

namespace Chrononuensis.Testing.Parsers;
public class YearSemesterParserTests
{
    [TestCase("25-S1", "yy-'S'S")]
    [TestCase("25-H1", "yy-HS")]
    [TestCase("25-1", "yy-S")]
    [TestCase("S1.2025", "'S'S.yyyy")]
    public void Parse_InputFormat_CorrectValue(string input, string format)
    {
        var parser = new YearSemesterParser();
        var result = parser.Parse(input, format, null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Year, Is.EqualTo(2025));
            Assert.That(result.Semester, Is.EqualTo(1));
        });
    }

    [TestCase("1st semester of 2025", "{S}['st'|'nd']' semester of '{yyyy}")]
    [TestCase("2nd semester of 2025", "{S}['st'|'nd']' semester of '{yyyy}")]
    public void Parse_InputFormatWithMutuallyExclusive_CorrectValue(string input, string format)
    {
        var parser = new YearSemesterParser();
        var result = parser.Parse(input, format, null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Year, Is.EqualTo(2025));
            Assert.That(result.Semester, Is.GreaterThanOrEqualTo(1).And.LessThanOrEqualTo(2));
        });
    }

    [TestCase("2rd quarter of 2025", "{S}['st'|'nd']' quarter of '{yyyy}")]
    public void Parse_InputFormatWithMutuallyExclusive_IncorrectValue(string input, string format)
    {
        var parser = new YearSemesterParser();
        Assert.Throws<FormatException>(() => parser.Parse(input, format, null));
    }
}
