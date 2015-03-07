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
            Error ignoredError;
            A.CallTo(() => connector.ReadObject(null, out ignoredError)).WithAnyArguments().Returns(true);

            var obj = Query.Find<CaseFile>(1);

            Assert.AreEqual(1, obj.Id);
            A.CallTo(() => connector.ReadObject(null, out ignoredError)).WithAnyArguments().MustHaveHappened();
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
            Error ignoredError;
            A.CallTo(() => connector.FindBy(null, out ignoredObjects, out ignoredError)).WithAnyArguments().Returns(true).AssignsOutAndRefParameters(returned, null);

            var objects = f();

            Assert.IsNotNull(objects);
            CollectionAssert.AreEqual(returned.ToList(), objects.ToList());

            A.CallTo(() => connector.FindBy(null, out objects, out ignoredError)).WithAnyArguments().MustHaveHappened();
        }

        private static void FindOneTest<T>(Func<T> f)
            where T : Entity
        {
            var connector = TestUtil.CreateFakeConnector();
            var instance = Activator.CreateInstance<T>();
            IEnumerable<T> returned = new[] { instance };
            IEnumerable<T> ignoredObjects;
            Error ignoredError;
            A.CallTo(() => connector.FindBy(null, out ignoredObjects, out ignoredError)).WithAnyArguments().Returns(true).AssignsOutAndRefParameters(returned, null);

            var obj = f();

            Assert.IsNotNull(obj);
            Assert.AreEqual(instance, obj);

            A.CallTo(() => connector.FindBy(null, out ignoredObjects, out ignoredError)).WithAnyArguments().MustHaveHappened();
        }
    }
}