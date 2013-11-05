using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTwo
{
    class TestTarget
    {
        public void IndexContactAdd(TestMessage message)
        {
            Console.WriteLine(message.Text);
        }
    }
}
