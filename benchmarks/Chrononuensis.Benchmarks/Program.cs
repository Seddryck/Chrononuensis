using System.Diagnostics;
using System.Globalization;
using Chrononuensis.Formats;
using Chrononuensis.Formats.Tokens;
using Chrononuensis.Parsers;
using Chrononuensis.Parsers.Internals;
using Pidgin;

namespace Chrononuensis.Benchmarks;

internal static partial class Program
{
    private const int Iterations = 100_000;
    private static object? sink;

    private static void Main()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
        Console.WriteLine($"{System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}; {System.Runtime.InteropServices.RuntimeInformation.OSDescription}; {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}");
        Console.WriteLine($"Release build; {Iterations:N0} operations/sample; 9 alternating samples; medians; current-thread managed allocations");
        var cached = new YearMonthParser();
        var lexer = new Lexer();
        var factory = new ParserFactory();
        Compare("Repeated yyyy-MM Parse", () =>
        {
            // Original ParseInternal: tokenize, compose, parse, extract results.
            var tokens = lexer.Tokenize("yyyy-MM");
            var result = Primitives.CombineParsers(tokens.Select(factory.Create).ToArray()).Parse("2025-06");
            var results = new List<object>();
            foreach (var type in new[] { typeof(IYearToken), typeof(IMonthToken) })
                results.Add(result.Value[tokens.GetIndex(type)]);
            var values = results.ToArray();
            sink = ((int)values[0], (int)values[1]);
        }, () => sink = cached.Parse("2025-06", "yyyy-MM", null));
        RunNumeric();
    }

    static partial void RunNumeric();

    private static void Compare(string name, Action before, Action after)
    {
        for (var i = 0; i < 20_000; i++) { before(); after(); }
        var baseline = new (double Ns, double Bytes)[9];
        var optimized = new (double Ns, double Bytes)[9];
        for (var i = 0; i < baseline.Length; i++)
        {
            if (i % 2 == 0) { baseline[i] = Measure(before); optimized[i] = Measure(after); }
            else { optimized[i] = Measure(after); baseline[i] = Measure(before); }
        }
        var b = (Ns: baseline.Select(x => x.Ns).Order().ElementAt(4), Bytes: baseline.Select(x => x.Bytes).Order().ElementAt(4));
        var a = (Ns: optimized.Select(x => x.Ns).Order().ElementAt(4), Bytes: optimized.Select(x => x.Bytes).Order().ElementAt(4));
        Console.WriteLine($"{name}: before {b.Ns:F2} ns/op, {b.Bytes:F2} B/op; after {a.Ns:F2} ns/op, {a.Bytes:F2} B/op; time {100 * (a.Ns / b.Ns - 1):F1}%; allocations {100 * (a.Bytes / b.Bytes - 1):F1}%");
    }

    private static (double Ns, double Bytes) Measure(Action action)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var allocated = GC.GetAllocatedBytesForCurrentThread();
        var start = Stopwatch.GetTimestamp();
        for (var i = 0; i < Iterations; i++) action();
        var elapsed = Stopwatch.GetTimestamp() - start;
        var bytes = GC.GetAllocatedBytesForCurrentThread() - allocated;
        GC.KeepAlive(sink);
        return (elapsed * (1e9 / Stopwatch.Frequency) / Iterations, (double)bytes / Iterations);
    }
}

