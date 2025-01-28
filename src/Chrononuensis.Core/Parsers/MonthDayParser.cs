using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Formats.Tokens;

namespace Chrononuensis.Parsers;
internal class MonthDayParser : ChrononuensisParser
{
    public static string DefaultPattern { get; } = "MM-dd";

    public (int Month, int Day) Parse(string input, string format, IFormatProvider? provider)
    {
        var results = ParseInternal(input, format, provider, [typeof(IMonthToken), typeof(IDayToken)]);
        return ((int)results[0], (int)results[1]);
    }
}
