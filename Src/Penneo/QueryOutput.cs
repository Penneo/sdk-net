using System.Collections.Generic;
using System.Linq;

namespace Penneo
{
    public abstract class QueryOutputBase
    {
        public bool Success { get; set; }
        public Error Errors { get; set; }
    }

    public class QueryOutput<T> : QueryOutputBase
    {
        public IEnumerable<T> Objects { get; set; }
    }

    public class QuerySingleObjectOutput<T> : QueryOutputBase
    {
        public QuerySingleObjectOutput()
        {
        }

        public QuerySingleObjectOutput(QueryOutput<T> output)
        {
            Success = output.Success;
            Errors = output.Errors;
            if (output.Objects != null)
            {
                Object = output.Objects.FirstOrDefault();
            }
        }

        public T Object { get; set; }
    }
}
