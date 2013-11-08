using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoThree
{
    class Program
    {
        static void Main(string[] args)
        {
            var invoker = new ConventionInvoker<TestTarget, TestMessage>("Index{type}{action}");

            var target = new TestTarget();

            var message = new TestMessage()
            {
                Type = "Contact",
                Action = "Add",
                Text = "Hello, World!"
            };

            invoker.Invoke(target, message);

            Console.Write("Done");
            Console.ReadKey();
        }
    }
}
