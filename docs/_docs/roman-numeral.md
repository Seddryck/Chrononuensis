---
title: Roman Numerals
tags: [advanced]
---

Chrononuensis supports parsing certain tokens expressed as **Roman numerals** instead of standard Arabic numerals.

## Specifying a Roman Numeral Format

o specify that a token should be expressed as a Roman numeral, enclose the format specifier in curly braces `{}` and append the `RN` suffix.

For example, the format specifier for a month expressed as a Roman numeral is `{M:RN}`.

## Behavior of Roman Numerals in Formatting

Roman numerals do not have a concept of zero, so padding is not applicable. This means that both `{M:RN}` and `{MM:RN}` produce the same output.

For years, Chrononuensis maintains normalization for 2-digit and 4-digit values when using numeric representations:

- `{yy:RN}` expects a 2-digit Roman numeral, such as `LXXVIII`, which is interpreted as `78` and normalized to `1978`.
- `{yyyy:RN}` expects a 4-digit Roman numeral, such as `MCMLXXVIII`, which corresponds directly to `1978`.

### Usage Example

```csharp
var text = "December MCMLXXVIII"; //December 1978
var format = "MMMM {yyyy:RN}" // Full month and year with 4 digits in roman numerals

var yearMonth = YearMonth.Parse(text, format, CultureInfo.InvariantCulture);
Assert.That(yearMonth.Year, Is.EqualTo(1978));
Assert.That(yearMonth.Month, Is.EqualTo(12));
```
