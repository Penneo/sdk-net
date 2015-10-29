using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Penneo.Connector;
using Penneo.Mapping;
using RestSharp;
using Penneo.Util;

namespace Penneo
{
    /// <summary>
    /// Create queries against Penneo
    /// </summary>
    public static class Query
    {
        private static Dictionary<Type, Func<object, object>> _postProcessors = new Dictionary<Type, Func<object, object>>(); 

        internal static void AddPostProcessor<T>(Func<object, object> processor)
        {
            _postProcessors[typeof (T)] = processor;
        }

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
            IRestResponse response;
            var obj = ApiConnector.Instance.ReadObject<T>(null, id, out response);
            return CreateSingleObjectResult(response, obj, id);
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

            UpdateCriteriaDateTimesToUnix(input);
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
            
            if (output.Success)
            {
                Func<object, object> processor;
                if (_postProcessors.TryGetValue(typeof (T), out processor))
                {
                    output.Objects = (IEnumerable<T>) processor(output.Objects);
                }
            }
            return output;
        }

        private static void UpdateCriteriaDateTimesToUnix(QueryInput input)
        {
            if (input.Criteria == null)
            {
                return;
            }
            var criteria = input.Criteria.Where(x => x.Value != null && x.Value.GetType() == typeof(DateTime)).ToList();
            foreach(var entry in criteria)
            {
                var key = entry.Key;
                var val = entry.Value;
                var newVal = TimeUtil.ToUnixTime((DateTime)val);
                input.Criteria[key] = newVal;
            }
        }

        public static QueryResult<MessageTemplate> GetDefaultMessageTemplate()
        {
            var resource = ServiceLocator.Instance.GetInstance<RestResources>().GetResource(typeof(MessageTemplate)) + "/default";
            var result = ApiConnector.Instance.CallServer(resource);
            var obj = JsonConvert.DeserializeObject<MessageTemplate>(result.Content);
            obj.Title = "Standard";
            return new QueryResult<MessageTemplate>(){ Objects = new []{ obj }, Success = true };
        }

        public static QuerySingleObjectResult<User> GetUser()
        {
            IRestResponse response;
            var user = ApiConnector.Instance.ReadObject<User>(null, null, "user", out response);
            return CreateSingleObjectResult(response, user, null);
        }

        private static QuerySingleObjectResult<T> CreateSingleObjectResult<T>(IRestResponse response, T obj, int? id)
        {
            var output = new QuerySingleObjectResult<T>
            {
                Success = obj != null,
                StatusCode = response.StatusCode,
                ErrorMessage = response.ErrorMessage,
                Object = obj
            };
            if (!output.Success)
            {
                output.ErrorMessage = "Penneo: Could not find the requested " + typeof(T).Name;
                if (id.HasValue)
                {
                    output.ErrorMessage += " (id = " + id + ")";
                }
            }
            return output;
        }

    }
}