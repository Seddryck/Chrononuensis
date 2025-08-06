using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Formats.Tokens;
using Chrononuensis.Formats.Tokens.Month;

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

        while (i < inputSpan.Length)
        {
            var currentChar = inputSpan[i];

            if (currentChar == '\'' || currentChar == '\"')
                i = HandleQuote(inputSpan, tokens, i, currentChar);
            else if (SymbolChars.Contains(currentChar))
                i = HandleSymbol(inputSpan, token, tokens, ref j, i, currentChar);
            else if (currentChar == '{')
                i = HandleBrace(inputSpan, token, tokens, ref j, i);
            else if (currentChar == '[')
                i = HandlePlaceholder(inputSpan, token, tokens, ref j, i);
            else
                tokens.Add(new LiteralToken(currentChar));
            i++;
        }

        return new Format([.. tokens]);
    }

    private void AppendToken(Span<char> token, List<FormatToken> tokens, ref int j)
    {
        if (j > 0)
        {
            if (token[0] == '#')
            {
                var value = new string(token.Slice(1, j - 1).ToArray());
                tokens.Add(new LocalizedToken(value));
            }
            else
            {
                var value = new string(token.Slice(0, j).ToArray());
                tokens.Add(_mapper.GetToken(value));
            }
            j = 0;
        }
    }

    private int HandleQuote(ReadOnlySpan<char> inputSpan, List<FormatToken> tokens, int i, char quoteChar)
    {
        i++;
        var tempToken = new StringBuilder();
        while (i < inputSpan.Length && inputSpan[i] != quoteChar)
            tempToken.Append(inputSpan[i++]);
        tokens.Add(new LiteralToken(tempToken.ToString()));
        return i;
    }

    private int HandleSymbol(ReadOnlySpan<char> inputSpan, Span<char> token, List<FormatToken> tokens, ref int j, int i, char currentChar)
    {
        while (i < inputSpan.Length && inputSpan[i] == currentChar)
        {
            token[j++] = currentChar;
            i++;
        }
        i--;
        AppendToken(token, tokens, ref j);
        return i;
    }

    private int HandleBrace(ReadOnlySpan<char> inputSpan, Span<char> token, List<FormatToken> tokens, ref int j, int i)
    {
        i++;
        while (i < inputSpan.Length && inputSpan[i] != '}')
        {
            token[j++] = inputSpan[i];
            i++;
        }
        AppendToken(token, tokens, ref j);
        return i;
    }

    private int HandlePlaceholder(ReadOnlySpan<char> inputSpan, Span<char> token, List<FormatToken> tokens, ref int j, int i)
    {
        i++;
        var tempToken = new StringBuilder();
        var exclusiveTokens = new List<FormatToken>();

        while (i < inputSpan.Length && inputSpan[i] != ']')
        {
            var currentChar = inputSpan[i];

            if (currentChar == '\'' || currentChar == '\"')
                i = HandleQuote(inputSpan, exclusiveTokens, i, currentChar);
            else if (currentChar == '|' && tempToken.Length > 0)
            {
                exclusiveTokens.Add(new LiteralToken(tempToken.ToString()));
                tempToken.Clear();
            }
            else if (currentChar != '|' && currentChar != ' ')
                tempToken.Append(currentChar);
            i++;
        }
        if (tempToken.Length > 0)
        {
            exclusiveTokens.Add(new LiteralToken(tempToken.ToString()));
            tempToken.Clear();
        }

        tokens.Add(new MutuallyExclusiveToken([.. exclusiveTokens]));
        return i;
    }
}
