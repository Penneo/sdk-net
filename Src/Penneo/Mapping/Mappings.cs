using System;
using System.Collections.Generic;

namespace Penneo.Mapping
{
    /// <summary>
    /// Container for mappings from type to request data
    /// </summary>
    internal class Mappings
    {
        private readonly Dictionary<Type, IMapping> _mappings;

        public Mappings()
        {
            _mappings = new Dictionary<Type, IMapping>();
        }

        /// <summary>
        /// Adds a mapping to the container
        /// </summary>
        public void AddMapping(IMapping m)
        {
            _mappings[m.Type] = m;
        }

        /// <summary>
        /// Gets a mapping for a given type from the container
        /// </summary>
        public IMapping GetMapping(Type t)
        {
            IMapping m;
            if (!_mappings.TryGetValue(t, out m))
            {
                return null;
            }
            return m;
        }
    }
}