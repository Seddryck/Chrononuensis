﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Chrononuensis.SourceGenerator.Definitions;
internal class StructDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Default { get; set; } = string.Empty;
    public List<StructPart> Parts { get; set; } = new();
    public StructPeriod? Period { get; set; }
}
