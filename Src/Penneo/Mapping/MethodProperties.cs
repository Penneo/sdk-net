using System;
using System.CodeDom;
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
        private readonly Dictionary<string, Func<object, object>> _convert;
        private readonly HashSet<string> _isFile;
        private readonly HashSet<string> _isBase64;
        private readonly Dictionary<string, Func<T, object>> _properties;

        public MethodProperties()
        {
            _properties = new Dictionary<string, Func<T, object>>();
            _isFile = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _isBase64 = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _convert = new Dictionary<string, Func<object, object>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Adds a property to the container
        /// </summary>      
        public void AddProperty(Expression<Func<T, object>> property, string alias = null, Func<object, object> convert = null)
        {
            var name = alias ?? ReflectionUtil.GetPropertyName(property);
            _properties[name] = property.Compile();
            if (convert != null)
            {
                _convert[name] = convert;
            }
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

        public void AddBase64Property(Expression<Func<T, byte[]>> property, string alias = null)
        {
            var name = alias ?? ReflectionUtil.GetPropertyName(property);
            _properties[name] = property.Compile();
            _isBase64.Add(name);
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

                object value;
                if (_isFile.Contains(propertyName))
                {
                    value = FileUtil.GetBase64((string) v);
                }
                else if (_isBase64.Contains(propertyName))
                {
                    value = Convert.ToBase64String((byte[]) v);
                }
                else
                {
                    value = v;
                }
               
                //Convert value if converter is defined
                Func<object, object> convert;
                if (_convert.TryGetValue(propertyName, out convert))
                {
                    value = convert(value);
                }

                //Store value
                values[propertyName] = value;
            }
            return values;
        }
    }
}