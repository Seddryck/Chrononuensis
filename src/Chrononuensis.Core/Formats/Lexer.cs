using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Formats.Tokens;

namespace Chrononuensis.Formats;
internal partial class Lexer
{
    private readonly TokenMapper _mapper = new();
    private static readonly char[] SymbolChars = InitializeSymbolChars();
    private static partial char[] InitializeSymbolChars();

    public Format Tokenize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input cannot be null or empty.", nameof(input));

        var inputSpan = input.AsSpan();
        var token = new Span<char>(new char[inputSpan.Length]);
        var tokens = new List<FormatToken>();
        var j = 0;
        var i = 0;

        void appendToken(Span<char> token)
        {
            if (j > 0)
            {
                var value = new string(token.Slice(0, j).ToArray());
                tokens.Add(_mapper.GetToken(value));
                j = 0;
            }
        }


        while (i < inputSpan.Length)
        {
            var currentChar = inputSpan[i];

            if (currentChar == '\'')
            {
                i++;
                while (i < inputSpan.Length && inputSpan[i] != '\'')
                    tokens.Add(new LiteralToken(inputSpan[i++]));
            }
            else if (SymbolChars.Contains(currentChar))
            {
                while (i < inputSpan.Length && inputSpan[i] == currentChar)
                {
                    token[j++] = currentChar;
                    i++;
                }
                i--;
                appendToken(token);
            }
            else
                tokens.Add(new LiteralToken(currentChar));
            i++;
        }

        return new Format([.. tokens]);
    }
}
