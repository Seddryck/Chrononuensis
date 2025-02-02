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

    private Result<char, object[]> CreateParseResult(ReadOnlySpan<char> input, Format tokens, IFormatProvider? provider, Type[] types)
    {
        var parsers = tokens.Select(factory.Create).ToArray();
        var parser = Primitives.CombineParsers(parsers);
        return parser.Parse(input);
    }

    private Format CreateTokens(string format)
        => lexer.Tokenize(format);

    private void ThrowsIfNotSuccess(Result<char, object[]> result)
    {
        if (result.Success == false && result.Error is not null)
        {
            var ex = new FormatExceptionFactory().Create(result);
            throw ex;
        }
    }

    private object[] CreateResults(Result<char, object[]> result, Format tokens, Type[] types)
    {
        var results = new List<object>();
        foreach (var type in types)
        {
            var index = tokens.GetIndex(type);
            if (index == -1)
                throw new FormatException($"Token '{type.Name[1..^5].ToLower()}' not found in the format");
            results.Add(result.Value[index]);
        }
        return [.. results];
    }

    public object[] ParseInternal(string input, string format, IFormatProvider? provider, Type[] types)
    {
        var tokens = CreateTokens(format);
        var result = CreateParseResult(input, tokens, provider, types);
        ThrowsIfNotSuccess(result);
        return CreateResults(result, tokens, types);
    }

    public object[] ParseInternal(ReadOnlySpan<char> input, string format, IFormatProvider? provider, Type[] types)
    {
        var tokens = CreateTokens(format);
        var result = CreateParseResult(input, tokens, provider, types);
        ThrowsIfNotSuccess(result);
        return CreateResults(result, tokens, types);
    }

    public bool TryParseInternal(string input, string format, IFormatProvider? provider, Type[] types, out object[]? results)
    {
        var tokens = CreateTokens(format);
        var result = CreateParseResult(input, tokens, provider, types);
        results = result.Success ? CreateResults(result, tokens, types) : null;
        return result.Success;
    }

    public bool TryParseInternal(ReadOnlySpan<char> input, string format, IFormatProvider? provider, Type[] types, out object[]? results)
    {
        var tokens = CreateTokens(format);
        var result = CreateParseResult(input, tokens, provider, types);
        results = result.Success ? CreateResults(result, tokens, types) : null;
        return result.Success;
    }
}
