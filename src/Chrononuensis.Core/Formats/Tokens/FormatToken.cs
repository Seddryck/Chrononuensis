using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrononuensis.Formats.Tokens;
internal abstract class FormatToken
{ }

internal abstract class FormatToken<T> : FormatToken where T : FormatToken<T>, new()
{
    private static readonly T _instance = new T();
    public static T Instance => _instance;
    protected FormatToken() { }
}
