using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FakeItEasy;
using Penneo;
using Penneo.Connector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace PenneoTests
{
    internal static class TestUtil
    {
        private static IRestResponse _response200 = new RestResponse { StatusCode = HttpStatusCode.OK };

        public static IApiConnector CreateFakeConnector()
        {
            var fake = A.Fake<IApiConnector>();
            ApiConnector.SetFactory(() => fake);
            return fake;
        }

        public static void TestPersist(Func<Entity> f)
        {
            var connector = CreateFakeConnector();
            A.CallTo(() => connector.WriteObject(null)).WithAnyArguments();

            var e = f();
            e.Persist(); 

            A.CallTo(() => connector.WriteObject(e)).MustHaveHappened();
        }

        public static void TestPersistFail(Func<Entity> f)
        {
            var connector = CreateFakeConnector();
            A.CallTo(() => connector.WriteObject(null)).WithAnyArguments().Returns(false);

            var e = f();
            var result = e.Persist();
            
            A.CallTo(() => connector.WriteObject(e)).MustHaveHappened();
            Assert.IsFalse(result);
        }

        public static void TestDelete(Func<Entity> f)
        {
            var connector = CreateFakeConnector();
            var e = f();
            A.CallTo(() => connector.DeleteObject(e)).Returns(true);
            e.Delete();
            A.CallTo(() => connector.DeleteObject(e)).MustHaveHappened();
        }   

        public static void TestGet<T>()
            where T : Entity
        {
            var connector = CreateFakeConnector();
            var list = new List<T> { Activator.CreateInstance<T>() };
            IEnumerable<T> ignoredObjects;
            IRestResponse ignoredResponse;
            A.CallTo(() => connector.FindBy(null, out ignoredObjects, out ignoredResponse)).WithAnyArguments().Returns(true).AssignsOutAndRefParameters(list, _response200);

            var result = Query.FindAll<T>();

            A.CallTo(() => connector.FindBy(null, out ignoredObjects, out ignoredResponse)).WithAnyArguments().MustHaveHappened();
            Assert.AreEqual(list, result);
        }

        public static void TestGetLinked<TChild>(Func<IEnumerable<TChild>> getter)
        {
            var connector = CreateFakeConnector();
            var list = new List<TChild>() {Activator.CreateInstance<TChild>()};
            A.CallTo(() => connector.GetLinkedEntities<TChild>(null, null)).WithAnyArguments().Returns(list);

            var result = getter();

            A.CallTo(() => connector.GetLinkedEntities<TChild>(null, null)).WithAnyArguments().MustHaveHappened();
            Assert.IsNotNull(result);
            Assert.AreEqual(list.Count, result.Count());
        }

        public static void TestGetLinked<TChild>(Func<TChild> getter)
        {
            var connector = CreateFakeConnector();
            var instance = Activator.CreateInstance<TChild>();
            var list = new List<TChild> { instance };
            A.CallTo(() => connector.GetLinkedEntities<TChild>(null, null)).WithAnyArguments().Returns(list);

            var result = getter();

            A.CallTo(() => connector.GetLinkedEntities<TChild>(null, null)).WithAnyArguments().MustHaveHappened();
            Assert.IsNotNull(result);
            Assert.AreEqual(instance, result);
        }

        public static void TestGetLinkedNotCalled<TChild>(Func<TChild> getter)
        {
            var connector = CreateFakeConnector();
            A.CallTo(() => connector.GetLinkedEntities<TChild>(null, null)).WithAnyArguments().Returns(new List<TChild>());
            getter();
            A.CallTo(() => connector.GetLinkedEntities<TChild>(null, null)).WithAnyArguments().MustNotHaveHappened();
        }

        public static void TestFindLinked<TChild>(Func<TChild> getter)
        {
            var connector = CreateFakeConnector();
            var instance = Activator.CreateInstance<TChild>();
            A.CallTo(() => connector.FindLinkedEntity<TChild>(null, 0)).WithAnyArguments().Returns(instance);

            var result = getter();

            A.CallTo(() => connector.FindLinkedEntity<TChild>(null, 0)).WithAnyArguments().MustHaveHappened();
            Assert.IsNotNull(result);
            Assert.AreEqual(instance, result);
        }

        public static void TestPerformActionSuccess(Action action)
        {
            var connector = CreateFakeConnector();
            A.CallTo(() => connector.PerformAction(null, null)).WithAnyArguments().Returns(new ServerResult());

            action();

            A.CallTo(() => connector.PerformAction(null, null)).WithAnyArguments().MustHaveHappened();
        }

        public static void TestLink(Action action, Entity parent, Entity child)
        {
            var connector = CreateFakeConnector();
            A.CallTo(() => connector.LinkEntity(parent, child)).WithAnyArguments().Returns(true);

            action();

            A.CallTo(() => connector.LinkEntity(parent, child)).WithAnyArguments().MustHaveHappened();
        }

        public static void TestUnlink(Action action, Entity parent, Entity child)
        {
            var connector = CreateFakeConnector();
            A.CallTo(() => connector.UnlinkEntity(parent, child)).WithAnyArguments().Returns(true);

            action();

            A.CallTo(() => connector.UnlinkEntity(parent, child)).WithAnyArguments().MustHaveHappened();
        }

        public static void TestGetFileAsset(Action action)
        {
            var connector = CreateFakeConnector();
            var data = new byte[] {1, 2, 3};
            A.CallTo(() => connector.GetFileAssets(null, null)).WithAnyArguments().Returns(data);

            action();

            A.CallTo(() => connector.GetFileAssets(null, null)).WithAnyArguments().MustHaveHappened();
        }

        public static void TestGetTextAsset(Action action)
        {
            var connector = CreateFakeConnector();
            const string data = "123";
            A.CallTo(() => connector.GetTextAssets(null, null)).WithAnyArguments().Returns(data);

            action();

            A.CallTo(() => connector.GetTextAssets(null, null)).WithAnyArguments().MustHaveHappened();
        }
    }
}
