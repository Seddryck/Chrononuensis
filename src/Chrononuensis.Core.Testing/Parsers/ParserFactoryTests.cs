using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Formats.Tokens;
using Chrononuensis.Formats.Tokens.Month;
using Chrononuensis.Parsers;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Chrononuensis.Testing.Parsers;
public class ParserFactoryTests
{
    [Test]
    public void Create_Existing_Token()
    {
        var factory = new ParserFactory();
        var parser = factory.Create(AbbreviationMonthToken.Instance);
        Assert.That(parser, Is.Not.Null);
    }

    private class UnknownToken: FormatToken
    { }

    [Test]
    public void Create_UnknowToken_Throws()
    {
        var factory = new ParserFactory();
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => factory.Create(new UnknownToken()));
        Assert.That(ex.Message, Does.Contain("Token Chrononuensis.Testing.Parsers.ParserFactoryTests+UnknownToken not found in the dictionary"));
    }
}
