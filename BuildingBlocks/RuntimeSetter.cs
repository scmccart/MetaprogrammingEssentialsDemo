using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks
{
    public class RuntimeSetter<TTarget, TProp>
    {
        public Action<TTarget, TProp> Setter { get; private set; }

        public RuntimeSetter(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            Setter = BuildSetter(property);
        }

        static Action<TTarget, TProp> BuildSetter(PropertyInfo property)
        {
            // 1. Declare Parameters
            var target = Expression.Parameter(typeof(TTarget), "target");
            var param = Expression.Parameter(typeof(TProp), "value");

            // 2. Build out body
            var body = Expression.Assign(
                Expression.Property(target, property),
                param);

            // 3. Pull together in Lambda
            var lambda = Expression.Lambda<Action<TTarget, TProp>>(body, target, param);

            // 4. Compile
            return lambda.Compile();
        }

        public void SetValue(TTarget target, TProp param)
        {
            // 5. Use!
            Setter(target, param);
        }
    }
}
