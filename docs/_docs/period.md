---
title: Periods
tags: [quick-start]
---
A period refers to a continuous time span with a well-defined start and end. It represents a range rather than a single event and serves as a frame of reference by providing structure.

Examples:

- `Q1 2025`: January 1 – March 31, 2025
- `FY2025`: April 1, 2024 – March 31, 2025
- `Week 5 of 2025`: January 29 – February 4, 2025

In Chrononuensis, a period is represented by the interface `IPeriod`. Its canonical
date-domain representation is a non-empty half-open range:

```text
[StartDate, EndDateExclusive)
```

The start belongs to the period and the exclusive end does not. Empty periods are
not valid, so `EndDateExclusive` must be after `StartDate`. This representation does
not require a time zone or a conversion to `DateTime`.

## Types implementing IPeriod

The following types implement the `IPeriod` interface:

- Century
- Decade
- Year
- YearSemester
- YearQuarter
- YearWeek
- YearDay
- CustomPeriod

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

### StartDate

Returns the inclusive start date of the period.

```csharp
var yearQuarter = YearQuarter.Parse("2025-Q1");
Assert.That(yearQuarter.StartDate, Is.EqualTo(new DateOnly(2025, 1, 1)));
```

The first date of Q1 2025 is January 1, 2025.

### EndDateExclusive

Returns the first date after the period.

```csharp
var yearQuarter = YearQuarter.Parse("2025-Q1");
Assert.That(yearQuarter.EndDateExclusive, Is.EqualTo(new DateOnly(2025, 4, 1)));
```

Q1 2025 includes dates up to March 31 and excludes April 1.

## Custom periods

`CustomPeriod` uses the same canonical constructor contract:

```csharp
var january = new CustomPeriod(
    new DateOnly(2025, 1, 1),
    new DateOnly(2025, 2, 1));
```

The constructor rejects equal or reversed bounds. To migrate code that supplies an
inclusive final date, use `CustomPeriod.FromInclusiveDates(firstDate, lastDate)`.
An inclusive `DateOnly.MaxValue` cannot be converted because `DateOnly` has no value
for the following exclusive date. A canonical period may use `DateOnly.MaxValue` as
its exclusive end, making `DateOnly.MaxValue.AddDays(-1)` its latest included date.

## Compatibility properties

`FirstDate` and `LastDate` remain available as inclusive display values derived from
the canonical bounds. `LowerBound` and `UpperBound` also remain available as derived
midnight `DateTime` values. New date-only code should use `StartDate` and
`EndDateExclusive` directly.

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

## Equality

All supported `IPeriod` implementations compare by their resolved canonical bounds.
For example, a `YearMonth` and a `CustomPeriod` representing the same half-open range
are equal and have the same hash code, regardless of their concrete types.
