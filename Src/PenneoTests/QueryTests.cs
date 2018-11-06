using System;
using System.Linq;
using System.Net;
using FakeItEasy;
using NUnit.Framework;
using Penneo;
using System.Collections.Generic;
using RestSharp;

namespace PenneoTests
{
    [TestFixture]
    public class QueryTests
    {
        private static IRestResponse _response200 = new RestResponse { StatusCode = HttpStatusCode.OK};

        [Test]
        public void FindTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            IRestResponse ignoredResponse;
            var expected = new CaseFile( "Test"){ Id = 1 };
            A.CallTo(() => con.ApiConnector.ReadObject<CaseFile>(null, 1, out ignoredResponse)).WithAnyArguments().Returns(expected).AssignsOutAndRefParameters(_response200);

            var q = new Query(con);
            var obj = q.Find<CaseFile>(1);

            Assert.AreEqual(1, obj.Id);
            Assert.AreEqual(expected.Title, obj.Title);
            A.CallTo(() => con.ApiConnector.ReadObject<CaseFile>(null, 1, out ignoredResponse)).WithAnyArguments().MustHaveHappened();
        }

        [Test]
        public void FindOneByTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var q = new Query(con);
            FindOneTest(con, () => q.FindOneBy<CaseFile>());
        }

        [Test]
        public void FindAllTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var q = new Query(con);
            FindCollectionTest(con, q.FindAll<CaseFile>);
        }

        [Test]
        public void FindByTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var q = new Query(con);
            FindCollectionTest(con, () =>  q.FindBy<Document>(
                new Dictionary<string, object> { { "title", "the" } },
                new Dictionary<string, string>() { { "created", "desc" } },
                10,5
            ));

        }

        private static void FindCollectionTest<T>(PenneoConnector con, Func<IEnumerable<T>> f)
            where T : Entity
        {
            IEnumerable<T> returned = new[] { (T)Activator.CreateInstance(typeof(T)) };
            IEnumerable<T> ignoredObjects;
            IRestResponse ignoredResponse;
            A.CallTo(() => con.ApiConnector.FindBy(null, out ignoredObjects, out ignoredResponse, null, null)).WithAnyArguments().Returns(true).AssignsOutAndRefParameters(returned, _response200);

            var objects = f();

            Assert.IsNotNull(objects);
            CollectionAssert.AreEqual(returned.ToList(), objects.ToList());

            A.CallTo(() => con.ApiConnector.FindBy(null, out objects, out ignoredResponse, null, null)).WithAnyArguments().MustHaveHappened();
        }

        private static void FindOneTest<T>(PenneoConnector con, Func<T> f)
            where T : Entity
        {
            var instance = (T) Activator.CreateInstance(typeof(T));
            IEnumerable<T> returned = new[] { instance };
            IEnumerable<T> ignoredObjects;
            IRestResponse ignoredResponse;
            A.CallTo(() => con.ApiConnector.FindBy(null, out ignoredObjects, out ignoredResponse, null, null)).WithAnyArguments().Returns(true).AssignsOutAndRefParameters(returned, _response200);

            var obj = f();

            Assert.IsNotNull(obj);
            Assert.AreEqual(instance, obj);

            A.CallTo(() => con.ApiConnector.FindBy(null, out ignoredObjects, out ignoredResponse, null, null)).WithAnyArguments().MustHaveHappened();
        }
    }
}