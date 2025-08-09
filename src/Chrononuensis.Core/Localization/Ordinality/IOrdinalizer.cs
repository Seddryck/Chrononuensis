using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrononuensis.Localization.Ordinality;
public interface IOrdinalizer
{
    string ToOrdinal(int value);  // e.g. "21e", "21st", "21."
    bool TryParse(string input, out int value); // e.g. "21e" → 21
}
