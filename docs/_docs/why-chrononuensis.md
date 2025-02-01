---
title: Why Was Chrononemesis Created?
tags: [quick-start]
---

## The Problem: Parsing Date Components in a Flexible Way

Many .NET solutions require **date parsing**, but existing solutions often enforce a **fixed structure** or rely on `DateTime`, which:

- **Doesn’t support partial dates** (e.g., just a **month and day**, or just a **year and week**).
- Requires **full timestamps**, even when only a subset of the date is needed.
- Offers **limited flexibility** in defining custom formats.

Chrononemesis was created to solve these issues by allowing **precise, component-based parsing**.

## The Solution: Component-Based Parsing

Instead of forcing a full `DateTime` structure, **Chrononemesis allows parsing only the needed components**.  

For example:

- **Extract just a Year and Month (`yyyy-MM`)** without needing a full timestamp.
- **Parse a Week-based format (`yyyy-'W'ww`)** without converting to `DateTime`.
- **Use a custom structure** (e.g., `YearQuarter`) instead of adapting to `DateTime`'s limitations.

## Decoupling Parsers and Structures

One of the key design principles of Chrononemesis is the **separation of concerns** between **parsers** and **structures**.

**Parsers** handle the **extraction of data** (e.g., getting `2025` from `"2025-08"`) where **Structures** define how the extracted values are stored and used.

This decoupling means that:

- **Parsers don’t enforce a specific structure** – they only return the extracted values.
- **Users are free to define their own structs/classes** to store the parsed components.
- **Parsing logic remains independent** from data representation, making the library more **flexible**.
