using Algorithms.Abstractions;

namespace Algorithms.Common;
public static class IntHelpers
{
    public static (int, int) PickRandomDivisors(int number, IRandomProvider randomProvider)
    {
        var divisors = new List<(int, int)>();
        for (var i = 1; i <= number / 2; i++)
        {
            if (number % i == 0)
            {
                divisors.Add((i, number / i));
            }
        }

        return randomProvider.NextItem(divisors);
    }
}
