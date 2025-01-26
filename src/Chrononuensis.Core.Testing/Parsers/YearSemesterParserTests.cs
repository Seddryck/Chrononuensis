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
}
