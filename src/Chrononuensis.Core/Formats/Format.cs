using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Formats.Tokens;

namespace Chrononuensis.Formats;
internal class Format : IEnumerable<FormatToken>
{
    private readonly List<FormatToken> _tokens = [];

    public Format(FormatToken[] tokens)
        => _tokens = [.. tokens];

    public IEnumerator<FormatToken> GetEnumerator()
        => _tokens.GetEnumerator();
    public int GetIndex<T>() where T : IPeriodToken
        => GetIndex(typeof(T));

    public int GetIndex(Type t)
        => _tokens.FindIndex(x => t.IsAssignableFrom(x.GetType()));

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
