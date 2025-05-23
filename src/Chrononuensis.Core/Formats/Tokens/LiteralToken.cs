﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrononuensis.Formats.Tokens;
internal class LiteralToken : FormatToken
{
    public string Value { get; }

    public LiteralToken(char value)
        => Value = value.ToString();

    public LiteralToken(string value)
        => Value = value;
}
