using System;
using System.Text.RegularExpressions;
using System.Web;

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
                var parameters = HttpUtility.ParseQueryString(relUrl.Substring(relUrl.IndexOf("?", StringComparison.OrdinalIgnoreCase)));
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
    }
}
