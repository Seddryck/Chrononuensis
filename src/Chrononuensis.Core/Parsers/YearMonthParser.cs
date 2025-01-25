using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Formats;
using Chrononuensis.Formats.Tokens;
using Chrononuensis.Parsers.Internals;
using Pidgin;

namespace Chrononuensis.Parsers;
internal class YearMonthParser : IParser
{
    public static string DefaultPattern { get; } = "yyyy-MM";

    public (int Year, int Month) Parse(string input, string format, IFormatProvider? provider)
    {
        var tokens = new Lexer().Tokenize(format);

        var factory = new ParserFactory();
        var parsers = tokens.Select(factory.Create).ToArray();

        var parser = Primitives.CombineParsers(parsers);
        var result = parser.Parse(input);

        if (result.Success == false && result.Error is not null)
        {
            var ex = new FormatExceptionFactory().Create(result);
            throw ex;
        }

        var year = (int)result.Value[tokens.GetIndex<IYearToken>()];
        var month = (int)result.Value[tokens.GetIndex<IMonthToken>()];
        return (year, month);
    }
}
