using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Formats;
using Chrononuensis.Parsers.Internals;
using Pidgin;

namespace Chrononuensis.Parsers;
public abstract class ChrononuensisParser : IParser
{
    public object[] ParseInternal(string input, string format, IFormatProvider? provider, Type[] types)
    {
        var tokens = new Lexer().Tokenize(format);

        var factory = new ParserFactory();
        var parsers = tokens.Select(factory.Create).ToArray();

        var parser = Primitives.CombineParsers(parsers);
        var result = parser.Parse(input);

        if (result.Success == false && result.Error is not null)
        {
            var ex = new FormatExceptionFactory().Create(result);
            throw ex;
        }

        var results = new List<object>();
        foreach (var type in types)
        {
            var index = tokens.GetIndex(type);
            results.Add(result.Value[index]);
        }
        return [.. results];
    }
}
