﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pidgin;

namespace Chrononuensis.Parsers.Internals;
internal class CenturyParser
{
    public static Parser<char, int> Digit { get; } = Primitives.OneOrTwoDigitParser(1,99);
    public static Parser<char, int> PaddedDigit { get; } = Primitives.TwoDigitParser(1,99);
    public static Parser<char, int> RomanNumeral { get; }
        = Primitives.RomanNumeral(1, 99);
}
