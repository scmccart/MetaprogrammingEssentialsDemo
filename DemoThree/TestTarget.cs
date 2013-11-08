using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoThree
{
    class TestTarget
    {
        public void IndexContactAdd(TestMessage message)
        {
            Console.WriteLine(message.Text);
        }
    }
}
