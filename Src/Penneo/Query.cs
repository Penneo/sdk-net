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
            var output = FindById<T>(id);
            if (!output.Success)
            {
                throw new Exception(output.Errors.ErrorMessage);
            }
            return output.Object;
        }

        public static QuerySingleObjectOutput<T> FindById<T>(int id)
            where T : Entity
        {
            var obj = Activator.CreateInstance<T>();
            obj.Id = id;

            var output = new QuerySingleObjectOutput<T>();
            Error error;
            output.Success = ApiConnector.Instance.ReadObject(obj, out error);
            output.Errors = error;
            output.Object = obj;
            if (!output.Success)
            {
                output.Errors.ErrorMessage = "Penneo: Could not find the requested " + typeof (T).Name + " (id = " + id + ")";
            }
            return output;
        }

        /// <summary>
        /// Get the first entity matching the search criteria
        /// </summary>
        public static T FindOneBy<T>(Dictionary<string, object> criteria = null, Dictionary<string, string> orderBy = null)
            where T : Entity
        {
            var input = new QueryInput {Criteria = criteria, OrderBy = orderBy};
            return FindOneBy<T>(input).Object;
        }

        public static QuerySingleObjectOutput<T> FindOneBy<T>(QueryInput input)
            where T : Entity
        {
            Log.Write("FindOneBy (" + typeof(T).Name + ")", LogSeverity.Information);
            var result = new QuerySingleObjectOutput<T>(FindBy<T>(input));
            return result;
        }

        /// <summary>
        /// Get all entities of the given type
        /// </summary>
        public static IEnumerable<T> FindAll<T>()
            where T : Entity
        {
            var input = new QueryInput();
            return FindAll<T>(input).Objects;
        }

        public static QueryOutput<T> FindAll<T>(QueryInput input)
            where T : Entity
        {
            Log.Write("FindAll (" + typeof(T).Name + ")", LogSeverity.Information);
            if (input.Criteria != null && input.Criteria.Any())
            {
                throw new ArgumentException("Criteria must be empty in query FindAll");
            }
            return FindBy<T>(input);
        }

        /// <summary>
        /// Get entities matching the search criteria
        /// </summary>
        public static IEnumerable<T> FindBy<T>(Dictionary<string, object> criteria = null, Dictionary<string, string> orderBy = null, int? limit = null, int? offset = null)
            where T : Entity
        {
            var input = new QueryInput();
            input.Criteria = criteria;
            input.OrderBy = orderBy;
            input.Limit = limit;
            input.Offset = offset;

            var output = FindBy<T>(input);
            if (!output.Success)
            {
                if (output.Errors != null)
                {
                    throw new Exception(output.Errors.ErrorMessage);
                }
                throw new Exception("Unknown error during FindBy");
            }
            return output.Objects;
        }

        public static QueryOutput<T> FindBy<T>(QueryInput input)
            where T : Entity
        {
            Log.Write("FindBy (" + typeof(T).Name + ")", LogSeverity.Information);

            var criteria = input.Criteria;
            var orderBy = input.OrderBy;
            var offset = input.Offset;
            var limit = input.Limit;

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

            var output = new QueryOutput<T>();

            IEnumerable<T> objects;
            Error error;
            output.Success = ApiConnector.Instance.FindBy(query, out objects, out error);
            output.Objects = objects;
            output.Errors = error;
            return output;
        }
    }
}