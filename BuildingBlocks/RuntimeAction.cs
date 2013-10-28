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
            var target = Expression.Parameter(typeof(TTarget), "target");
            var param = Expression.Parameter(typeof(TParam), "param");

            var call = Expression.Call(target, method, param);

            var lambda = Expression.Lambda<Action<TTarget, TParam>>(call, target, param);

            return lambda.Compile();
        }

        public void Invoke(TTarget target, TParam param)
        {
            Action(target, param);
        }
    }
}
