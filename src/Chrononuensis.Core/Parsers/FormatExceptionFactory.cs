using Pidgin;

namespace Chrononuensis.Parsers;
internal class FormatExceptionFactory
{
    internal static FormatException MissingToken(Type type)
        => new($"Token '{type.Name[1..^5].ToLower()}' not found in the format");

    public FormatException Create<T>(Result<char, T> result)
    {
        var error = result.Error
            ?? throw new ArgumentNullException(nameof(result.Error), "Error object cannot be null.");
        var position = error.ErrorPos.Col;

        if (!error.Unexpected.HasValue)
            return new FormatException($"Parsing error at character {position}: {error.Message}.");

        var unexpected = error.EOF ? "end-of-file" : $"'{error.Unexpected.Value}'";
        if (!error.Expected.Any())
            return new FormatException($"Parsing error at character {position}: Unexpected {unexpected}.");

        var expectation = error.Expected.First();
        var expected = expectation.IsEof ? "end-of-file" : expectation.Label ?? $"'{expectation.Tokens[0]}'";
        return new FormatException($"Parsing error at character {position}: Expected {expected} but found {unexpected}.");
    }
}
