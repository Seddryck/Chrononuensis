using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text.Unicode;
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

    protected object[] ParseInternal(ReadOnlySpan<byte> input, string format, IFormatProvider? provider, Type[] types)
        => ParseUtf8(input, format, provider, types, throwOnFailure: true)!;

    protected bool TryParseInternal(ReadOnlySpan<byte> input, string format, IFormatProvider? provider, Type[] types, out object[]? results)
    {
        results = ParseUtf8(input, format, provider, types, throwOnFailure: false);
        return results is not null;
    }

    private object[]? ParseUtf8(ReadOnlySpan<byte> input, string format, IFormatProvider? provider, Type[] types, bool throwOnFailure)
    {
        const int StackBufferSize = 256;
        char[]? rented = null;
        Span<char> buffer = input.Length <= StackBufferSize
            ? stackalloc char[input.Length]
            : (rented = ArrayPool<char>.Shared.Rent(input.Length));

        try
        {
            var status = Utf8.ToUtf16(input, buffer, out _, out var charsWritten, replaceInvalidSequences: false);
            if (status != OperationStatus.Done)
            {
                if (throwOnFailure)
                    throw new FormatException("Input is not valid UTF-8.");

                return null;
            }

            ReadOnlySpan<char> decoded = buffer[..charsWritten];
            if (throwOnFailure)
                return ParseInternal(decoded, format, provider, types);

            TryParseInternal(decoded, format, provider, types, out var results);
            return results;
        }
        finally
        {
            if (rented is not null)
                ArrayPool<char>.Shared.Return(rented);
        }
    }
}
