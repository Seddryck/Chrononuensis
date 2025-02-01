using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pidgin;

namespace Chrononuensis.Parsers.Internals;
internal class QuarterParser
{
    public static Parser<char, int> Digit { get; } = Primitives.OneDigitParser(1,4);
}

