using System;
using System.Collections.Generic;

namespace Penneo
{
    public class QueryInput : ICloneable
    {
        public int Id { get; set; }
        public Dictionary<string, object> Criteria { get; set; }
        public Dictionary<string, string> OrderBy { get; set; }
        [Obsolete("Use Page and PerPage")]
        public int? Limit { get; set; }
        [Obsolete("Use Page and PerPage")]
        public int? Offset { get; set; }
        public int? Page { get; set; }
        public int? PerPage { get; set; }

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

        public object Clone()
        {
            var clone = new QueryInput()
            {
                Id = Id,
                Criteria = Criteria,
                OrderBy = OrderBy,
                Limit = Limit,
                Offset = Offset,
                Page = Page,
                PerPage = PerPage
            };
            return clone;
        }
    }
}
