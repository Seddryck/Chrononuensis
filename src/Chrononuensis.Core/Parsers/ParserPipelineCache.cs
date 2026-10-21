using System.Globalization;
using Chrononuensis.Formats;
using Chrononuensis.Parsers.Internals;
using Pidgin;

namespace Chrononuensis.Parsers;

// Shared across parser instances. FIFO eviction bounds retained formats; oversized
// formats are parsed normally without retaining their potentially large pipelines.
internal sealed class ParserPipelineCache
{
    internal const int Capacity = 256;
    internal const int MaximumFormatLength = 1024;
    private readonly object gate = new();
    private readonly Dictionary<(string Format, string UICulture), Pipeline> entries = new();
    private readonly Queue<(string Format, string UICulture)> insertionOrder = new();

    internal sealed record Pipeline(Format Tokens, Parser<char, object[]> Parser);

    internal Pipeline Get(string format)
    {
        var key = (format, CultureInfo.CurrentUICulture.Name);
        if (format is not null && format.Length > MaximumFormatLength)
            return Create(format);

        // Serialize misses as well as eviction so concurrent callers build a retained
        // pipeline only once. Parsing itself always happens outside this lock.
        lock (gate)
        {
            if (entries.TryGetValue(key!, out var pipeline))
                return pipeline;

            pipeline = Create(format!);
            if (entries.Count == Capacity)
                entries.Remove(insertionOrder.Dequeue());
            entries.Add(key!, pipeline);
            insertionOrder.Enqueue(key!);
            return pipeline;
        }
    }

    private static Pipeline Create(string format)
    {
        var tokens = new Lexer().Tokenize(format);
        var factory = new ParserFactory();
        return new(tokens, Primitives.CombineParsers(tokens.Select(factory.Create).ToArray()));
    }
}
