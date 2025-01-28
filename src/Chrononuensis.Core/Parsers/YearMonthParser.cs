using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Formats.Tokens;

namespace Chrononuensis.Parsers;
internal class YearMonthParser : ChrononuensisParser
{
    public static string DefaultPattern { get; } = "yyyy-MM";

    public (int Year, int Month) Parse(string input, string format, IFormatProvider? provider)
    {
        var results = ParseInternal(input, format, provider, [typeof(IYearToken), typeof(IMonthToken)]);
        return ((int)results[0], (int)results[1]);
    }
}
