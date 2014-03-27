using System;
using System.Collections.Generic;

namespace Penneo.Mapping
{
    /// <summary>
    /// Mapping for type T to request values
    /// <see cref="IMapping"/>
    /// </summary>
    internal class Mapping<T> : IMapping
        where T : Entity
    {
        private readonly Dictionary<string, MethodProperties<T>> _mapping;

        public Mapping()
        {
            _mapping = new Dictionary<string, MethodProperties<T>>();
        }

        #region IMapping Members

        /// <summary>
        /// <see cref="IMapping.GetCreateValues"/>
        /// </summary>
        public Dictionary<string, object> GetCreateValues(object obj)
        {
            return GetValues("create", (T) obj);
        }

        /// <summary>
        /// <see cref="IMapping.GetUpdateValues"/>
        /// </summary>
        public Dictionary<string, object> GetUpdateValues(object obj)
        {
            return GetValues("update", (T) obj);
        }

        /// <summary>
        /// <see cref="IMapping.Type"/>
        /// </summary>
        public Type Type
        {
            get { return typeof (T); }
        }

        #endregion

        /// <summary>
        /// Adds a type mapping
        /// </summary>
        internal void AddMapping(string method, MethodProperties<T> properties)
        {
            _mapping[method] = properties;
        }

        /// <summary>
        /// Gets request values for a given method and object
        /// </summary>
        internal Dictionary<string, object> GetValues(string method, T obj)
        {
            MethodProperties<T> props;
            if (!_mapping.TryGetValue(method, out props))
            {
                throw new KeyNotFoundException("Method not defined");
            }
            return props.GetValues(obj);
        }
    }
}