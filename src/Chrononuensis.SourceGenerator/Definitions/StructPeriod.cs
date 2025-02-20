using System;
using System.Collections.Generic;
using System.Text;

namespace Chrononuensis.SourceGenerator.Definitions;
internal class StructPeriod
{
    public string? First { get; set; }
    public string? Last { get; set; }
    public string? Year { get; set; }
    public int? YearDuration { get; set; }
    public int? ByYear { get; set; }
}
