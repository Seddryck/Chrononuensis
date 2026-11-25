using Chrononuensis.Formats;
using Pidgin;

namespace Chrononuensis.Parsers;

internal sealed class TypedPeriodParser<TResult> where TResult : struct
{
    internal readonly record struct Component(Type TokenType, Func<TResult, int, TResult> SetValue);

    private readonly Component[] components;
    private readonly Func<Format, IFormatProvider?, Parser<char, TResult>> compose;

    internal TypedPeriodParser(Component[] components)
    {
        this.components = components;
        compose = Compose;
    }

    internal TResult Parse(string input, string format, IFormatProvider? provider)
        => GetValue(GetPipeline(format, provider).Parse(input));

    internal TResult Parse(ReadOnlySpan<char> input, string format, IFormatProvider? provider)
        => GetValue(GetPipeline(format, provider).Parse(input));

    internal TResult Parse(ReadOnlySpan<byte> input, string format, IFormatProvider? provider)
        => Utf8PeriodInput.Parse(input, (Parser: this, format, provider), throwOnInvalidUtf8: true,
            static (text, state) => state.Parser.Parse(text, state.format, state.provider));

    internal bool TryParse(string input, string format, IFormatProvider? provider, out TResult value)
        => TryGetValue(GetPipeline(format, provider).Parse(input), out value);

    internal bool TryParse(ReadOnlySpan<char> input, string format, IFormatProvider? provider, out TResult value)
        => TryGetValue(GetPipeline(format, provider).Parse(input), out value);

    internal bool TryParse(ReadOnlySpan<byte> input, string format, IFormatProvider? provider, out TResult value)
    {
        var result = Utf8PeriodInput.Parse(input, (Parser: this, format, provider), throwOnInvalidUtf8: false,
            static (text, state) =>
            {
                var success = state.Parser.TryParse(text, state.format, state.provider, out var parsed);
                return (Success: success, Value: parsed);
            });
        value = result.Value;
        return result.Success;
    }

    private Parser<char, TResult> GetPipeline(string format, IFormatProvider? provider)
        => ParserPipelineCache.Shared.Get(format, provider, compose);

    private static TResult GetValue(Result<char, TResult> result)
        => result.Success ? result.Value : throw new FormatExceptionFactory().Create(result);

    private static bool TryGetValue(Result<char, TResult> result, out TResult value)
    {
        value = result.Success ? result.Value : default;
        return result.Success;
    }

    private Parser<char, TResult> Compose(Format tokens, IFormatProvider? provider)
    {
        var factory = new ParserFactory(provider);
        // Only the first matching token contributes a value; later tokens still validate.
        var indices = Array.ConvertAll(components, component => tokens.GetIndex(component.TokenType));
        Parser<char, TResult>? parser = null;
        var index = 0;

        foreach (var token in tokens)
        {
            var componentIndex = Array.IndexOf(indices, index++);
            if (componentIndex >= 0)
            {
                var component = components[componentIndex];
                var value = factory.CreateValue(token);
                parser = parser is null
                    ? value.Select(number => component.SetValue(default, number))
                    : parser.Then(value, component.SetValue);
            }
            else
            {
                var ignored = factory.CreateIgnored(token);
                parser = parser is null ? ignored.ThenReturn(default(TResult)) : parser.Before(ignored);
            }
        }

        if (parser is null)
            throw new ArgumentException("At least one parser must be provided.", "parsers");

        parser = parser.Before(Parser<char>.End);
        var missing = Array.IndexOf(indices, -1);
        if (missing >= 0)
        {
            // Keep syntax and trailing-input errors ahead of missing-component errors.
            var type = components[missing].TokenType;
            parser = parser.Select<TResult>(_ => throw FormatExceptionFactory.MissingToken(type));
        }

        return parser;
    }
}
