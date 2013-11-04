using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DemoOne
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Declare Parameters
            var value = Expression.Parameter(typeof(int), "value");

            // 2. Build out body
            var body = Expression.Multiply(value, Expression.Constant(2));

            // 3. Pull together in Lambda
            var lambda = Expression.Lambda<Func<int, int>>(body, value);

            // 4. Compile
            var compiled = lambda.Compile();

            // 5. Use!
            Console.Write("input: ");
            var input = int.Parse(Console.ReadLine());
            var output = compiled(input);

            Console.WriteLine("output: " + output);
            Console.ReadKey();
        }
    }
}
