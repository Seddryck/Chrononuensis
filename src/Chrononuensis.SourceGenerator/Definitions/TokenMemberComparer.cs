using System;
using System.Collections.Generic;
using System.Text;

namespace Chrononuensis.SourceGenerator.Definitions;
internal class TokenMemberComparer : IEqualityComparer<TokenMember>
{
    public bool Equals(TokenMember? x, TokenMember? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;

        return string.Equals(x.Name, y.Name, StringComparison.Ordinal);
    }

    public int GetHashCode(TokenMember obj)
    {
        if (obj is null) throw new ArgumentNullException(nameof(obj));

        return obj.Name.GetHashCode();
    }
}
