using System;
using System.Collections.Generic;

namespace Penneo.Connector
{
    /// <summary>
    /// Stores resources/urls for REST
    /// </summary>
    internal class RestResources
    {
        private readonly Dictionary<Type, string> _resources;

        public RestResources()
        {
            _resources = new Dictionary<Type, string>();
        }

        /// <summary>
        /// Adds a rest resource to a given type
        /// </summary>
        public void Add<T>(string resource)
        {
            _resources[typeof (T)] = resource;
        }

        /// <summary>
        /// Get a resource for a given type and parent
        /// </summary>
        public string GetResource<T>(Entity parent = null)
        {
            return GetResource(typeof (T), parent);
        }

        /// <summary>
        /// Get a resource for a given type and parent
        /// </summary>
        public string GetResource(Type type, Entity parent = null)
        {
            string r;
            if (!_resources.TryGetValue(type, out r))
            {
                throw new IndexOutOfRangeException("Resource not found");
            }
            if (parent == null)
            {
                return r;
            }
            return GetResource(parent.GetType()) + "/" + parent.Id + "/" + r;
        }
    }
}