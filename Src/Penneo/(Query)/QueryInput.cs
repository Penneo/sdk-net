using System.Collections.Generic;

namespace Penneo
{
    public class QueryInput
    {
        public int Id { get; set; }
        public Dictionary<string, object> Criteria { get; set; }
        public Dictionary<string, string> OrderBy { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }

        public void AddCriteria(string key, object value)
        {
            if (Criteria == null)
            {
                Criteria = new Dictionary<string, object>();
            }
            Criteria.Add(key, value);
        }

        public void AddOrderBy(string key, string column)
        {
            if (OrderBy == null)
            {
                OrderBy = new Dictionary<string, string>();
            }
            OrderBy.Add(key, column);
        }
    }
}
