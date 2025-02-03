using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Parsers;
using NUnit.Framework;

namespace Chrononuensis.Testing.Parsers;
public class YearParserTests
{
    [TestCase("25", "yy", 2025)]
    [TestCase("05", "yy", 2005)]
    [TestCase("75", "yy", 1975)]
    [TestCase("2025", "yyyy", 2025)]
    public void Parse_InputFormat_CorrectValue(string input, string format, int expected)
    {
        var parser = new YearParser();
        var result = parser.Parse(input, format, null);
        Assert.That(result, Is.EqualTo(expected));
    }
}
