using BuildingBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DemoTwo
{
    abstract class ConventionInvoker
    {
        protected static readonly Regex s_tokenRegex = new Regex("{[^{]+}", RegexOptions.Compiled);
        protected static readonly MethodInfo s_toString = typeof(Object).GetMethod("ToString");
        protected static readonly MethodInfo s_toUpperInvariant = typeof(String).GetMethod("ToUpperInvariant");
    }

    class ConventionInvoker<TTarget, TMessage> : ConventionInvoker
    {
        Func<TMessage, string[]> _keyer;

        IReadOnlyDictionary<string[], RuntimeAction<TTarget, TMessage>> _methods;

        public ConventionInvoker(string conventionPattern)
        {
            _keyer = BuildKeyer(conventionPattern);
            _methods = FindMatchingMethods(conventionPattern);
        }

        public void Invoke(TTarget target, TMessage message)
        {
            var key = _keyer(message);
            RuntimeAction<TTarget, TMessage> _method = null;

            if (_methods.TryGetValue(key, out _method))
            {
                _method.Invoke(target, message);
            }
        }

        private static IReadOnlyDictionary<string[], RuntimeAction<TTarget, TMessage>> FindMatchingMethods(string conventionPattern)
        {
            var regexed = new Regex(s_tokenRegex.Replace(conventionPattern, "([A-Z][a-z]*)"));

            return typeof(TTarget)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Select(method => new
                {
                    Method = method,
                    Matches = regexed.Match(method.Name)
                        .Groups
                        .AsEnumerable()
                        .Skip(1) //Skip all match.
                        .Select(g => g.Value.ToUpperInvariant())
                        .ToArray()
                })
                .Where(info => info.Matches.Length > 0)
                .ToDictionary(
                    info => info.Matches,
                    info => new RuntimeAction<TTarget, TMessage>(info.Method),
                    new StringArrayEqualityComparer());
        }

        private static Func<TMessage, string[]> BuildKeyer(string conventionPattern)
        {
            var matches = s_tokenRegex.Matches(conventionPattern);

            var properties = matches.AsEnumerable()
                .Select(match => match.Value.Trim('{', '}'))
                .Select(name => typeof(TMessage).GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase))
                .Where(prop => prop != null)
                .ToArray();

            if (matches.Count != properties.Length)
            {
                throw new ArgumentException("TMessage does not meet convention", "conventionPattern");
            }

            var message = Expression.Parameter(typeof(TMessage), "message");

            var propertyGetters = properties
                .Select(p => Expression.Property(message, p))
                .Select(px => Expression.Condition(
                    Expression.Equal(px, Expression.Constant(null)),
                    Expression.Constant("Null"),
                    Expression.Call(Expression.Call(px, s_toString), s_toUpperInvariant)));

            var arrayInit = Expression.NewArrayInit(typeof(string), propertyGetters);

            var lambda = Expression.Lambda<Func<TMessage, string[]>>(arrayInit, message);

            return lambda.Compile();
        }
    }
}
