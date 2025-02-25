using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

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

            return ((MemberExpression)e).Member.Name;
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
            return ((MemberExpression)property.Body).Member.Name;
        }
        /// <summary>
        /// Get the EnumMember value of a enum
        /// </summary>
        public static string ToEnumString<T>(this T type)
        {
            var enumType = typeof(T);
            var name = Enum.GetName(enumType, type);
            var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
            return enumMemberAttribute.Value;
        }
    }
}