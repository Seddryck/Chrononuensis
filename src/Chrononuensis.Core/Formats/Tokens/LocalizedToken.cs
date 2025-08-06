using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrononuensis.Formats.Tokens;
internal class LocalizedToken : FormatToken
{
    public string Key { get; }

    public LocalizedToken(string key)
        => Key = key;
}
