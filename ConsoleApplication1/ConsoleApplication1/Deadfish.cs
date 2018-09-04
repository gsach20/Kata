using System.Collections.Generic;
using NUnit.Framework;

public class Deadfish
{
    public static int[] Parse(string data)
    {
        List<int> numbers = new List<int>();
        int number = 0;
        foreach (char c in data)
        {
            if (c == 'i') number++;
            else if (c == 'd') number--;
            else if (c == 's') number *= number;
            else if( c == 'o') numbers.Add(number);
        }

        return numbers.ToArray();
    }
}

namespace Solution {
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class SolutionTest
    {
        private static object[] sampleTestCases = new object[]
        {
            new object[] {"iiisdoso", new int[] {8, 64}},
            new object[] {"iiisdosodddddiso", new int[] {8, 64, 3600}},
        };
  
        [Test, TestCaseSource("sampleTestCases")]
        public void SampleTest(string data, int[] expected)
        {
            Assert.AreEqual(expected, Deadfish.Parse(data));
        }
    }
}