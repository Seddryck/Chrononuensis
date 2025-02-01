using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Parsers;
using NUnit.Framework;

namespace Chrononuensis.Testing.Parsers;
public class YearWeekParserTests
{
    [TestCase("25-W1", "yy-'W'w", 1)]
    [TestCase("25-W17", "yy-'W'w", 17)]
    [TestCase("25-W01", "yy-'W'ww", 1)]
    [TestCase("25-W17", "yy-'W'ww", 17)]
    [TestCase("2025-W7", "yyyy-'W'w", 7)]
    [TestCase("2025-W17", "yyyy-'W'ww", 17)]
    public void Parse_InputFormat_CorrectValue(string input, string format, int expected)
    {
        var parser = new YearWeekParser();
        var result = parser.Parse(input, format, null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Year, Is.EqualTo(2025));
            Assert.That(result.Week, Is.EqualTo(expected));
        });
    }
}
