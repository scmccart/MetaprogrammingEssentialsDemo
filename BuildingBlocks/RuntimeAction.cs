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
        MethodInfo _method;
        Task<Action<TTarget, TParam>> _builderTask;

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

            _method = method;
            _builderTask = Task.Factory.StartNew<Action<TTarget, TParam>>(BuildAction);
        }

        private Action<TTarget, TParam> BuildAction()
        {
            var target = Expression.Parameter(typeof(TTarget), "target");
            var param = Expression.Parameter(typeof(TParam), "param");

            var call = Expression.Call(target, _method, param);

            var lambda = Expression.Lambda<Action<TTarget, TParam>>(call, target, param);

            return lambda.Compile();
        }

        public void Invoke(TTarget target, TParam param)
        {
            if (_builderTask.IsCompleted)
            {
                _builderTask.Result(target, param);
            }
            else
            {
                _method.Invoke(target, new object[] { param });
            }
        }
    }
}
