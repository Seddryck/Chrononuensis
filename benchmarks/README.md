# Parser performance benchmarks

Run from the repository root:

```powershell
dotnet run --project benchmarks/Chrononuensis.Benchmarks -c Release -f net10.0 -p:NuGetAudit=false
```

The dependency-free harness warms each case with 20,000 operations, then alternates
nine samples of 100,000 operations per implementation. It reports median elapsed
nanoseconds and current-thread managed bytes per operation (including delegate
and result-sink costs equally on both sides). GC runs outside measured intervals.
Run on an otherwise idle machine; timing is machine-specific, not a CI threshold.
The baseline reproduces the original code and is isolated from production.

## Pipeline caching (#195)

The baseline reuses the original lexer/factory but tokenizes and composes on every
call, then performs the original result extraction. The optimized case calls the
public parser with a warm cache. Both parse `2025-06` using `yyyy-MM`, extract year
and month, and box the returned tuple into the same result sink.

Measured on Windows 10.0.26200 x64, .NET 10.0.12, Release:

| Case | Before ns/op | After ns/op | Time change | Before B/op | After B/op | Allocation change |
|---|---:|---:|---:|---:|---:|---:|
| Repeated yyyy-MM Parse | 407.20 | 223.97 | -45.0% | 2016 | 656 | -67.5% |

These measurements were taken before the numeric primitive optimization, isolating
pipeline caching. The final harness also runs against optimized numeric primitives.

The shared cache retains at most 256 entries with FIFO eviction and skips retention
for formats longer than 1024 characters. Keys contain the ordinal format string
and current UI culture name: localized tokens use `CurrentUICulture`; month names
are invariant and the provider argument currently does not affect construction.
Failures are not cached. Cache access/construction is synchronized; Pidgin parsing
runs outside the lock with per-call results. Dynamic result types are resolved
after parsing and do not change the composed pipeline.

## Numeric primitives (#196)

The numeric baselines are exact copies of the original methods from `50c82ad`, in
`src/Chrononuensis.Core.Testing/Parsers/Internals/OriginalNumericPrimitives.cs`.
The benchmark links that validation-only file; production never references it.
Both sides precompose parsers outside timing and use identical inputs and sinks.
The remaining 56 B/op includes Pidgin/result-sink overhead, not digit conversion.

Measured on the same Windows x64 / .NET 10.0.12 environment:

| Case (input) | Before ns/op | After ns/op | Time change | Before B/op | After B/op | Allocation change |
|---|---:|---:|---:|---:|---:|---:|
| Fixed two (`06`) | 78.20 | 40.43 | -48.3% | 152 | 56 | -63.2% |
| Fixed four (`2025`) | 104.95 | 54.85 | -47.7% | 152 | 56 | -63.2% |
| Variable one/two (`6`) | 216.80 | 182.13 | -16.0% | 80 | 56 | -30.0% |
| Variable one/two (`12`) | 76.11 | 58.11 | -23.6% | 88 | 56 | -36.4% |
| Variable one/three (`365`) | 149.80 | 115.21 | -23.1% | 120 | 56 | -53.3% |

With numeric optimization applied to both sides, the pipeline case measured
389.15 -> 189.28 ns/op (-51.4%), 1832 -> 592 B/op (-67.7%). This isolates caching
under the final numeric implementation; it is not an original-to-final comparison.

[Pidgin 3.5.1's `Digit`](https://github.com/benjamin-hodgson/Pidgin/blob/v3.5.1/Pidgin/Parser.Char.cs)
uses `char.IsDigit`, accepting non-ASCII decimal digits. The original `int.Parse`
then throws for a complete non-ASCII number. The optimized implementation preserves
that exception, its message, and conversion timing via an exceptional stack-span
fallback. Successful ASCII parsing accumulates integers without digit strings or
arrays. Characters are temporarily packed into a `ulong` so conversion still occurs
only after the complete primitive parses. New differential tests compare values,
full Pidgin errors and conversion exceptions for all ten primitive variants,
including every UTF-16 digit character, truncated inputs, string/span paths,
bounds, normalization and three current cultures. Existing test files are untouched.

## Validation

- Focused parser suite on .NET 10 before numeric changes: 211 passed.
- New numeric differential suite on .NET 10: 13 passed.
- `dotnet build Chrononuensis.sln -c Release -p:NuGetAudit=false`: succeeded,
  all supported targets, zero warnings/errors.
- `dotnet test Chrononuensis.sln -c Release --no-build --no-restore`: 846 core
  tests plus 7 source-generator tests passed on each of .NET 8, 9, and 10
  (2,559 passed total, none failed/skipped).
- Release benchmark above executed on .NET 10 only; performance was not measured
  on .NET 8/9 or Linux. No Debug-configuration or Linux test runs were performed.
- No pre-existing test files or assertions were changed.
