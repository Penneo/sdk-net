using System;
using System.Linq;
using NUnit.Framework;
using Penneo;
using Penneo.Util;

namespace PenneoTests
{
    [TestFixture]
    public class PaginationTests
    {
        [Test]
        public void ParseLinkResponseHeaderTest()
        {
            var queryResult = new QueryResult<CaseFile>();
            const string linkHeader = "Link=<https://app.penneo.com/api/v1/casefiles?page=17&per_page=10>; rel=\"next\",<https://app.penneo.com/api/v1/casefiles?page=1&per_page=10>; rel=\"first\",<https://app.penneo.com/api/v1/casefiles?page=15&per_page=10>; rel=\"prev\"";
            PaginationUtil.ParseResponseHeadersForPagination(linkHeader, queryResult);
            Assert.That(queryResult.NextPage, Is.EqualTo(17));
            Assert.That(queryResult.PrevPage, Is.EqualTo(15));
            Assert.That(queryResult.FirstPage, Is.EqualTo(1));
        }

        [Test]
        public void PageNotZeroTest()
        {
          Assert.That(() => TestUtil.CreateTestApiConnector().PrepareRequest(string.Empty, page: 0, perPage: 10),
                Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void PerPageNotZeroTest()
        {
          Assert.That(() => TestUtil.CreateTestApiConnector().PrepareRequest(string.Empty, page: 5, perPage: 0),
                Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void PageNotLessThanZeroTest()
        {
          Assert.That(() => TestUtil.CreateTestApiConnector().PrepareRequest(string.Empty, page: -2, perPage: 10),
                Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void PerPageNotLessThanZeroTest()
        {
          Assert.That(() => TestUtil.CreateTestApiConnector().PrepareRequest(string.Empty, page: 5, perPage: -5),
                Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void PaginateRequestParametersTest()
        {
            var connector = TestUtil.CreateTestApiConnector();
            var request = connector.PrepareRequest(string.Empty, page: 5, perPage: 10);

            var paginationHeader = request.Parameters.FirstOrDefault(x => x.Name.Equals("x-paginate"));
            Assert.That(paginationHeader != null && paginationHeader.Value.ToString().Equals("true", StringComparison.OrdinalIgnoreCase), Is.True);

            var pageParameter = request.Parameters.FirstOrDefault(x => x.Name.Equals("page"));
            Assert.That(pageParameter != null && pageParameter.Value.ToString() == "5", Is.True);

            var perPageParameter = request.Parameters.FirstOrDefault(x => x.Name.Equals("per_page"));
            Assert.That(perPageParameter != null && perPageParameter.Value.ToString() == "10", Is.True);
        }

        [Test]
        public void NoPaginateRequestParametersTest()
        {
            var connector = TestUtil.CreateTestApiConnector();
            var request = connector.PrepareRequest(string.Empty);

            var paginationHeader = request.Parameters.FirstOrDefault(x => x.Name.Equals("x-paginate"));
            Assert.That(paginationHeader, Is.Null);

            var pageParameter = request.Parameters.FirstOrDefault(x => x.Name.Equals("page"));
            Assert.That(pageParameter, Is.Null);

            var perPageParameter = request.Parameters.FirstOrDefault(x => x.Name.Equals("per_page"));
            Assert.That(perPageParameter, Is.Null);
        }
    }
}
