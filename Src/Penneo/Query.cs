using System;
using System.Collections.Generic;
using System.Linq;
using Penneo.Connector;

namespace Penneo
{
    /// <summary>
    /// Create queries against Penneo
    /// </summary>
    public static class Query
    {
        /// <summary>
        /// Get an entity by its Id
        /// </summary>
        public static T Find<T>(int id)
            where T : Entity
        {
            var obj = Activator.CreateInstance<T>();
            obj.Id = id;
            if (!ApiConnector.Instance.ReadObject(obj))
            {
                throw new Exception("Penneo: Could not find the requested " + typeof (T).Name + " (id = " + id + ")");
            }
            return obj;
        }

        /// <summary>
        /// Get the first entity matching the search criteria
        /// </summary>
        public static T FindOneBy<T>(Dictionary<string, object> criteria = null, Dictionary<string, string> orderBy = null)
            where T : Entity
        {
            Log.Write("FindOneBy (" + typeof (T).Name + ")", LogSeverity.Information);
            return FindBy<T>(criteria, orderBy).FirstOrDefault();
        }

        /// <summary>
        /// Get all entities of the given type
        /// </summary>        
        public static IEnumerable<T> FindAll<T>()
            where T : Entity
        {
            Log.Write("FindAll (" + typeof (T).Name + ")", LogSeverity.Information);
            return FindBy<T>();
        }

        /// <summary>
        /// Get entities matching the search criteria
        /// </summary>
        public static IEnumerable<T> FindBy<T>(Dictionary<string, object> criteria = null, Dictionary<string, string> orderBy = null, int? limit = null, int? offset = null)
            where T : Entity
        {
            Log.Write("FindBy (" + typeof (T).Name + ")", LogSeverity.Information);

            var query = criteria ?? new Dictionary<string, object>();

            if (limit.HasValue)
            {
                query["limit"] = limit;
            }
            if (offset.HasValue)
            {
                query["offset"] = offset;
            }

            if (orderBy != null)
            {
                var sort = string.Join(",", orderBy.Keys);
                var order = string.Join(",", orderBy.Values);
                query["sort"] = sort;
                query["order"] = order;
            }

            IEnumerable<T> result;
            if (!ApiConnector.Instance.FindBy(query, out result))
            {
                throw new Exception("Penneo: Internal problem encountered");
            }
            return result;
        }
    }
}