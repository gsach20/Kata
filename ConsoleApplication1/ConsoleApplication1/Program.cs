using System.Collections.Generic;
using NUnit.Framework;
using System;
using Moq;

namespace ConsoleApplication1
{
    public partial class Kata
    {
        static void Main(string[] args)
        {
           
        }

        int MockMyFunc(int i , int j)
        {
            Console.Out.Write("In MockClass");
            return 1;
        }

        [Test]
        public void TestTest()
        {
            //BaseClass1 baseObj = new Derived11();
            //baseObj.MyFunc();

            Mock<BaseClass1> mockObj = new Mock<BaseClass1>();
            mockObj.Setup(x => x.MyFunc(1, 4)).Returns(MockMyFunc(43, 54));
        }

        [Test]
        public void EmptyTest()
        {
            Assert.AreEqual("", UniqueInOrder(""));
        }

        [Test]
        public void Test1()
        {
            Assert.AreEqual("ABCDAB", UniqueInOrder("AAAABBBCCDAABBB"));
        }

        public static IEnumerable<T> UniqueInOrder<T>(IEnumerable<T> iterable)
        {
            T lastItem = default(T);
            foreach (T x1 in iterable)
            {
                if (!x1.Equals(lastItem))
                {
                    lastItem = x1;
                    yield return x1;
                }
            }
        }





        [Test]
        public void BasicTests()
        {
            Assert.AreEqual(5000, CalculateScrap(new[] { 10 }, 90));
            Assert.AreEqual(3820, CalculateScrap(new[] { 20, 10 }, 55));
        }
        public long CalculateScrap(int[] scraps, int numberOfRobots)
        {
            double ironNeededForLastStep = 50 * numberOfRobots;
            foreach (int scrap in scraps)
            {
                ironNeededForLastStep = 100.0 * ironNeededForLastStep / (100 - scrap);
            }

            double roundedNumber = (long) ironNeededForLastStep;
            if (ironNeededForLastStep > roundedNumber)
                return (long)roundedNumber + 1;
            return (long) ironNeededForLastStep + 1;
        }





        //****************
        //Ragbaby cipher
        //*****************

        public static string Encode(string text, string key)
        {
            return Encode_Decode(text, key, IndexOfC_Encoding);
        }

        public static string Decode(string text, string key)
        {
            return Encode_Decode(text, key, IndexOfC_Decoding);
        }

        private static string Encode_Decode(string text, string key, Func<List<char>, char, int, int> indexOfC)
        {
            List<char> alphabets = KeyedAlphabets(key);

            List<char> endocedString = new List<char>();

            int j = 0;
            foreach (char c in text)
            {
                j++;

                char lowerC = c;
                bool isUpper = false;
                if (char.IsUpper(c))
                {
                    lowerC = char.ToLower(c);
                    isUpper = true;
                }
                if (lowerC >= 'a' && lowerC <= 'z')
                {
                    char encodedChar = alphabets[indexOfC(alphabets, lowerC, j)];
                    if (isUpper) encodedChar = char.ToUpper(encodedChar);
                    endocedString.Add(encodedChar);
                }
                else
                {
                    endocedString.Add(c);
                    j = 0;
                }
            }

            return new string(endocedString.ToArray());
        }

        private static int IndexOfC_Encoding(List<char> alphabets, char lowerC, int j)
        {
            int indexOfC = alphabets.IndexOf(lowerC) + j;
            if (indexOfC >= 26) indexOfC = indexOfC - 26;
            return indexOfC;
        }

        private static int IndexOfC_Decoding(List<char> alphabets, char lowerC, int j)
        {
            int indexOfC = alphabets.IndexOf(lowerC) - j;
            if (indexOfC < 0) indexOfC = 26 + indexOfC;
            return indexOfC;
        }

        private static List<char> KeyedAlphabets(string key)
        {
            HashSet<char> keyHash = new HashSet<char>();
            List<char> alphabets = new List<char>();
            foreach (char c in key)
            {
                if (keyHash.Add(c)) alphabets.Add(c);
            }

            for (char c = 'a'; c <= 'z'; c++)
            {
                if (!keyHash.Contains(c)) alphabets.Add(c);
            }
            return alphabets;
        }

        //    List<char> alphabets = KeyedAlphabets(key);

        //    List<char> decodedString = new List<char>();

        //    int j = 0;
        //    foreach (char c in text)
        //    {
        //        j++;

        //        char lowerC = c;
        //        bool isUpper = false;
        //        if (char.IsUpper(c))
        //        {
        //            lowerC = char.ToLower(c);
        //            isUpper = true;
        //        }
        //        if (lowerC >= 'a' && lowerC <= 'z')
        //        {
        //            char encodedChar = alphabets[IndexOfC_Decoding(alphabets, lowerC, j)];
        //            if (isUpper) encodedChar = char.ToUpper(encodedChar);
        //            decodedString.Add(encodedChar);
        //        }
        //        else
        //        {
        //            decodedString.Add(c);
        //            j = 0;
        //        }
        //    }

        //    return new string(decodedString.ToArray());
        //}








    }

    [TestFixture]
    public class SolutionTest
    {
        [Test]
        public void SampleTests()
        {
            Assert.AreEqual("ihrbfj", Kata.Encode("cipher", "cipher"));
            Assert.AreEqual("ihrbfj", Kata.Encode("cipher", "cccciiiiippphheeeeerrrrr"));
            Assert.AreEqual("Urew pu bq rzfsbtj.", Kata.Encode("This is an example.", "cipher"));
            Assert.AreEqual("Urew.uRew.urEw.ureW...", Kata.Encode("This.tHis.thIs.thiS...", "cipher"));

            Assert.AreEqual("cipher", Kata.Decode("ihrbfj", "cipher"));
            Assert.AreEqual("This is an example.", Kata.Decode("Urew pu bq rzfsbtj.", "cipher"));
            Assert.AreEqual("This.tHis.thIs.thiS...", Kata.Decode("Urew.uRew.urEw.ureW...", "cipher"));

            Assert.AreEqual("This is an example.", Kata.Encode(Kata.Decode("This is an example.", "cipher"), "cipher"));
            Assert.AreEqual("This is an example.", Kata.Decode(Kata.Encode("This is an example.", "cipher"), "cipher"));

        }
    }
}

