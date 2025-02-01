using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Parsers;
using NUnit.Framework;

namespace Chrononuensis.Testing.Parsers;
public class YearDayParserTests
{
    [TestCase("25-D1", "yy-'D'j", 1)]
    [TestCase("25-D17", "yy-'D'j", 17)]
    [TestCase("25-D117", "yy-'D'j", 117)]
    [TestCase("25-D001", "yy-'D'jjj", 1)]
    [TestCase("25-D017", "yy-'D'jjj", 17)]
    [TestCase("25-D117", "yy-'D'jjj", 117)]
    [TestCase("2025-D17", "yyyy-'D'j", 17)]
    [TestCase("2025-D017", "yyyy-'D'jjj", 17)]
    public void Parse_InputFormat_CorrectValue(string input, string format, int expected)
    {
        var parser = new YearDayParser();
        var result = parser.Parse(input, format, null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Year, Is.EqualTo(2025));
            Assert.That(result.DayOfYear, Is.EqualTo(expected));
        });
    }
}
