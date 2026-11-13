---
title: Quick Start Guide
tags: [quick-start]
---

This guide helps you quickly run **Chrononuensis** without covering installation steps. If you haven't installed it yet, refer to the [Installation Guide](installation.md).

Open the Program.cs file and add the necessary using directive:

```csharp
using Chrononuensis;
```

## Use the structures

Structs in Chrononuensis are lightweight data structures designed to hold parsed data. They are optimized for performance and memory efficiency. They are immutable where possible, ensuring data integrity.

### Manually create a struct by passing values

You can create a struct directly by passing its required values to the constructor.

Chrononuensis follows a consistent parameter ordering for struct constructors and parsers. This order follows a decreasing specificity pattern, similar to how `DateTime` works in .NET. i.e. for the `YearMonth` struct, `Year` comes before `Month`, following a logical decreasing order.

```csharp
// Create a Year-Month period for July 2025
var yearMonth = new YearMonth(2025, 7);
```

### Parsing from a string

The static `YearMonth.Parse` method reads the year and month from a formatted string and constructs a `YearMonth`.

```csharp
YearMonth other = YearMonth.Parse("2025-08", "yyyy-MM");
```

In this example,

* `2025-08` is the input string.
* `yyyy-MM` is the format that defines how the input should be interpreted.

### Complete example

```csharp
using System;
using Chrononuensis;

namespace MyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var yearMonth = new YearMonth(2025, 7);
            YearMonth other = YearMonth.Parse("2025-08", "yyyy-MM");

            Console.WriteLine(yearMonth <= other); // True
        }
    }
}
```

## Low-level parsers

For most uses, call `Parse` on the struct you want, such as `YearMonth.Parse`. Parser classes are low-level APIs intended mainly for extensibility and for cases where you need the individual components, such as constructing your own type.

`YearMonthParser.Parse` returns an `(int Year, int Month)` tuple. Pass a format provider to control culture-sensitive parsing.

```csharp
using Chrononuensis.Parsers;

var parser = new YearMonthParser();
(int year, int month) = parser.Parse("2025-08", "yyyy-MM", provider: null);
```
