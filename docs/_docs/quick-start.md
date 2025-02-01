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

Instead of manually creating a struct, you can parse a formatted string to obtain a struct.

If you have a formatted string "2025-08" and want to convert it into a YearMonth struct, use the Parse method:

```csharp
// Parse a period Year-Month for August 2025
var other = YearMonth.Parse("2025-08", "yyyy-MM");
```

In this example,

* `2025-08` is the input string.
* `yyyy-MM` is the format that defines how the input should be interpreted.
* The parser extracts year and month from the string and returns a YearMonth struct.

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
            // Parse a period Year-Month for August 2025
            var yearMonth = new YearMonth(2025, 7); 

            // Parse a period Year-Month for August 2025
            var other = YearMonth.Parse("2025-08", "yyyy-MM"); 
            
            //The struct handling Year-Month supports basic comparison
            Console.Writeline(yearMonth <= other); // Display True
        }
    }
}
```

## Use the parsers

Parsers in Chrononuensis are flexible tools that extract structured information from input strings. Their role is **not** to instantiate predefined structs or classes, but rather to **return extracted components (as integers)** so that **you** can use them to create your own structures.

### What Parsers Do

✅ **Parse an input string** based on a specified format.  
✅ **Consider culture settings** when interpreting numbers or dates.  
✅ **Extract individual components as integers** (e.g., Year, Month, Day).  
❌ **Do not enforce a specific object structure** – that’s up to you.

### How Parsers Work

Instead of returning a predefined struct, a parser simply extracts and returns individual components.  
For example, given an input string like `"2025-08"`:

```csharp
var parser = new YearMonthParser();
(int year, int month) = parser.Parse("2025-08", "yyyy-MM");
```
