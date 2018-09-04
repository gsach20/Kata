using System;
using System.Linq;
using ConsoleApplication1;
using NUnit.Framework;

namespace ConsoleApplication1
{
    class GoingToZeroOrToInfinity
    {
        public static double going(int n)
        {
            Console.WriteLine(n);

            double factorialSum = 0;
            for (int i = 1; i <= n; i++)
            {
                factorialSum += 1 / FI_DEV_FN(i,n);
            }

            return Math.Round(factorialSum, 6);
        }

        private static double FI_DEV_FN(int n, int d)
        {
            double retVal = 1;
            for(int i = n + 1; i<=d; i++)
                retVal *= i;
            return retVal;
        }
    }
}

[TestFixture]
public class SuiteTests
{
    [Test]
    public void Test01()
    {
        Assert.AreEqual(1.275, GoingToZeroOrToInfinity.going(5));
    }
    [Test]
    public void Test02()
    {
        Assert.AreEqual(1.2125, GoingToZeroOrToInfinity.going(6));
    }
    [Test]
    public void Test03()
    {
        Assert.AreEqual(1.173214, GoingToZeroOrToInfinity.going(7));
    }
}
