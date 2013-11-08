using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks
{
    public class RuntimeCreator<TTarget>
    {
        public Func<TTarget> Creator { get; private set; }

        public RuntimeCreator()
        {
            Creator = BuildCreator(typeof(TTarget));
        }

        public RuntimeCreator(Type derivedType)
        {
            if (!typeof(TTarget).IsAssignableFrom(derivedType))
            {
                throw new ArgumentException("derivedType must be assignable to TTarget", "derivedType");
            }

            Creator = BuildCreator(derivedType);
        }

        static Func<TTarget> BuildCreator(Type type)
        {
            // 1. Declare Parameters ... None in this case

            // 2. Build out body
            var body = Expression.New(type);

            // 3. Pull together in Lambda
            var lambda = Expression.Lambda<Func<TTarget>>(body);

            // 4. Compile
            return lambda.Compile();
        }

        public TTarget Instantiate()
        {
            // 5. Use!
            return Creator();
        }
    }
}
