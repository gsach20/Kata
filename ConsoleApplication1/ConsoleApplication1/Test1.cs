using System;
using System.Collections;
using System.Collections.Generic;
using Castle.Core.Internal;
using NUnit.Framework;

namespace MySpace123
{
    [TestFixture]
    public class MyTest123
    {
        [Test]
        public void Test11()
        {
            foreach (var myString in GetMyStrings(false)) Console.WriteLine(myString);
        }

        private IEnumerable<string> GetMyStrings(bool yieldReturn)
        {
            if (yieldReturn)
                yield return "MyString";
        }

        [Test]
        public void Test22()
        {
            MyClass obj1 = new MyClass(5);

            MyClass obj2 = obj1;

            obj2.i = 7;

            Assert.AreEqual(5, obj1.i);
        }

        [Test]
        public void Test33()
        {
            Assert.Fail(GetBinary(4).ToString());
        }

        private static int GetBinary(int value)
        {
            char[] binaryString = new[] {'1', '0', '1', '0'};
            //binaryString[4 - value] = '1';
            return Convert.ToInt32(string.Concat(binaryString), 2);
        }
    }

    class MyClass
    {
        public int i;

        public MyClass(int i)
        {
            this.i = i;
        }
    }
}
