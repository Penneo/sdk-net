using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Penneo.Util
{
    public static class PaginationUtil
    {
        /// <summary>
        /// Parses a pagination Link header for next page, previous page, first page and number of objects per page
        /// Updates the given query result with the parse result
        /// </summary>
        public static void ParseRepsonseHeadersForPagination<T>(string linkHeader, QueryResult<T> output)
            where T : Entity
        {
            var relations = linkHeader.ToString().Split(',');
            foreach (var relation in relations)
            {
                var lastIndex = relation.LastIndexOf(";", StringComparison.OrdinalIgnoreCase);
                var relName = Regex.Match(relation, "rel=\"(?<rel>.*)\"").Groups[1].Value;
                var relUrl = relation.Substring(0, lastIndex - 1);
                var parameters = ParseQueryString(relUrl.Substring(relUrl.IndexOf("?", StringComparison.OrdinalIgnoreCase)));
                var page = int.Parse(parameters["page"]);

                switch (relName)
                {
                    case "next":
                        output.NextPage = page;
                        break;
                    case "prev":
                        output.PrevPage = page;
                        break;
                    case "first":
                        output.FirstPage = page;
                        break;
                }

                var perPage = int.Parse(parameters["per_page"]);
                output.PerPage = perPage;
            }
        }

        private static IDictionary<string, string> ParseQueryString(this string query)
        {
            // [DC]: This method does not URL decode, and cannot handle decoded input
            if (query.StartsWith("?"))
                query = query.Substring(1);

            if (query.Equals(string.Empty))
                return new Dictionary<string, string>();

            var parts = query.Split('&');

            return parts.Select(part => part.Split('='))
                .ToDictionary(pair => pair[0], pair => pair[1]);
        }
    }
}
