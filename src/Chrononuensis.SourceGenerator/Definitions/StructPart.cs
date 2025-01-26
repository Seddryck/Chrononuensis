using System;
using System.Collections.Generic;
using System.Text;

namespace Chrononuensis.SourceGenerator.Definitions;
internal class StructPart
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int? Min { get; set; }
    public int? Max { get; set; }
}
