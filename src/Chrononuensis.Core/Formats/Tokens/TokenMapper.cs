using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Formats.Tokens.Month;
using Chrononuensis.Formats.Tokens.Quarter;
using Chrononuensis.Formats.Tokens.Year;

namespace Chrononuensis.Formats.Tokens;
internal class TokenMapper
{
    private readonly Dictionary<string, FormatToken> _tokenMap = new();
    public TokenMapper()
    {
        _tokenMap.Add("yy", DigitOn2YearToken.Instance);
        _tokenMap.Add("yyyy", DigitOn4YearToken.Instance);

        _tokenMap.Add("M", DigitMonthToken.Instance);
        _tokenMap.Add("MM", PaddedDigitMonthToken.Instance);
        _tokenMap.Add("MMM", AbbreviationMonthToken.Instance);
        _tokenMap.Add("MMMM", LabelMonthToken.Instance);

        _tokenMap.Add("q", DigitQuarterToken.Instance);
    }

    public FormatToken GetToken(string token)
    {
        if (_tokenMap.ContainsKey(token))
            return _tokenMap[token];
        throw new ArgumentException($"Token {token} not found");
    }
}
