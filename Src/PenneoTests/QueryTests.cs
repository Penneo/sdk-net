using System;
using System.Linq;
using System.Net;
using FakeItEasy;
using NUnit.Framework;
using Penneo;
using System.Collections.Generic;
using System.Threading.Tasks;
using Penneo.Connector;
using RestSharp;

namespace PenneoTests
{
    [TestFixture]
    public class QueryTests
    {
        private static RestResponse _response200 = new RestResponse { StatusCode = HttpStatusCode.OK };

        [Test]
        public async Task FindTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var expected = new CaseFile("Test") { Id = 1 };
            var readObjectResult = new ReadObjectResult<CaseFile>()
            {
                Response = new RestResponse(),
                Result = expected
            };
            A.CallTo(() => con.ApiConnector.ReadObjectAsync<CaseFile>(null, 1))
                .WithAnyArguments()
                .Returns(Task.FromResult(readObjectResult));

            var q = new Query(con);
            var obj = await q.FindAsync<CaseFile>(1);

            Assert.AreEqual(1, obj.Id);
            Assert.AreEqual(expected.Title, obj.Title);
            A.CallTo(() => con.ApiConnector.ReadObjectAsync<CaseFile>(null, 1)).WithAnyArguments().MustHaveHappened();
        }

        [Test]
        public void FindOneByTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var q = new Query(con);
            FindOneTest(con, () => q.FindOneByAsync<CaseFile>());
        }

        [Test]
        public void FindAllTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var q = new Query(con);
            FindCollectionTest(con, async () => await q.FindAllAsync<CaseFile>());
        }

        [Test]
        public void FindByTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var q = new Query(con);
            FindCollectionTest(con, async () => await q.FindByAsync<Document>(
                new Dictionary<string, object> { { "title", "the" } },
                new Dictionary<string, string>() { { "created", "desc" } },
                10, 5
            ));

        }

        private static async void FindCollectionTest<T>(PenneoConnector con, Func<Task<IEnumerable<T>>> f)
            where T : Entity
        {
            IEnumerable<T> returned = new[] { (T)Activator.CreateInstance(typeof(T)) };
            A.CallTo(() => con.ApiConnector.FindByAsync<T>(null, null, null)).WithAnyArguments().Returns(
                Task.FromResult(new FindByResult<T> { Success = true, Objects = returned, Response = _response200 }));

            var objects = await f();

            Assert.IsNotNull(objects);
            CollectionAssert.AreEqual(returned.ToList(), objects.ToList());

            A.CallTo(() => con.ApiConnector.FindByAsync<T>(null, null, null)).WithAnyArguments().MustHaveHappened();
        }

        private static async Task FindOneTest<T>(PenneoConnector con, Func<Task<T>> f)
            where T : Entity
        {
            var instance = (T)Activator.CreateInstance(typeof(T));
            IEnumerable<T> returned = new[] { instance };
            A.CallTo(() => con.ApiConnector.FindByAsync<T>(null, null, null)).WithAnyArguments()
                .Returns(Task.FromResult(new FindByResult<T>
                    { Success = true, Objects = returned, Response = _response200 }))
                .AssignsOutAndRefParameters(returned, _response200);

            var obj = await f();

            Assert.IsNotNull(obj);
            Assert.AreEqual(instance, obj);

            A.CallTo(() => con.ApiConnector.FindByAsync<T>(null, null, null)).WithAnyArguments().MustHaveHappened();
        }
    }
}
