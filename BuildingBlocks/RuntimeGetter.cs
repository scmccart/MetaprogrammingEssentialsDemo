using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks
{
    public class RuntimeGetter<TTarget, TProp>
    {
        public Func<TTarget, TProp> Getter { get; private set; }

        public RuntimeGetter(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            Getter = BuildGetter(property);
        }

        static Func<TTarget, TProp> BuildGetter(PropertyInfo property)
        {
            // 1. Declare Parameters
            var target = Expression.Parameter(typeof(TTarget), "target");

            // 2. Build out body
            var body = Expression.Property(target, property);

            // 3. Pull together in Lambda
            var lambda = Expression.Lambda<Func<TTarget, TProp>>(body, target);

            // 4. Compile
            return lambda.Compile();
        }

        public TProp GetValue(TTarget target)
        {
            // 5. Use!
            return Getter(target);
        }
    }
}
