using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Chrononuensis.SourceGenerator.Definitions;
internal class StructCollection : IEnumerable<StructDefinition>
{
    public List<StructDefinition> Items { get; set; } = new();

    public IEnumerator<StructDefinition> GetEnumerator()
        => Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
