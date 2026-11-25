using System.Globalization;
using Chrononuensis.Formats;
using Chrononuensis.Parsers.Internals;
using Pidgin;

namespace Chrononuensis.Parsers;

// Bound tokenized formats and implicit pipelines independently. Explicit pipelines
// can capture caller state and are built outside the lock for each operation.
internal sealed class ParserPipelineCache
{
    internal const int Capacity = 256;
    internal const int MaximumFormatLength = 1024;
    private readonly object gate = new();
    private readonly Dictionary<string, Format> tokenizedFormats = new(StringComparer.Ordinal);
    private readonly Queue<string> formatInsertionOrder = new();
    private readonly Dictionary<(string Format, string UICulture), Pipeline> pipelines = new();
    private readonly Queue<(string Format, string UICulture)> pipelineInsertionOrder = new();

    internal sealed record Pipeline(Format Tokens, Parser<char, object[]> Parser);

    internal Pipeline Get(string format, IFormatProvider? provider = null)
    {
        if (format is null || format.Length > MaximumFormatLength)
            return Create(new Lexer().Tokenize(format!), provider);

        if (provider is not null)
        {
            Format tokens;
            lock (gate)
                tokens = GetOrTokenize(format);

            return Create(tokens, provider);
        }

        var culture = CultureInfo.CurrentUICulture;
        var key = (format, culture.Name);
        lock (gate)
        {
            if (pipelines.TryGetValue(key, out var pipeline))
                return pipeline;

            pipeline = Create(GetOrTokenize(format), culture);
            if (pipelines.Count == Capacity)
                pipelines.Remove(pipelineInsertionOrder.Dequeue());
            pipelines.Add(key, pipeline);
            pipelineInsertionOrder.Enqueue(key);
            return pipeline;
        }
    }

    // Call under the cache lock so concurrent misses tokenize a retained format once.
    private Format GetOrTokenize(string format)
    {
        if (tokenizedFormats.TryGetValue(format, out var tokens))
            return tokens;

        tokens = new Lexer().Tokenize(format);
        if (tokenizedFormats.Count == Capacity)
            tokenizedFormats.Remove(formatInsertionOrder.Dequeue());
        tokenizedFormats.Add(format, tokens);
        formatInsertionOrder.Enqueue(format);
        return tokens;
    }

    private static Pipeline Create(Format tokens, IFormatProvider? provider)
    {
        var factory = new ParserFactory(provider);
        return new(tokens, Primitives.CombineParsers(tokens.Select(factory.Create).ToArray()).Before(Parser<char>.End));
    }
}
