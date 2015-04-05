using System;
using System.Collections.Generic;
using System.Linq;
using Penneo.Connector;
using RestSharp;

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
                throw new Exception(output.ErrorMessage);
            }
            return output.Object;
        }

        public static QuerySingleObjectResult<T> FindById<T>(int id)
            where T : Entity
        {
            var obj = Activator.CreateInstance<T>();
            obj.Id = id;

            var output = new QuerySingleObjectResult<T>();
            IRestResponse response;
            output.Success = ApiConnector.Instance.ReadObject(obj, out response);
            output.StatusCode = response.StatusCode;
            output.ErrorMessage = response.ErrorMessage;
            output.Object = obj;
            if (!output.Success)
            {
                output.ErrorMessage = "Penneo: Could not find the requested " + typeof (T).Name + " (id = " + id + ")";
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

        public static QuerySingleObjectResult<T> FindOneBy<T>(QueryInput input)
            where T : Entity
        {
            Log.Write("FindOneBy (" + typeof(T).Name + ")", LogSeverity.Information);
            var result = new QuerySingleObjectResult<T>(FindBy<T>(input));
            return result;
        }

        /// <summary>
        /// Get all entities of the given type
        /// </summary>
        public static IEnumerable<T> FindAll<T>()
            where T : Entity
        {
            Log.Write("FindAll (" + typeof(T).Name + ")", LogSeverity.Information);
            var input = new QueryInput();
            return FindBy<T>(input).Objects;
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
                if (!string.IsNullOrEmpty(output.ErrorMessage))
                {
                    throw new Exception(output.ErrorMessage);
                }
                throw new Exception("Unknown error during FindBy");
            }
            return output.Objects;
        }

        public static QueryResult<T> FindBy<T>(QueryInput input)
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

            var output = new QueryResult<T>();

            IEnumerable<T> objects;
            IRestResponse response;
            output.Success = ApiConnector.Instance.FindBy(query, out objects, out response);
            output.Objects = objects;
            output.StatusCode = response.StatusCode;
            output.ErrorMessage = response.ErrorMessage;
            return output;
        }
    }
}