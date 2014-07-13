﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;

namespace Penneo.Util
{
    /// <summary>
    /// Reflection utilities
    /// </summary>
    internal static class ReflectionUtil
    {
        /// <summary>
        /// Sets properties defined in the given json on the given object.
        /// </summary>
        public static void SetPropertiesFromJson(object obj, string json)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            SetPropertiesFromDictionary(obj, values);
        }

        /// <summary>
        /// Sets properties defined in the given dictionary on the given object.
        /// </summary>
        public static void SetPropertiesFromDictionary(object obj, Dictionary<string, object> values)
        {
            foreach (var entry in values)
            {
                var propertyName = entry.Key;
                var value = entry.Value;
                var propInfo = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (propInfo != null && propInfo.CanWrite)
                {
                    if (value is long && propInfo.PropertyType == typeof(int) || propInfo.PropertyType == typeof(int?))
                    {                        
                        value = Convert.ToInt32(value);
                    }

                    propInfo.SetValue(obj, ConvertToType(propInfo.PropertyType, value), null);
                }
            }
        }

        /// <summary>
        /// Converts a given value to the given type
        /// </summary>
        private static object ConvertToType(Type type, object value)
        {
            if (type == typeof (bool))
            {
                return Convert.ToBoolean(value);
            }
            if (type == typeof (DateTime) || type == typeof(DateTime?))
            {
                return TimeUtil.FromUnixTime(Convert.ToInt64(value));
            }
            return value;
        }

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
        public static string GetPropertyName<T>(Expression<Func<T, string>> property)
        {
            return ((MemberExpression) property.Body).Member.Name;
        }
    }
}