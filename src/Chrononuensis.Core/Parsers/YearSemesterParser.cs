using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Formats.Tokens;

namespace Chrononuensis.Parsers;
internal class YearSemesterParser : ChrononuensisParser
{
    public static string DefaultPattern { get; } = "yyyy-'H'S";

    public (int Year, int Semester) Parse(string input, string format, IFormatProvider? provider)
    {
        var results = ParseInternal(input, format, provider, [typeof(IYearToken), typeof(ISemesterToken)]);
        return ((int)results[0], (int)results[1]);
    }
}
