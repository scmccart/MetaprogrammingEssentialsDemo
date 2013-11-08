using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks
{
    public class RuntimeAction<TTarget, TParam>
    {
        public Action<TTarget, TParam> Action { get; private set; }

        public RuntimeAction(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            var parameters = method.GetParameters();
            if (parameters.Length != 1 || parameters[0].ParameterType != typeof(TParam))
            {
                throw new ArgumentException("method must except one parameter of type TParam", "method");
            }

            Action = BuildAction(method);
        }

        static Action<TTarget, TParam> BuildAction(MethodInfo method)
        {
            // 1. Declare Parameters
            var target = Expression.Parameter(typeof(TTarget), "target");
            var param = Expression.Parameter(typeof(TParam), "param");

            // 2. Build out body
            var call = Expression.Call(target, method, param);

            // 3. Pull together in Lambda
            var lambda = Expression.Lambda<Action<TTarget, TParam>>(call, target, param);

            // 4. Compile
            return lambda.Compile();
        }

        public void Invoke(TTarget target, TParam param)
        {
            // 5. Use!
            Action(target, param);
        }
    }
}
