using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Formats;
using Chrononuensis.Parsers.Internals;
using Pidgin;

namespace Chrononuensis.Parsers;
public abstract class ChrononuensisParser : IParser
{
    private Lexer lexer = new();
    private ParserFactory factory = new();

    private Result<char, object[]> CreateParseResult(string input, Format tokens, IFormatProvider? provider, Type[] types)
    {
        var parsers = tokens.Select(factory.Create).ToArray();
        var parser = Primitives.CombineParsers(parsers);
        return parser.Parse(input);
    }

    private Format CreateTokens(string format)
        => lexer.Tokenize(format);

    public object[] ParseInternal(string input, string format, IFormatProvider? provider, Type[] types)
    {
        var tokens = CreateTokens(format);
        var result = CreateParseResult(input, tokens, provider, types);

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

    public bool TryParseInternal(string input, string format, IFormatProvider? provider, Type[] types, out object[]? results)
    {
        var tokens = CreateTokens(format);
        var result = CreateParseResult(input, tokens, provider, types);

        if (result.Success == true)
        {
            var temp = new List<object>();
            foreach (var type in types)
            {
                var index = tokens.GetIndex(type);
                temp.Add(result.Value[index]);
            }
            results = [.. temp];
            return true;
        }
        else
        {
            results = null;
            return false;
        }
    }
}
