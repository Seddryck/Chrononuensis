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
