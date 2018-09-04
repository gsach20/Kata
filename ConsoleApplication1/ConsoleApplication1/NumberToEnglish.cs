using System;
using NUnit.Framework;

namespace ConsoleApplication1
{
    partial class Kata
    {
        public static string NumberToEnglish(int n)
        {
            if(n < 0 || n > 99999)
                return string.Empty;
            if (n == 0) return "zero";

            if (n < 1000) return ProcessThreeDigits(n);
            return ProcessTwoDigits(n/1000) + " thousand" + (n%1000 > 0 ? " " + ProcessThreeDigits(n%1000) : string.Empty);
        }

        private static string ProcessThreeDigits(int i)
        {
            if (i < 100 ) return ProcessTwoDigits(i);
            return ProcessSingleDigit(i / 100) + " hundred" + ( i%100 > 0 ? " " + ProcessTwoDigits(i % 100) : string.Empty);
        }

        private static string ProcessTwoDigits(int i)
        {
            if (i < 10) return ProcessSingleDigit(i);
            if(i < 20) return ProcessLessThan20(i);
            return Process10Mulitiplier(i / 10) + (i%10 > 0 ? " " + ProcessSingleDigit(i%10) : string.Empty);
        }

        private static string Process10Mulitiplier(int i)
        {
            switch (i)
            {
                case 2:
                    return "twenty";
                case 3:
                    return "thirty";
                case 4:
                    return "forty";
                case 5:
                    return "fifty";
                case 6:
                    return "sixty";
                case 7:
                    return "seventy";
                case 8:
                    return "eighty";
                case 9:
                    return "ninety";
                default:
                    throw new InvalidOperationException("invalid input");
            }
        }

        private static string ProcessLessThan20(int i)
        {
            i = i % 10;
            switch (i)
            {
                case 0:
                    return "ten";
                case 1:
                    return "eleven";
                case 2:
                    return "twelve";
                case 3:
                    return "thirteen";
                case 4:
                    return "fourteen";
                case 5:
                    return "fifteen";
                case 6:
                    return "sixteen";
                case 7:
                    return "seventeen";
                case 8:
                    return "eighteen";
                case 9:
                    return "nineteen";
                default:
                    throw new InvalidOperationException("invalid input");
            }
        }

        private static string ProcessSingleDigit(int i)
        {
            switch (i)
            {
                case 1:
                    return "one";
                case 2:
                    return "two";
                case 3:
                    return "three";
                case 4:
                    return "four";
                case 5:
                    return "five";
                case 6:
                    return "six";
                case 7:
                    return "seven";
                case 8:
                    return "eight";
                case 9:
                    return "nine";
                default:
                    throw new InvalidOperationException("invalid input");
            }
        }
    }

    [TestFixture]
    internal class Tests
    {
        [TestCase(-4, "")]
        [TestCase(0, "zero")]
        [TestCase(7, "seven")]
        [TestCase(11, "eleven")]
        [TestCase(20, "twenty")]
        [TestCase(47, "forty seven")]
        [TestCase(100, "one hundred")]
        [TestCase(305, "three hundred five")]
        [TestCase(4002, "four thousand two")]
        [TestCase(20005, "twenty thousand five")]
        [TestCase(6800, "six thousand eight hundred")]
        [TestCase(14111, "fourteen thousand one hundred eleven")]
        [TestCase(3892, "three thousand eight hundred ninety two")]
        [TestCase(99999, "ninety nine thousand nine hundred ninety nine")]
        public void BasicTest(int n, string expected)
        {
            Assert.That(Kata.NumberToEnglish(n), Is.EqualTo(expected));
        }
    }
}
