using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Parsers;
using NUnit.Framework;

namespace Chrononuensis.Testing.Parsers;
public class OlympiadParserTests
{
    [TestCase("I Olympiad", "{o:RN} 'Olympiad'", 1)]
    [TestCase("VI Olympiad", "{o:RN} 'Olympiad'", 6)]
    [TestCase("24 Olympiad", "o 'Olympiad'", 24)]
    public void Parse_InputFormat_CorrectValue(string input, string format, int expected)
    {
        var parser = new OlympiadParser();
        var result = parser.Parse(input, format, null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
        });
    }
}
