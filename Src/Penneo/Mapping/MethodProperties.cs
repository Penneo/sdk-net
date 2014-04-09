using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Penneo.Util;

namespace Penneo.Mapping
{
    /// <summary>
    /// Properties for a mapping
    /// </summary>
    internal class MethodProperties<T>
        where T : Entity
    {
        private readonly HashSet<string> _isFile;
        private readonly Dictionary<string, Func<T, object>> _properties;

        public MethodProperties()
        {
            _properties = new Dictionary<string, Func<T, object>>();
            _isFile = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Adds a property to the container
        /// </summary>      
        public void AddProperty(Expression<Func<T, object>> property, string alias = null)
        {
            var name = alias ?? ReflectionUtil.GetPropertyName(property);
            _properties[name] = property.Compile();
        }

        /// <summary>
        /// Adds a file property to the container
        /// </summary>
        public void AddFileProperty(Expression<Func<T, string>> property, string alias = null)
        {
            var name = alias ?? ReflectionUtil.GetPropertyName(property);
            _properties[name] = property.Compile();
            _isFile.Add(name);
        }

        /// <summary>
        /// Gets property values as a key/value dictionary
        /// </summary>        
        public Dictionary<string, object> GetValues(T obj)
        {
            var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var p in _properties)
            {
                var propertyName = StringUtil.FirstCharacterToLower(p.Key);
                var f = p.Value;
                object v = null;
                try
                {
                    v = f(obj);
                }
                catch (NullReferenceException)
                {
                    //object in path not set, ignore value
                }
                if (v == null)
                {
                    continue;
                }
                var value = _isFile.Contains(propertyName) ? FileUtil.GetBase64((string) v) : v;
                values[propertyName] = value;
            }
            return values;
        }
    }
}