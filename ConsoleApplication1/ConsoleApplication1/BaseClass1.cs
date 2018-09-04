using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class BaseClass1
    {
        public virtual int MyFunc(int i, int j)
        {
            Console.Out.Write("In class BaseClass1");
            return 1;
        }
    }
}
