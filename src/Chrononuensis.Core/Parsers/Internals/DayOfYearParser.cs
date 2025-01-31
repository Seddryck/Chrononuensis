using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pidgin;

namespace Chrononuensis.Parsers.Internals;
internal class DayOfYearParser
{
    public static Parser<char, int> Digit { get; } = Primitives.OneToThreeDigitParser(1, 366);
    public static Parser<char, int> PaddedDigit { get; } = Primitives.ThreeDigitParser(1, 366);
}
