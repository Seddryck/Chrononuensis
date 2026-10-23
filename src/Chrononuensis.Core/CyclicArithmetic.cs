namespace Chrononuensis;

internal static class CyclicArithmetic
{
    public static int Normalize(int value, int delta, int minimum, int maximum, out int cycles)
    {
        var range = (long)maximum - minimum + 1;
        var offset = (long)value - minimum + delta;
        var quotient = Math.DivRem(offset, range, out var remainder);

        if (remainder < 0)
        {
            remainder += range;
            quotient--;
        }

        cycles = checked((int)quotient);
        return checked((int)(remainder + minimum));
    }
}
