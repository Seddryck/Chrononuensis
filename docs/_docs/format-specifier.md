---
title: Format Specifiers
tags: [quick-start]
---

Chrononuensis supports various **tokens** that allow parsing date and time components from formatted strings.

## Specifying a Format

By default, a character displayed in the format is interpreted as a format specifier unless it is not part of the list of format specifiers. This allows for intuitive formats such as `yyyy-MM`, which are easily readable.

If a character is part of the format specifiers but needs to be treated as a literal, enclose it in either single `'` or double quotes `"`. For example, in `'year:'yyyy`, the  `y` in *year* is interpreted as a literal since it is enclosed in quotes. If your format includes a single quote character, enclose the entire string in double quotes, and vice versa, to avoid ambiguity.

For complex format specifiers, such as [Roman numerals](docs/roman-numeral), enclose the format specifiers in curly braces and separate different parts using colons (`:`).

### **Examples:**

The following formats specify a four-digit year and a two-digit, zero-padded month:

| Format | Simple | Quoted | Curly |
|--------|--------|--------|--------|
| Standard numeric representation | `yyyy-MM` | `yyyy'-'MM` | `{yyyy}'-'{MM}` |
| Roman numeral for month |  |  | `{yyyy}-{MM:RN}` |
| Roman numeral for year |  |  | `{yyyy:RN}-{MM}` |

## List of Tokens

Below is a list of all supported tokens, grouped by category.

{% for group in site.data.tokens %}

## {{ group.group }}

<table>
  <thead>
    <tr>
      <th>Token Name</th>
      <th>Pattern</th>
      <th>Description</th>
    </tr>
  </thead>
  <tbody>

  {% for member in group.members %}
    <tr>
      <td><strong>{{ member.name }}</strong></td>
      <td><code>{{ member.pattern }}</code></td>
      <td>Extracts the {{ group.group | downcase }} component.</td>
    </tr>
    {% endfor %}
  </tbody>
</table>
{% endfor %}

## **Usage Example**

You can use these tokens with a parser to extract specific date components:

```csharp
var parser = new YearMonthParser();
(int year, int month) = parser.Parse("2025-08", "yyyy-MM");

Console.WriteLine($"Year: {year}, Month: {month}");
```

Parsing a Day of Year Format

```csharp
var parser = new YearDatParser();
(int year, int dayOfYear) = parser.Parse("2025-256", "yyyy-jjj");

Console.WriteLine($"Year: {year}, Day of Year: {dayOfYear}");
```
