using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Penneo.Connector;
using RestSharp;
using Penneo.Util;

namespace Penneo
{
    /// <summary>
    /// Create queries against Penneo
    /// </summary>
    public class Query
    {
        private readonly PenneoConnector _con;

        public Query(PenneoConnector con)
        {
            _con = con;
        }

        /// <summary>
        /// Get an entity by its Id
        /// </summary>
        public async Task<T> Find<T>(int id)
            where T : Entity
        {
            var output = await FindById<T>(id);
            if (!output.Success)
            {
                throw new Exception(output.ErrorMessage);
            }
            return output.Object;
        }

        public async Task<QuerySingleObjectResult<T>> FindById<T>(int id)
            where T : Entity
        {
            RestResponse response;
            var objectResult = await _con.ApiConnector.ReadObject<T>(null, id);
            return CreateSingleObjectResult(objectResult.Response, objectResult.Result, id);
        }

        /// <summary>
        /// Get the first entity matching the search criteria
        /// </summary>
        public async Task<T> FindOneBy<T>(Dictionary<string, object> criteria = null, Dictionary<string, string> orderBy = null)
            where T : Entity
        {
            var input = new QueryInput { Criteria = criteria, OrderBy = orderBy };
            return (await FindOneBy<T>(input)).Object;
        }

        public async Task<QuerySingleObjectResult<T>> FindOneBy<T>(QueryInput input)
            where T : Entity
        {
            _con.Log("FindOneBy (" + typeof(T).Name + ")", LogSeverity.Information);
            var result = new QuerySingleObjectResult<T>(await FindBy<T>(input));
            return result;
        }

        /// <summary>
        /// Get all entities of the given type
        /// </summary>
        public async Task<IEnumerable<T>> FindAll<T>()
            where T : Entity
        {
            _con.Log("FindAll (" + typeof(T).Name + ")", LogSeverity.Information);
            var input = new QueryInput();
            return (await FindBy<T>(input)).Objects;
        }

        /// <summary>
        /// Get entities matching the search criteria
        /// </summary>
        public async Task<IEnumerable<T>> FindBy<T>(Dictionary<string, object> criteria = null, Dictionary<string, string> orderBy = null, int? perPage = null, int? page = null)
            where T : Entity
        {
            var input = new QueryInput();
            input.Criteria = criteria;
            input.OrderBy = orderBy;
            input.PerPage = perPage;
            input.Page = page;
            
            var output = await FindBy<T>(input);
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
        public async Task<QueryResult<T>> FindBy<T>(QueryInput input)
            where T : Entity
        {
            _con.Log("FindBy (" + typeof(T).Name + ")", LogSeverity.Information);

            var criteria = input.Criteria;
            var orderBy = input.OrderBy;
            var page = input.Page;
            var perPage = input.PerPage;

            UpdateCriteriaDateTimesToUnix(input);
            var query = criteria ?? new Dictionary<string, object>();

            if (perPage.HasValue)
            {
                query["per_page"] = perPage;
            }
            if (page.HasValue)
            {
                query["page"] = page;
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
            
            var findByResult = await _con.ApiConnector.FindBy<T>(query, input.Page, input.PerPage);

            output.Success = findByResult.Success;
            output.Objects = findByResult.Objects;
            output.StatusCode = findByResult.Response.StatusCode;
            output.ErrorMessage = findByResult.Response.ErrorMessage;

            if (output.Success)
            {
                //Map objects
                var postProcessor = _con.Setup.GetPostProcessor(typeof(T));
                if (postProcessor != null)
                {
                    output.Objects = (IEnumerable<T>)postProcessor(output.Objects);
                }

                //Pagination
                output.Page = input.Page;
                output.PerPage = input.PerPage;
                if (null != findByResult.Response.Headers)
                {
                    var linkHeader =
                        findByResult.Response.Headers.FirstOrDefault(x => x.Name.Equals("link", StringComparison.OrdinalIgnoreCase));
                    if (linkHeader != null && linkHeader.Value != null)
                    {
                        PaginationUtil.ParseRepsonseHeadersForPagination(linkHeader.Value.ToString(), output);
                    }
                }

                //If no objects were returned, assume that there is no next page - regardless of any returned Link response header
                if (findByResult.Objects == null || !findByResult.Objects.Any())
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
            foreach (var entry in criteria)
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
        /// <param name="messageTemplateType">The default message template type. Defaults to signing request</param>
        /// <param name="isoLanguage">The language iso code for the message template as two letter isocode. Defaults to users language</param>
        public async Task<QueryResult<MessageTemplate>> GetDefaultMessageTemplate(MessageTemplate.MessageTemplateType messageTemplateType = MessageTemplate.MessageTemplateType.SigningRequest, string isoLanguage = null)
        {
            var resource = _con.ServiceLocator.GetInstance<RestResources>().GetResource(typeof(MessageTemplate)) + $"/default?type={messageTemplateType.ToEnumString()}";
            if (!string.IsNullOrEmpty(isoLanguage))
            {
                resource += $"&language={isoLanguage}";
            }
            var result = await _con.ApiConnector.CallServer(resource);
            var obj = JsonConvert.DeserializeObject<MessageTemplate>(result.Content);
            obj.Title = "Standard";
            return new QueryResult<MessageTemplate>() { Objects = new[] { obj }, Success = true };
        }

        /// <summary>
        /// Get the current user
        /// </summary>
        public async Task<QuerySingleObjectResult<User>> GetUser()
        {
            RestResponse response;
            var objectResult = await _con.ApiConnector.ReadObject<User>(null, null, "user");
            return CreateSingleObjectResult(objectResult.Response, objectResult.Result, null);
        }

        private QuerySingleObjectResult<T> CreateSingleObjectResult<T>(RestResponse response, T obj, int? id)
            where T : Entity
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
        public async Task<QueryResult<T>> GetNextPage<T>(QueryResult<T> result)
            where T : Entity
        {
            _con.Log("GetNextPage (" + typeof(T).Name + ")", LogSeverity.Information);
            if (!result.NextPage.HasValue)
            {
                return null;
            }
            return await GetPage(result, result.NextPage);
        }

        /// <summary>
        /// Get the previous page based on an earlier result
        /// </summary>
        public async Task<QueryResult<T>> GetPreviousPage<T>(QueryResult<T> result)
            where T : Entity
        {
            _con.Log("GetPreviousPage (" + typeof(T).Name + ")", LogSeverity.Information);
            if (!result.PrevPage.HasValue)
            {
                return null;
            }
            return await GetPage(result, result.PrevPage);
        }

        /// <summary>
        /// Get the first page based on an earlier result
        /// </summary>
        public async Task<QueryResult<T>> GetFirstPage<T>(QueryResult<T> result)
            where T : Entity
        {
            _con.Log("GetFirstPage (" + typeof(T).Name + ")", LogSeverity.Information);
            if (!result.FirstPage.HasValue)
            {
                return null;
            }
            return await GetPage(result, result.FirstPage);
        }

        /// <summary>
        /// Get the first page of a given entity
        /// </summary>
        public async Task<QueryResult<T>> GetFirstPage<T>(int? perPage = null)
            where T : Entity
        {
            _con.Log("GetFirstPage (" + typeof(T).Name + ")", LogSeverity.Information);
            var input = new QueryInput();
            input.Page = 1;
            input.PerPage = perPage;
            return await FindBy<T>(input);
        }

        private void ThrowIfNoPagination<T>(QueryResult<T> result, int? page)
            where T : Entity
        {
            if (!page.HasValue || result.Input == null || !result.Input.Page.HasValue)
            {
                throw new NotSupportedException("Cannot use pagination, since the previous request was not using pagination");
            }
        }

        private async Task<QueryResult<T>> GetPage<T>(QueryResult<T> result, int? page)
            where T : Entity
        {
            ThrowIfNoPagination(result, page);
            var pageInput = (QueryInput)result.Input.Clone();
            pageInput.Page = page;
            return await FindBy<T>(pageInput);
        }
    }
}
