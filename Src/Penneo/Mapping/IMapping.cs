using System;
using System.Collections.Generic;

namespace Penneo.Mapping
{
    /// <summary>
    /// Mapping from type to request values
    /// </summary>
    internal interface IMapping
    {
        /// <summary>
        /// The data type
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Gets request values for 'create'
        /// </summary>        
        Dictionary<string, object> GetCreateValues(object obj);

        /// <summary>
        /// Gets request values for 'update'
        /// </summary>
        Dictionary<string, object> GetUpdateValues(object obj);
    }
}