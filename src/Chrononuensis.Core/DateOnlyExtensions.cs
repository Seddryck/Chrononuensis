using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chrononuensis;
internal static class DateOnlyExtensions
{
    public static DateOnly Max(DateOnly dateOnly, params DateOnly[] others)
    {
        var max = dateOnly;
        foreach (var other in others)
            if (other > max)
                max = other;
        return max;
    }

    public static DateOnly Min(DateOnly dateOnly, params DateOnly[] others)
    {
        var min = dateOnly;
        foreach (var other in others)
            if (other < min)
                min = other;
        return min;
    }
}
