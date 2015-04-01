using System;
using System.Linq;
using System.Net;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Penneo;
using System.Collections.Generic;
using RestSharp;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using CollectionAssert = Microsoft.VisualStudio.TestTools.UnitTesting.CollectionAssert;

namespace PenneoTests
{
    [TestFixture]
    public class QueryTests
    {
        private static IRestResponse _response200 = new RestResponse { StatusCode = HttpStatusCode.OK};

        [Test]
        public void FindTest()
        {
            var connector = TestUtil.CreateFakeConnector();
            IRestResponse ignoredResponse;
            A.CallTo(() => connector.ReadObject(null, out ignoredResponse)).WithAnyArguments().Returns(true).AssignsOutAndRefParameters(_response200);

            var obj = Query.Find<CaseFile>(1);

            Assert.AreEqual(1, obj.Id);
            A.CallTo(() => connector.ReadObject(null, out ignoredResponse)).WithAnyArguments().MustHaveHappened();
        }

        [Test]
        public void FindOneByTest()
        {
            FindOneTest(() => Query.FindOneBy<CaseFile>());
        }

        [Test]
        public void FindAllTest()
        {
            FindCollectionTest(Query.FindAll<CaseFile>);
        }

        [Test]
        public void FindByTest()
        {
            FindCollectionTest(() =>  Query.FindBy<Document>(
                new Dictionary<string, object> { { "title", "the" } },
                new Dictionary<string, string>() { { "created", "desc" } },
                10,5
            ));

        }

        private static void FindCollectionTest<T>(Func<IEnumerable<T>> f)
            where T : Entity
        {
            var connector = TestUtil.CreateFakeConnector();
            IEnumerable<T> returned = new[] { Activator.CreateInstance<T>() };
            IEnumerable<T> ignoredObjects;
            IRestResponse ignoredResponse;
            A.CallTo(() => connector.FindBy(null, out ignoredObjects, out ignoredResponse)).WithAnyArguments().Returns(true).AssignsOutAndRefParameters(returned, _response200);

            var objects = f();

            Assert.IsNotNull(objects);
            CollectionAssert.AreEqual(returned.ToList(), objects.ToList());

            A.CallTo(() => connector.FindBy(null, out objects, out ignoredResponse)).WithAnyArguments().MustHaveHappened();
        }

        private static void FindOneTest<T>(Func<T> f)
            where T : Entity
        {
            var connector = TestUtil.CreateFakeConnector();
            var instance = Activator.CreateInstance<T>();
            IEnumerable<T> returned = new[] { instance };
            IEnumerable<T> ignoredObjects;
            IRestResponse ignoredResponse;
            A.CallTo(() => connector.FindBy(null, out ignoredObjects, out ignoredResponse)).WithAnyArguments().Returns(true).AssignsOutAndRefParameters(returned, _response200);

            var obj = f();

            Assert.IsNotNull(obj);
            Assert.AreEqual(instance, obj);

            A.CallTo(() => connector.FindBy(null, out ignoredObjects, out ignoredResponse)).WithAnyArguments().MustHaveHappened();
        }
    }
}