using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Chrononuensis.SourceGenerator.Definitions;
internal class TokenCollection : IEnumerable<TokenDefinition>
{
    public List<TokenDefinition> Items { get; set; } = new();

    public IEnumerator<TokenDefinition> GetEnumerator()
        => Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
