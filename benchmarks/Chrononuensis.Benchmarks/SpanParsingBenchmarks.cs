using System.Globalization;
using Chrononuensis.Parsers;

namespace Chrononuensis.Benchmarks;

internal static partial class Program
{
    private static volatile int periodSink;

    private static void RunTypedParsing()
    {
        MeasureSpanCase("YearMonth.Parse yyyy-MM", static () =>
        {
            var period = YearMonth.Parse("2025-06".AsSpan(), "yyyy-MM", null);
            periodSink = period.Year + period.Month;
        });

        MeasureSpanCase("YearMonth.TryParse yyyy-MM", static () =>
        {
            if (YearMonth.TryParse("2025-06".AsSpan(), "yyyy-MM", null, out var period))
                periodSink = period.Year + period.Month;
        });

        MeasureSpanCase("Year.Parse yyyy", static () =>
        {
            periodSink = Year.Parse("2025".AsSpan(), "yyyy", null).Value;
        });
    }

    private static void RunSpanParsing()
    {
        // Cycling through more formats than the cache holds keeps cold calls missing
        // after JIT warm-up. Prepare inputs and formats outside the timed interval.
        var cases = Enumerable.Range(0, 1024)
            .Select(i => (Input: $"25-January {i:D4}", Format: $"yy-MMMM '{i:D4}'"))
            .ToArray();
        var parser = new YearMonthParser();

        foreach (var provider in new[] { null, CultureInfo.InvariantCulture })
        {
            var providerLabel = provider is null ? "implicit" : "explicit";
            var warm = cases[0];
            MeasureSpanCase($"{providerLabel}, warm", () => sink = parser.Parse(warm.Input.AsSpan(), warm.Format, provider));

            var index = 0;
            MeasureSpanCase($"{providerLabel}, cold", () =>
            {
                var (input, format) = cases[index];
                index = (index + 1) % cases.Length;
                sink = parser.Parse(input.AsSpan(), format, provider);
            });
        }
    }

    private static void MeasureSpanCase(string name, Action parse)
    {
        for (var i = 0; i < 20_000; i++)
            parse();

        var samples = new (double Ns, double Bytes)[9];
        for (var i = 0; i < samples.Length; i++)
            samples[i] = Measure(parse);

        var ns = samples.Select(sample => sample.Ns).Order().ElementAt(samples.Length / 2);
        var bytes = samples.Select(sample => sample.Bytes).Order().ElementAt(samples.Length / 2);
        Console.WriteLine($"{name}: {ns:F2} ns/op, {bytes:F2} B/op");
    }
}
