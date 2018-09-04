using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ConsoleApplication1
{
    class FindTheOddInt
    {
        public static int find_it(int[] seq)
        {
            HashSet<int> intOddSet = new HashSet<int>();
            foreach (int i in seq)
            {
                if (!intOddSet.Remove(i)) intOddSet.Add(i);
            }

            return intOddSet.First();
        }

        [Test]
        public void Tests()
        {
            Assert.AreEqual(5, find_it(new[] { 20, 1, -1, 2, -2, 3, 3, 5, 5, 1, 2, 4, 20, 4, -1, -2, 5 }));
        }
    }
}
