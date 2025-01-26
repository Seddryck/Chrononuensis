using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Formats.Tokens;

namespace Chrononuensis.Parsers;
internal class YearQuarterParser : ChrononuensisParser
{
    public static string DefaultPattern { get; } = "yyyy-Qq";

    public (int Year, int Quarter) Parse(string input, string format, IFormatProvider? provider)
    {
        var results = ParseInternal(input, format, provider, [typeof(IYearToken), typeof(IQuarterToken)]);
        return ((int)results[0], (int)results[1]);
    }
}
