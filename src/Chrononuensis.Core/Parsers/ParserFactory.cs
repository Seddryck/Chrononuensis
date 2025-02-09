using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Formats;
using Tokens = Chrononuensis.Formats.Tokens;
using Pidgin;
using Chrononuensis.Formats.Tokens;
using Chrononuensis.Parsers.Internals;

namespace Chrononuensis.Parsers;
internal partial class ParserFactory
{
    public Dictionary<FormatToken, Parser<char, object>> _dict { get; set; } = [];

    partial void Initialize();

    public ParserFactory()
        => Initialize();

    protected void AddMapping(FormatToken token, Parser<char, object> parser)
    {
        _dict.TryAdd(token, parser);
    }

    public Parser<char, object> Create(FormatToken token)
    {
        if (token is LiteralToken literal)
            return Primitives.CharParser(literal.Value).Cast<object>();

        if (!_dict.TryGetValue(token, out var parser))
            throw new ArgumentOutOfRangeException($"Token {token} not found in the dictionary");
        return parser;
    }
}
