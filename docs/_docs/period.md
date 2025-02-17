---
title: Periods
tags: [quick-start]
---
A period refers to a continuous time span with a well-defined start and end. It represents a range rather than a single event and serves as a frame of reference by providing structure.

Examples:

- `Q1 2025`: January 1 – March 31, 2025
- `FY2025`: April 1, 2024 – March 31, 2025
- `Week 5 of 2025`: January 29 – February 4, 2025

In Chrononuensis, a period is represented by the interface `IPeriod`.

## Types implementing IPeriod

The following types implement the `IPeriod` interface:

- Century
- Decade
- Year
- YearSemester
- YearQuarter
- YearWeek
- YearDay

Each of these types provides a structured way to represent specific time intervals.

## Properties

All types implementing `IPeriod` expose the following properties:

### Days

Returns the total number of days within the period.

```csharp
var yearQuarter = YearQuarter.Parse("2025-Q1");
Assert.That(yearQuarter.Days, Is.EqualTo(31 + 28 + 31));
```

In this example, the first quarter of 2025 spans 31 days in January, 28 days in February, and 31 days in March (equal to 90 days).

### FirstDate

Returns the first date of the period.

```csharp
var yearQuarter = YearQuarter.Parse("2025-Q1");
Assert.That(yearQuarter.FirstDate, Is.EqualTo(new DateOnly(2025, 1, 1)));
```

The first date of Q1 2025 is January 1, 2025.

### LastDate

Returns the last date of the period.

```csharp
var yearQuarter = YearQuarter.Parse("2025-Q1");
Assert.That(yearQuarter.LastDate, Is.EqualTo(new DateOnly(2025, 3, 31)));
```

The last date of Q1 2025 is March 31, 2025.

### LowerBound

Represents the lower bound of the period, defined as a close/open interval, typically at the start of `FirstDate`

```csharp
var yearQuarter = YearQuarter.Parse("2025-Q1");
Assert.That(yearQuarter.LowerBound, Is.EqualTo(new DateTime(2025, 1, 1, 0, 0, 0)));
```

The lower bound of Q1 2025 starts on January 1, 2025, at midnight.

### UpperBound

Represents the upper bound of the period, defined as a close/open interval, typically at the start of the day following `LastDate`.

```csharp
var yearQuarter = YearQuarter.Parse("2025-Q1");
Assert.That(yearQuarter.UpperBound, Is.EqualTo(new DateTime(2025, 4, 1, 0, 0, 0)));
```

The upper bound of Q1 2025 is April 1, 2025, at midnight, meaning the period runs from January 1, 2025 (inclusive) to April 1, 2025 (exclusive).
