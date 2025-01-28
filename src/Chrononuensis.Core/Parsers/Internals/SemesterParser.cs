using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pidgin;

namespace Chrononuensis.Parsers.Internals;
internal class SemesterParser
{
    public static Parser<char, int> OneDigit { get; } = Primitives.OneDigitParser(1,4);
}

