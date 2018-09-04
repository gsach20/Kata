using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

class FactDecomp
{
    public static string Decomp(int n)
    {
        List<Pair<int, int>> primeNumCount = new List<Pair<int, int>>();
        primeNumCount.Add(new Pair<int, int>(2, 0));

        for (int i = 1; i <= n; i++)
            DecompNum(i, primeNumCount);

        return primeNumCount.Select(p => p.PrimeNumber.ToString() + (p.Count > 1 ? "^" + p.Count : ""))
            .Aggregate((curr, next) => curr + " * " + next);

    }

    private static void DecompNum(int number, List<Pair<int, int>> primeNumCount)
    {
        foreach (Pair<int, int> pair in primeNumCount)
        {
            pair.Count += GetFactor(ref number, pair.PrimeNumber);
            if(number == 1) return;
        }

        do
        {
            Pair<int, int> nextPrimeNumber = GetNextPrimeNumber(primeNumCount);
            nextPrimeNumber.Count += GetFactor(ref number, nextPrimeNumber.PrimeNumber);
        } while (number != 1);
    }

    private static Pair<int, int> GetNextPrimeNumber(List<Pair<int, int>> primeNumCount)
    {
        for(int current = primeNumCount.Last().PrimeNumber + 1;; current++)
        {
            bool isPrime = true;
            foreach (Pair<int, int> pair in primeNumCount)
            {
                if (current % pair.PrimeNumber == 0)
                {
                    isPrime = false;
                    break;
                }
            }

            if (isPrime)
            {
                Pair<int, int> pair = new Pair<int, int>(current, 0);
                primeNumCount.Add(pair);
                return pair;
            }
        } 
        
        
    }

    private static int GetFactor(ref int number, int primeNumber)
    {
        int count = 0;
        do
        {
            if (number % primeNumber != 0) break;
            count++;
            number = number / primeNumber;
        } while (true);

        return count;
    }

    public class Pair<T, U> {
        public Pair(T primeNumber, U count) {
            PrimeNumber = primeNumber;
            Count = count;
        }

        public T PrimeNumber { get; set; }
        public U Count { get; set; }
    }

}

[TestFixture]
public class FactDecompTest
{
    private static void testing(int n, string expected)
    {
        Console.WriteLine("n: {0}, expected: {1}", n, expected);
        Assert.AreEqual(expected, FactDecomp.Decomp(n));
    }
    [Test]
    public static void test()
    {
        testing(17, "2^15 * 3^6 * 5^3 * 7^2 * 11 * 13 * 17");
        testing(5, "2^3 * 3 * 5");
        testing(22, "2^19 * 3^9 * 5^4 * 7^3 * 11^2 * 13 * 17 * 19");
        testing(14, "2^11 * 3^5 * 5^2 * 7^2 * 11 * 13");
        testing(25, "2^22 * 3^10 * 5^6 * 7^3 * 11^2 * 13 * 17 * 19 * 23");
    }
}

