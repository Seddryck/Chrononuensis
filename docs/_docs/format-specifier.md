---
title: Format specifiers
tags: [quick-start]
---

XChrononuensis supports various **tokens** that allow parsing date and time components from formatted strings. Below is a list of all supported tokens, grouped by category.

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
