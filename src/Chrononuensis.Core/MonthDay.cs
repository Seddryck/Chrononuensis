using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Chrononuensis;
public partial record struct MonthDay
{
    
    private static readonly int[] longMonths = [1, 3, 5, 7, 8, 10, 12];

    private static int GetDaysInMonth(int month)
        => month switch
        {
            _ when longMonths.Contains(month) => 31,
            2 => 29,
            >= 1 and <= 12 => 30,
            _ => throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12")
        };
}
