using System.Buffers;
using System.Text.Unicode;

namespace Chrononuensis.Parsers;

internal static class Utf8PeriodInput
{
    internal delegate TResult ParseText<TState, TResult>(ReadOnlySpan<char> input, TState state);

    internal static TResult Parse<TState, TResult>(
        ReadOnlySpan<byte> input, TState state, bool throwOnInvalidUtf8, ParseText<TState, TResult> parse)
    {
        const int StackBufferSize = 256;
        char[]? rented = null;
        Span<char> buffer = input.Length <= StackBufferSize
            ? stackalloc char[input.Length]
            : (rented = ArrayPool<char>.Shared.Rent(input.Length));

        try
        {
            var status = Utf8.ToUtf16(input, buffer, out _, out var charsWritten, replaceInvalidSequences: false);
            if (status == OperationStatus.Done)
                return parse(buffer[..charsWritten], state);

            if (throwOnInvalidUtf8)
                throw new FormatException("Input is not valid UTF-8.");

            return default!;
        }
        finally
        {
            if (rented is not null)
                ArrayPool<char>.Shared.Return(rented);
        }
    }
}
