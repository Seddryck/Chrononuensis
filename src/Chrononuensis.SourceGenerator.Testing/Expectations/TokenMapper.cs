using Chrononuensis.Formats.Tokens.Day;
using Chrononuensis.Formats.Tokens.DayOfYear;

namespace Chrononuensis.Formats.Tokens;

internal partial class TokenMapper
{
    partial void Initialize()
    {
        _tokenMap.Add("d", DigitDayToken.Instance);
        _tokenMap.Add("dd", PaddedDigitDayToken.Instance);
        
        _tokenMap.Add("j", DigitDayOfYearToken.Instance);
        _tokenMap.Add("jjj", PaddedDigitDayOfYearToken.Instance);
    }
}
