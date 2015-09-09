using System;
using System.Linq.Expressions;

namespace Penneo.Util
{
    /// <summary>
    /// Reflection utilities
    /// </summary>
    internal static class ReflectionUtil
    {
        /// <summary>
        /// Get the property name from a property get expression
        /// </summary>
        public static string GetPropertyName<T>(Expression<Func<T, object>> property)
        {
            var e = property.Body;

            var unary = property.Body as UnaryExpression;
            if (unary != null)
            {
                e = unary.Operand;
            }

            return ((MemberExpression) e).Member.Name;
        }

        /// <summary>
        /// Get the property name from a property get expression
        /// </summary>
        public static string GetPropertyName<T>(Expression<Func<T, byte[]>> property)
        {
            return ((MemberExpression)property.Body).Member.Name;
        }

        /// <summary>
        /// Get the property name from a property get expression
        /// </summary>
        public static string GetPropertyName<T>(Expression<Func<T, string>> property)
        {
            return ((MemberExpression) property.Body).Member.Name;
        }
    }
}