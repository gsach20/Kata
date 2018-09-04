using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

class PrimeDecomp
{
    public static string factors(int n)
    {
        List<Pair<int, int>> primeNumCount = new List<Pair<int, int>>();
        primeNumCount.Add(new Pair<int, int>(2, 0));
        DecompNum(n, primeNumCount);

        return primeNumCount.Where(p => p.Count != 0).Select(p => "(" + p.PrimeNumber.ToString() + (p.Count > 1 ? "**" + p.Count : "") + ")")
            .Aggregate((curr, next) => curr + next);
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
public class PrimeDecompTests {

    [Test]
    public void Test1() {
  
        int lst = 7775460;
        Assert.AreEqual("(2**2)(3**3)(5)(7)(11**2)(17)", PrimeDecomp.factors(lst));
    }
}


