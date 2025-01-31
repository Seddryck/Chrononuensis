using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Formats.Tokens.Day;
using Chrononuensis.Formats.Tokens.DayOfYear;
using Chrononuensis.Formats.Tokens.Month;
using Chrononuensis.Formats.Tokens.Quarter;
using Chrononuensis.Formats.Tokens.Semester;
using Chrononuensis.Formats.Tokens.Year;

namespace Chrononuensis.Formats.Tokens;
internal partial class TokenMapper
{
    private readonly Dictionary<string, FormatToken> _tokenMap = new();

    partial void Initialize();

    public TokenMapper()
        => Initialize();

    public FormatToken GetToken(string token)
    {
        if (_tokenMap.ContainsKey(token))
            return _tokenMap[token];
        throw new ArgumentException($"Token {token} not found");
    }
}
