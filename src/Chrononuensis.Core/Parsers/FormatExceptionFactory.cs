using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pidgin;

namespace Chrononuensis.Parsers;
internal class FormatExceptionFactory
{
    public FormatException Create(Result<char, object[]> result)
    {
        var error = result.Error
                        ?? throw new ArgumentNullException(nameof(result.Error), "Error object cannot be null.");

        var position = error.ErrorPos.Col;

        if (error.Unexpected.HasValue && error.Expected.Any())
        {
            var unexpected = error.Unexpected.HasValue && error.EOF
                                ? "end-of-file"
                                : error.Unexpected.HasValue && error.Unexpected.Value.ToString() == ""
                                    ? $"'{error.Unexpected.Value}'"
                                    : $"'{error.Unexpected.Value}'";

            var expected = error.Expected.FirstOrDefault().IsEof
                                    ? "end-of-file"
                                    : error.Expected.FirstOrDefault().Label is null
                                        ? $"'{error.Expected.FirstOrDefault().Tokens[0]}'"
                                        : error.Expected.FirstOrDefault().Label;
            

            var message = $"Parsing error at character {position}: Expected {expected} but found {unexpected}.";
            return new FormatException(message);
        }
        else if (error.Unexpected.HasValue)
        {
            var unexpected = error.Unexpected.HasValue && error.EOF
                                ? "end-of-file"
                                : error.Unexpected.HasValue && error.Unexpected.Value.ToString() == ""
                                    ? $"'{error.Unexpected.Value}'"
                                    : $"'{error.Unexpected.Value}'";
            var message = $"Parsing error at character {position}: Unexpected {unexpected}.";
            return new FormatException(message);
        }
        else
        {
            var message = $"Parsing error at character {position}: {error.Message}.";
            return new FormatException(message);
        }
    }
}
