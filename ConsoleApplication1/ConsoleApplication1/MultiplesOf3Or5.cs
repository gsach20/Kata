using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ConsoleApplication1
{
    class MultiplesOf3Or5
    {

        public static int Solution(int value)
        {
            int sum = 0;
            int factor = 1;
            while(3 * factor < value)
            {
                sum += 3 * factor;
                factor++;
            }
            factor = 1;
            while (5 * factor < value)
            {
                sum += 5 * factor;
                factor++;
            }
            factor = 1;
            while (15 * factor < value)
            {
                sum -= 15 * factor;
                factor++;
            }
            return sum;
        }

        [Test]
        public void Test()
        {
            Assert.AreEqual(23, Solution(10));
        }
    }
}
