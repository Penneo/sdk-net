using System;
using System.Linq;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penneo;
using System.Collections.Generic;

namespace PenneoTests
{
    [TestClass]
    public class QueryTests
    {
        [TestMethod]
        public void FindTest()
        {
            var connector = TestUtil.CreateFakeConnector();
            A.CallTo(() => connector.ReadObject(null)).WithAnyArguments().Returns(true);

            var obj = Query.Find<CaseFile>(1);

            Assert.AreEqual(1, obj.Id);
            A.CallTo(() => connector.ReadObject(null)).WithAnyArguments().MustHaveHappened();
        }

        [TestMethod]
        public void FindOneByTest()
        {
            FindOneTest(() => Query.FindOneBy<CaseFile>());
        }

        [TestMethod]
        public void FindAllTest()
        {
            FindCollectionTest(Query.FindAll<CaseFile>);
        }

        [TestMethod]
        public void FindByTest()
        {
            /*var query = new Dictionary<string, object>();
            var orderBy = new Dictionary<string, string>() { {"title", "asc"}};
            FindCollectionTest(() => Query.FindBy<CaseFile>(query, orderBy, 10, 5));*/

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
            IEnumerable<T> ignored;
            A.CallTo(() => connector.FindBy(null, out ignored)).WithAnyArguments().Returns(true).AssignsOutAndRefParameters(returned);

            var objects = f();

            Assert.IsNotNull(objects);
            CollectionAssert.AreEqual(returned.ToList(), objects.ToList());

            A.CallTo(() => connector.FindBy(null, out objects)).WithAnyArguments().MustHaveHappened();
        }

        private static void FindOneTest<T>(Func<T> f)
            where T : Entity
        {
            var connector = TestUtil.CreateFakeConnector();
            var instance = Activator.CreateInstance<T>();
            IEnumerable<T> returned = new[] { instance };
            IEnumerable<T> ignored;
            A.CallTo(() => connector.FindBy(null, out ignored)).WithAnyArguments().Returns(true).AssignsOutAndRefParameters(returned);

            var obj = f();

            Assert.IsNotNull(obj);
            Assert.AreEqual(instance, obj);

            A.CallTo(() => connector.FindBy(null, out ignored)).WithAnyArguments().MustHaveHappened();
        }
    }
}