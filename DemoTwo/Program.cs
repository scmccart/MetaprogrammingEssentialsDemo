using BuildingBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DemoTwo
{
    class Program
    {
        static readonly PropertyInfo s_name = typeof(TestDummy).GetProperty("Name");
        static readonly MethodInfo s_sayHello = typeof(TestDummy).GetMethod("SayHello");

        static void Main(string[] args)
        {
            //Expression<Func<string, int>> lambda = x => x.Length;
            //Console.WriteLine(lambda);

            var creator = new RuntimeCreator<TestDummy>();
            var nameGetter = new RuntimeGetter<TestDummy, string>(s_name);
            var nameSetter = new RuntimeSetter<TestDummy, string>(s_name);
            var sayHello = new RuntimeAction<TestDummy, string>(s_sayHello);

            var dummy = creator.Instantiate();

            nameSetter.SetValue(dummy, "Wilbur");

            var name = nameGetter.GetValue(dummy);
            Console.WriteLine("Dummy's name is {0}", name);

            sayHello.Invoke(dummy, "Joe");

            Console.ReadKey();
        }
    }
}
