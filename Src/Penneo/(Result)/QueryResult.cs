using System.Collections.Generic;
using System.Linq;

namespace Penneo
{
    /// <summary>
    /// Query result with multiple fetched objects
    /// </summary>
    public class QueryResult<T> : ServerResult
        where T: Entity
    {
        /// <summary>
        /// The objects fetched by the executed query
        /// </summary>
        public IEnumerable<T> Objects { get; set; }

        /// <summary>
        /// For a query using pagination, this property contains the page number fetched
        /// </summary>
        public int? Page { get; set; }
        /// <summary>
        /// For a query using pagination, this property contains the number of objects per page
        /// </summary>
        public int? PerPage { get; set; }
        /// <summary>
        /// For a query using pagination, this property contains the next page number
        /// </summary>
        public int? NextPage { get; set; }
        /// <summary>
        /// For a query using pagination, this property contains the previous page number
        /// </summary>
        public int? PrevPage { get; set; }
        /// <summary>
        /// For a query using pagination, this property contains the first page number
        /// </summary>
        public int? FirstPage { get; set; }

        /// <summary>
        /// Query input is stored to facilitate queries with pagination (next page, prev page, etc.)
        /// </summary>
        public QueryInput Input { get; set; }
    }

    /// <summary>
    /// Query result for a single object
    /// </summary>
    public class QuerySingleObjectResult<T> : ServerResult
        where T: Entity
    {
        /// <summary>
        /// Create an empty result
        /// </summary>
        public QuerySingleObjectResult()
        {
        }

        /// <summary>
        /// Create a single object result from a generic query result
        /// </summary>
        public QuerySingleObjectResult(QueryResult<T> output)
        {
            Success = output.Success;
            if (output.Objects != null)
            {
                Object = output.Objects.FirstOrDefault();
            }
        }

        /// <summary>
        /// The single object fetched
        /// </summary>
        public T Object { get; set; }
    }
}
