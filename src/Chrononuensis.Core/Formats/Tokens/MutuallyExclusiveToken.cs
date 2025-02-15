using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrononuensis.Formats.Tokens;
internal class MutuallyExclusiveToken : FormatToken
{
    public FormatToken[] Values { get; }

    public MutuallyExclusiveToken(FormatToken[] values)
        => Values = values;
}
