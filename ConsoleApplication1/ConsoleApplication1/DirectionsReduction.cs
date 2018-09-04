using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ConsoleApplication1
{
    public class DirReduction
    {

        public static string[] dirReduc(string[] arr)
        {
            var stack = new Stack<string>();
            stack.Push(string.Empty);
            stack = arr.Aggregate(stack, (dirStack, currDir) =>
            {
                if (TryDirReduc(currDir, dirStack.Peek())) dirStack.Pop();
                else dirStack.Push(currDir);
                return dirStack;
            });

            var list = stack.Reverse().ToList();
            list.RemoveAt(0);
            return list.ToArray();
        }

        private static bool TryDirReduc(string currDir, string prevDir)
        {
            if (prevDir == "NORTH" && currDir == "SOUTH") return true;
            if (prevDir == "SOUTH" && currDir == "NORTH") return true;
            if (prevDir == "EAST" && currDir == "WEST") return true;
            if (prevDir == "WEST" && currDir == "EAST") return true;
            return false;
        }
    }

    [TestFixture]
    public class DirReductionTests
    {

        [Test]
        public void Test1()
        {
            string[] a = new string[] { "NORTH", "SOUTH", "SOUTH", "EAST", "WEST", "NORTH", "WEST" };
            string[] b = new string[] { "WEST" };
            Assert.AreEqual(b, DirReduction.dirReduc(a));
        }
        [Test]
        public void Test2()
        {
            string[] a = new string[] { "NORTH", "WEST", "SOUTH", "EAST" };
            string[] b = new string[] { "NORTH", "WEST", "SOUTH", "EAST" };
            Assert.AreEqual(b, DirReduction.dirReduc(a));
        }
    }

}
