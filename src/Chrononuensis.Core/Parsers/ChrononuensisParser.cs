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
    private static readonly ParserPipelineCache Pipelines = new();

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
        var pipeline = Pipelines.Get(format, provider);
        var result = pipeline.Parser.Parse(input);
        ThrowsIfNotSuccess(result);
        return CreateResults(result, pipeline.Tokens, types);
    }

    public object[] ParseInternal(ReadOnlySpan<char> input, string format, IFormatProvider? provider, Type[] types)
    {
        var pipeline = Pipelines.Get(format, provider);
        var result = pipeline.Parser.Parse(input);
        ThrowsIfNotSuccess(result);
        return CreateResults(result, pipeline.Tokens, types);
    }

    public bool TryParseInternal(string input, string format, IFormatProvider? provider, Type[] types, out object[]? results)
    {
        var pipeline = Pipelines.Get(format, provider);
        var result = pipeline.Parser.Parse(input);
        results = result.Success ? CreateResults(result, pipeline.Tokens, types) : null;
        return result.Success;
    }

    public bool TryParseInternal(ReadOnlySpan<char> input, string format, IFormatProvider? provider, Type[] types, out object[]? results)
    {
        var pipeline = Pipelines.Get(format, provider);
        var result = pipeline.Parser.Parse(input);
        results = result.Success ? CreateResults(result, pipeline.Tokens, types) : null;
        return result.Success;
    }
}
