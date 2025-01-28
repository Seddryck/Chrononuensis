using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Chrononuensis.SourceGenerator.Definitions;
internal class TokenDefinition : IEnumerable<TokenMember>
{
    public string Group { get; set; } = string.Empty;
    public List<TokenMember> Members { get; set; } = new();

    public IEnumerator<TokenMember> GetEnumerator()
        => Members.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
