using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Penneo.Connector;
using Penneo.Mapping;
using RestSharp;
using Penneo.Util;
using RestSharp.Contrib;

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

        /// <summary>
        /// Find entities based on an input object.
        /// Returns output with data and metadata
        /// </summary>
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
            output.Input = input;

            IEnumerable<T> objects;
            IRestResponse response;
            output.Success = ApiConnector.Instance.FindBy(query, out objects, out response, input.Page, input.PerPage);
            output.Objects = objects;
            output.StatusCode = response.StatusCode;
            output.ErrorMessage = response.ErrorMessage;

            if (output.Success)
            {
                //Map objects
                Func<object, object> processor;
                if (_postProcessors.TryGetValue(typeof (T), out processor))
                {
                    output.Objects = (IEnumerable<T>) processor(output.Objects);
                }

                //Pagination
                output.Page = input.Page;
                output.PerPage = input.PerPage;
                var linkHeader = response.Headers.FirstOrDefault(x => x.Name.Equals("link", StringComparison.OrdinalIgnoreCase));
                if (linkHeader != null && linkHeader.Value != null)
                {
                    PaginationUtil.ParseRepsonseHeadersForPagination(linkHeader.Value.ToString(), output);
                }
                

                //If no objects were returned, assume that there is no next page - regardless of any returned Link response header
                if (objects == null || !objects.Any())
                {
                    output.NextPage = null;
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

        /// <summary>
        /// Get the default message template
        /// </summary>
        public static QueryResult<MessageTemplate> GetDefaultMessageTemplate()
        {
            var resource = ServiceLocator.Instance.GetInstance<RestResources>().GetResource(typeof(MessageTemplate)) + "/default";
            var result = ApiConnector.Instance.CallServer(resource);
            var obj = JsonConvert.DeserializeObject<MessageTemplate>(result.Content);
            obj.Title = "Standard";
            return new QueryResult<MessageTemplate>(){ Objects = new []{ obj }, Success = true };
        }

        /// <summary>
        /// Get the current user
        /// </summary>
        public static QuerySingleObjectResult<User> GetUser()
        {
            IRestResponse response;
            var user = ApiConnector.Instance.ReadObject<User>(null, null, "user", out response);
            return CreateSingleObjectResult(response, user, null);
        }

        private static QuerySingleObjectResult<T> CreateSingleObjectResult<T>(IRestResponse response, T obj, int? id)
            where T: Entity
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

        /// <summary>
        /// Get the next page based on an earlier result
        /// </summary>
        public static QueryResult<T> GetNextPage<T>(QueryResult<T> result)
            where T: Entity
        {
            Log.Write("GetNextPage (" + typeof(T).Name + ")", LogSeverity.Information);
            if (!result.NextPage.HasValue)
            {
                return null;
            }
            return GetPage(result, result.NextPage);
        }

        /// <summary>
        /// Get the previous page based on an earlier result
        /// </summary>
        public static QueryResult<T> GetPreviousPage<T>(QueryResult<T> result)
            where T : Entity
        {
            Log.Write("GetPreviousPage (" + typeof(T).Name + ")", LogSeverity.Information);
            if (!result.PrevPage.HasValue)
            {
                return null;
            }
            return GetPage(result, result.PrevPage);
        }

        /// <summary>
        /// Get the first page based on an earlier result
        /// </summary>
        public static QueryResult<T> GetFirstPage<T>(QueryResult<T> result)
            where T : Entity
        {
            Log.Write("GetFirstPage (" + typeof(T).Name + ")", LogSeverity.Information);
            if (!result.FirstPage.HasValue)
            {
                return null;
            }
            return GetPage(result, result.FirstPage);
        }

        /// <summary>
        /// Get the first page of a given entity
        /// </summary>
        public static QueryResult<T> GetFirstPage<T>(int? perPage = null)
            where T : Entity
        {
            Log.Write("GetFirstPage (" + typeof(T).Name + ")", LogSeverity.Information);
            var input = new QueryInput();
            input.Page = 1;
            input.PerPage = perPage;
            return FindBy<T>(input);
        }

        private static void ThrowIfNoPagination<T>(QueryResult<T> result, int? page)
            where T : Entity
        {
            if (!page.HasValue || result.Input == null || !result.Input.Page.HasValue)
            {
                throw new NotSupportedException("Cannot use pagination, since the previous request was not using pagination");
            }
        }

        private static QueryResult<T> GetPage<T>(QueryResult<T> result, int? page)
            where T : Entity
        {
            ThrowIfNoPagination(result, page);
            var pageInput = (QueryInput)result.Input.Clone();
            pageInput.Page = page;
            return FindBy<T>(pageInput);
        }
    }
}