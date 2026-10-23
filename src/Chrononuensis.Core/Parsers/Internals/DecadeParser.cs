using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pidgin;

namespace Chrononuensis.Parsers.Internals;
internal class DecadeParser
{
    private static int NormalizeDecade(int decade) => decade < 40 ? decade + 2000 : decade + 1900;

    public static Parser<char, int> DigitOn2 { get; } = CreateDigitOn2(NormalizeDecade);
    public static Parser<char, int> DigitOn4 { get; } = Primitives.ThreeDigitThenZeroParser();

    public static Parser<char, int> CreateDigitOn2(Func<int, int> normalizeDecade)
        => Primitives.OneDigitThenZeroParser(normalizeDecade);
}

