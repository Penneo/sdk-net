using System.Collections.Generic;
using System.Linq;

namespace Penneo
{
    public class QueryResult<T> : ServerResult
    {
        public IEnumerable<T> Objects { get; set; }
    }

    public class QuerySingleObjectResult<T> : ServerResult
    {
        public QuerySingleObjectResult()
        {
        }

        public QuerySingleObjectResult(QueryResult<T> output)
        {
            Success = output.Success;
            if (output.Objects != null)
            {
                Object = output.Objects.FirstOrDefault();
            }
        }

        public T Object { get; set; }
    }
}
