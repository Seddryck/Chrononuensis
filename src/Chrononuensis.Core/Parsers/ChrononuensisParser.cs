using Chrononuensis.Formats;
using Pidgin;

namespace Chrononuensis.Parsers;
public abstract class ChrononuensisParser : IParser
{
    private static readonly ParserPipelineCache Pipelines = ParserPipelineCache.Shared;

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
                throw FormatExceptionFactory.MissingToken(type);
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

    protected object[] ParseInternal(ReadOnlySpan<byte> input, string format, IFormatProvider? provider, Type[] types)
        => ParseUtf8(input, format, provider, types, throwOnFailure: true)!;

    protected bool TryParseInternal(ReadOnlySpan<byte> input, string format, IFormatProvider? provider, Type[] types, out object[]? results)
    {
        results = ParseUtf8(input, format, provider, types, throwOnFailure: false);
        return results is not null;
    }

    private object[]? ParseUtf8(ReadOnlySpan<byte> input, string format, IFormatProvider? provider, Type[] types, bool throwOnFailure)
        => Utf8PeriodInput.Parse(input, (Parser: this, format, provider, types, throwOnFailure), throwOnFailure,
            static (decoded, state) =>
            {
                if (state.throwOnFailure)
                    return state.Parser.ParseInternal(decoded, state.format, state.provider, state.types);

                state.Parser.TryParseInternal(decoded, state.format, state.provider, state.types, out var results);
                return results;
            });
}
