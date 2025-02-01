# Chrononuensis

![Logo](https://raw.githubusercontent.com/Seddryck/Chrononuensis/main/assets/chrononuensis-icon-128.png)

Transform your structured YAML, JSON, XML, CSV or FrontMatter data into beautiful, fully-customized HTML pages or plain text in seconds with Chrononuensis. This command-line tool seamlessly generates renders from data files using your preferred templates through Scriban, Handlebars, DotLiquid, Fluid, StringTemplate or SmartFormat. Whether you're building static sites, documentation, or reporting tools, Chrononuensis makes it easy to turn raw data into polished, web-ready content.

[About][] | [Installing][] | [Quickstart][]

[About]: #about (About)
[Installing]: #installing (Installing)
[Quickstart]: #quickstart (Quickstart)

## About

**Social media:** [![website](https://img.shields.io/badge/website-seddryck.github.io/Chrononuensis-fe762d.svg)](https://seddryck.github.io/Chrononuensis)
[![twitter badge](https://img.shields.io/badge/twitter%20Chrononuensis-@Seddryck-blue.svg?style=flat&logo=twitter)](https://twitter.com/Seddryck)

**Releases:** [![GitHub releases](https://img.shields.io/github/v/release/seddryck/chrononuensis?label=GitHub%20releases)](https://github.com/seddryck/chrononuensis/releases/latest) 
[![nuget](https://img.shields.io/nuget/v/Chrononuensis.svg)](https://www.nuget.org/packages/Chrononuensis/) [![GitHub Release Date](https://img.shields.io/github/release-date/seddryck/Chrononuensis.svg)](https://github.com/Seddryck/Chrononuensis/releases/latest) [![licence badge](https://img.shields.io/badge/License-Apache%202.0-yellow.svg)](https://github.com/Seddryck/Chrononuensis/blob/master/LICENSE) 

**Dev. activity:** [![GitHub last commit](https://img.shields.io/github/last-commit/Seddryck/Chrononuensis.svg)](https://github.com/Seddryck/Chrononuensis/commits)
![Still maintained](https://img.shields.io/maintenance/yes/2025.svg)
![GitHub commit activity](https://img.shields.io/github/commit-activity/y/Seddryck/Chrononuensis)

**Continuous integration builds:** [![Build status](https://ci.appveyor.com/api/projects/status/omt58fj96enn9p34?svg=true)](https://ci.appveyor.com/project/Seddryck/Chrononuensis/)
[![Tests](https://img.shields.io/appveyor/tests/seddryck/Chrononuensis.svg)](https://ci.appveyor.com/project/Seddryck/Chrononuensis/build/tests)
[![CodeFactor](https://www.codefactor.io/repository/github/seddryck/Chrononuensis/badge)](https://www.codefactor.io/repository/github/seddryck/Chrononuensis)
[![codecov](https://codecov.io/github/Seddryck/Chrononuensis/branch/main/graph/badge.svg?token=ZXR5A0QJXF)](https://codecov.io/github/Seddryck/Chrononuensis)
<!-- [![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FSeddryck%2FChrononuensis.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2FSeddryck%2FChrononuensis?ref=badge_shield) -->

**Status:** [![stars badge](https://img.shields.io/github/stars/Seddryck/Chrononuensis.svg)](https://github.com/Seddryck/Chrononuensis/stargazers)
[![Bugs badge](https://img.shields.io/github/issues/Seddryck/Chrononuensis/bug.svg?color=red&label=Bugs)](https://github.com/Seddryck/Chrononuensis/issues?utf8=%E2%9C%93&q=is:issue+is:open+label:bug+)
[![Top language](https://img.shields.io/github/languages/top/seddryck/Chrononuensis.svg)](https://github.com/Seddryck/Chrononuensis/search?l=C%23)

## Installing

### Using .NET CLI

Creating a new reference using .NET CLI
If you’re using the .NET command-line interface (CLI), open a terminal in your project’s root directory and run:

```sh
dotnet add package Chrononuensis
```
This will:

- Add the Chrononuensis package to your project.
- Update the csproj file to include it as a dependency.

[More info](https://seddryck.github.io/Chrononuensis/docs/installation/)

## Quick-start

### Use the structures
Structs in Chrononuensis are lightweight data structures designed to hold parsed data. They are optimized for performance and memory efficiency. They are immutable where possible, ensuring data integrity.

#### Manually create a struct by passing values

You can create a struct directly by passing its required values to the constructor.

Chrononuensis follows a consistent parameter ordering for struct constructors and parsers. This order follows a decreasing specificity pattern, similar to how DateTime works in .NET. i.e. for the YearMonth struct, Year comes before Month, following a logical decreasing order.

```csharp
// Create a Year-Month period for July 2025
var yearMonth = new YearMonth(2025, 7);
```

#### Parsing from a string

Instead of manually creating a struct, you can parse a formatted string to obtain a struct.

If you have a formatted string “2025-08” and want to convert it into a YearMonth struct, use the Parse method:

```csharp
// Parse a period Year-Month for August 2025
var other = YearMonth.Parse("2025-08", "yyyy-MM");
```

In this example,

- `2025-08` is the input string.
- `yyyy-MM` is the format that defines how the input should be interpreted.

The parser extracts year and month from the string and returns a YearMonth struct.

### Use the parsers

Parsers in Chrononuensis are flexible tools that extract structured information from input strings. Their role is not to instantiate predefined structs or classes, but rather to return extracted components (as integers) so that you can use them to create your own structures.

Instead of returning a predefined struct, a parser simply extracts and returns individual components.
For example, given an input string like "2025-08":

```csharp
var parser = new YearMonthParser();
(int year, int month) = parser.Parse("2025-08", "yyyy-MM");
```

[More info](https://seddryck.github.io/Chrononuensis/docs/quick-start/)

### List of supported structures and parsers

Chrononuensis provides a set of predefined structures for parsing and representing date-based information.

- MonthDay
- YearDay
- YearWeek
- YearMonth
- YearQuarter
- YearSemester

[More info](https://seddryck.github.io/Chrononuensis/docs/structures/)

### List of supported format specifiers

- Year: yy and yyyy
- Semester: S
- Quarter: q
- Month: M, MM, MMM, MMMM
- Week: w, ww
- Day of Year: j and jjj
- Day: d, dd

[More info](https://seddryck.github.io/Chrononuensis/docs/format-specifier/)
