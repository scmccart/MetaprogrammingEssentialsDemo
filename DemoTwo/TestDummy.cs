using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTwo
{
    class TestDummy
    {
        public string Name { get; set; }

        public void SayHello(string to)
        {
            Console.WriteLine("Hello, {0}, my name is {1}!", to, this.Name);
        }
    }
}
