using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrononuensis.Formats;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal class FormatIdentifierAttribute : Attribute
{
    public string Value { get; }

    public FormatIdentifierAttribute(string value)
        => Value = value;
}
