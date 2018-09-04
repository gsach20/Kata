using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ConsoleApplication1
{
    partial class Kata
    {
        public static int IsInteresting(int number, List<int> awesomePhrases)
        {
            if (IsInterestingNumer(number, awesomePhrases)) return 2;
            if (IsInterestingNumer(number+1, awesomePhrases)) return 1;
            if (IsInterestingNumer(number+2, awesomePhrases)) return 1;

            return 0;
        }

        private static bool IsInterestingNumer(int number, List<int> awesomePhrases)
        {
            if (number < 100) return false;

            var digits = GetDigits(number);

            List<int> interestingNumbers = new List<int>();
            interestingNumbers.Add((int) (digits.First() * Math.Pow(10, digits.Count - 1)));
            interestingNumbers.Add(digits.Aggregate((a, b) => a * 10 + a % 10)); //11111
            if (digits.Count <= 10 - digits.First() + 1)
                interestingNumbers.Add(digits.Aggregate((a, b) => a * 10 + (a % 10 + 1) % 10)); //1234
            if (digits.Count <= digits.First() + 1)
                interestingNumbers.Add(digits.Aggregate((a, b) => a * 10 + a % 10 - 1)); //43210

            if (interestingNumbers.Contains(number)) return true;
            if (IsPalindrome(digits)) return true;
            if (awesomePhrases.Contains(number)) return true;

            return false;
        }

        private static List<int> GetDigits(int number)
        {
            List<int> digits = new List<int>();
            while (number > 0)
            {
                digits.Add(number % 10);
                number = number / 10;
            }
            digits.Reverse();
            return digits;
        }

        private static bool IsPalindrome(List<int> digits)
        {
            int count = digits.Count;
            int middleCount = count/2;
            for (int i = 0; i < middleCount; i++)
            {
                if (digits[i] != digits[count - 1 - i]) return false;
            }

            return true;
        }
    }

    

    [TestFixture]
    public class Test
    {
        [Test]
        public void ShouldWorkTest()
        {
            Assert.AreEqual(1, Kata.IsInteresting(98, new List<int>() { 1337, 256 }));
            Assert.AreEqual(2, Kata.IsInteresting(3210, new List<int>() { 1337, 256 }));
            Assert.AreEqual(2, Kata.IsInteresting(67890, new List<int>() { 1337, 256 }));
            Assert.AreEqual(2, Kata.IsInteresting(9999999, new List<int>() { 1337, 256 }));
            Assert.AreEqual(0, Kata.IsInteresting(3, new List<int>() { 1337, 256 }));
            Assert.AreEqual(1, Kata.IsInteresting(1336, new List<int>() { 1337, 256 }));
            Assert.AreEqual(2, Kata.IsInteresting(1337, new List<int>() { 1337, 256 }));
            Assert.AreEqual(2, Kata.IsInteresting(2000, new List<int>() { 1337, 256 }));
            Assert.AreEqual(2, Kata.IsInteresting(1234, new List<int>() { 1337, 256 }));
            Assert.AreEqual(2, Kata.IsInteresting(12321, new List<int>() { 1337, 256 }));
            Assert.AreEqual(0, Kata.IsInteresting(11208, new List<int>() { 1337, 256 }));
            Assert.AreEqual(1, Kata.IsInteresting(11209, new List<int>() { 1337, 256 }));
            Assert.AreEqual(2, Kata.IsInteresting(11211, new List<int>() { 1337, 256 }));
        }
    }
}
