using BuildingBlocks;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpeedTest
{
    class Program
    {
        const int ITERATIONS = 1000000;
        const int RUNS = 10;

        static void Main(string[] args)
        {
            Console.Write("Running... ");

            var times = Enumerable.Range(1, RUNS)
                .Select(run => new
                {
                    Run = run,
                    Native = Time(RunNative),
                    Dynamic = Time(RunDynamic),
                    Reflection = Time(RunReflection),
                    ExpressionTree = Time(RunExpressionTree),
                    RuntimeAction = Time(RunRuntimeAction),
                    CreateDelegate = Time(RunCreateDelegate)
                })
                .ToArray();

            using (var csv = new CsvWriter(File.CreateText(String.Format("runtimes-{0:yyyyMMddHHmmss}.csv", DateTime.Now))))
            {
                csv.WriteRecords(times);
            }

            Console.WriteLine("Done");
        }

        static readonly Lazy<Action<TestTarget, string>> s_native = new Lazy<Action<TestTarget, string>>(() => (inst, value) => inst.DoSomething(value));
        static void RunNative()
        {
            var inst = new TestTarget();

            for (int i = 0; i < ITERATIONS; i++)
            {
                s_native.Value(inst, string.Empty);
            }
        }

        static readonly Lazy<Action<dynamic, string>> s_dynamic = new Lazy<Action<dynamic, string>>(() => (inst, value) => inst.DoSomething(value));
        static void RunDynamic()
        {
            var inst = new TestTarget();

            for (int i = 0; i < ITERATIONS; i++)
            {
                s_dynamic.Value(inst, string.Empty);
            }
        }

        static readonly Lazy<MethodInfo> s_methodInfo = new Lazy<MethodInfo>(() => typeof(TestTarget).GetMethod("DoSomething"));
        static void RunReflection()
        {
            var inst = new TestTarget();

            for (int i = 0; i < ITERATIONS; i++)
            {
                s_methodInfo.Value.Invoke(inst, new object[] { string.Empty });
            }
        }

        static readonly Lazy<Action<TestTarget, string>> s_expressionTree = new Lazy<Action<TestTarget, string>>(BuildExpressionTreeAction);
        static void RunExpressionTree()
        {
            var inst = new TestTarget();

            for (int i = 0; i < ITERATIONS; i++)
            {
                s_expressionTree.Value(inst, string.Empty);
            }
        }

        static Action<TestTarget, string> BuildExpressionTreeAction()
        {
            var method = typeof(TestTarget).GetMethod("DoSomething");

            var pTarget = Expression.Parameter(typeof(TestTarget), "target");
            var pValue = Expression.Parameter(typeof(string), "value");

            var body = Expression.Call(pTarget, method, pValue);

            var lambda = Expression.Lambda<Action<TestTarget, string>>(body, pTarget, pValue);

            return lambda.Compile();
        }

        static readonly Lazy<RuntimeAction<TestTarget, string>> s_runtimeAction = new Lazy<RuntimeAction<TestTarget, string>>(() => new RuntimeAction<TestTarget, string>(typeof(TestTarget).GetMethod("DoSomething")));

        static void RunRuntimeAction()
        {
            var inst = new TestTarget();

            for (int i = 0; i < ITERATIONS; i++)
            {
                s_runtimeAction.Value.Invoke(inst, string.Empty);
            }
        }

        static readonly Lazy<Action<string>> s_createDelegate = new Lazy<Action<string>>(() => (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), new TestTarget(), typeof(TestTarget).GetMethod("DoSomething")));
        static void RunCreateDelegate()
        {
            for (int i = 0; i < ITERATIONS; i++)
            {
                s_createDelegate.Value(string.Empty);
            }
        }

        static double Time(Action action)
        {
            var watch = Stopwatch.StartNew();

            action();

            watch.Stop();

            return watch.ElapsedMilliseconds;
        }
    }
}
