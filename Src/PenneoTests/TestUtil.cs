using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using Penneo;
using Penneo.Connector;
using RestSharp;

namespace PenneoTests
{
    internal static class TestUtil
    {
        private static RestResponse _response200 = new RestResponse { StatusCode = HttpStatusCode.OK };

        public static PenneoConnector CreatePenneoConnector()
        {
            var con = new PenneoConnector(null, null, null, null, null, AuthType.WSSE);
            con.ApiConnector = CreateFakeConnector();
            return con;
        }

        public static IApiConnector CreateFakeConnector()
        {
            var fake = A.Fake<IApiConnector>();
            return fake;
        }

        internal static ApiConnector CreateTestApiConnector()
        {
            return new ApiConnector(null, null, null, null, null, null, null, AuthType.WSSE);
        }

        public static async Task TestPersist(PenneoConnector con, Func<Entity> f)
        {
            A.CallTo(() => con.ApiConnector.WriteObjectAsync(null)).WithAnyArguments();

            var e = f();
            await e.PersistAsync(con); 

            A.CallTo(() => con.ApiConnector.WriteObjectAsync(e)).MustHaveHappened();
        }

        public static async Task TestPersistFail(PenneoConnector con, Func<Entity> f)
        {
            A.CallTo(() => con.ApiConnector.WriteObjectAsync(null)).WithAnyArguments().Returns(false);

            var e = f();
            var result = await e.PersistAsync(con);
            
            A.CallTo(() => con.ApiConnector.WriteObjectAsync(e)).MustHaveHappened();
            Assert.IsFalse(result);
        }

        public static async Task TestDelete(PenneoConnector con, Func<Entity> f)
        {
            var e = f();
            A.CallTo(() => con.ApiConnector.DeleteObjectAsync(e)).Returns(true);
            await e.DeleteAsync(con);
            A.CallTo(() => con.ApiConnector.DeleteObjectAsync(e)).MustHaveHappened();
        }   

        public static async Task TestGet<T>()
            where T : Entity
        {
            var con = TestUtil.CreatePenneoConnector();
            var list = new List<T> { (T)Activator.CreateInstance(typeof(T)) };
            for (var i = 0; i < list.Count; i++)
            {
                list[i].Id = i;
            }
            A.CallTo(() => con.ApiConnector.FindByAsync<T>(null, null, null)).WithAnyArguments()
                .Returns(Task.FromResult(
                    new FindByResult<T> { Success = true, Objects = list, Response = _response200 }));

            var q = new Query(con);
            var result = (await q.FindAllAsync<T>()).ToList();

            A.CallTo(() => con.ApiConnector.FindByAsync<T>(null, null, null)).WithAnyArguments().MustHaveHappened();
            CollectionAssert.AreEqual(list, result);
        }

        public static async Task TestGetLinked<TChild>(PenneoConnector con, Func<Task<IEnumerable<TChild>>> getter)
            where TChild: Entity
        {
            var list = new List<TChild>() { (TChild)Activator.CreateInstance(typeof(TChild))};
            var mockedResult = new QueryResult<TChild>() { Objects = list, StatusCode = HttpStatusCode.OK };
            A.CallTo(() => con.ApiConnector.GetLinkedEntitiesAsync<TChild>(null, null)).WithAnyArguments().Returns(mockedResult);

            var result = await getter();

            A.CallTo(() => con.ApiConnector.GetLinkedEntitiesAsync<TChild>(null, null)).WithAnyArguments().MustHaveHappened();
            Assert.IsNotNull(result);
            Assert.AreEqual(list.Count, result.Count());
        }

        public static async Task TestGetLinked<TChild>(PenneoConnector con, Func<Task<TChild>> getter)
            where TChild: Entity
        {
            var instance = (TChild)Activator.CreateInstance(typeof(TChild));
            var list = new List<TChild> { instance };
            var mockedResult = new QueryResult<TChild>() {Objects = list, StatusCode = HttpStatusCode.OK};
            A.CallTo(() => con.ApiConnector.GetLinkedEntitiesAsync<TChild>(null, null)).WithAnyArguments().Returns(mockedResult);

            var result = await getter();

            A.CallTo(() => con.ApiConnector.GetLinkedEntitiesAsync<TChild>(null, null)).WithAnyArguments().MustHaveHappened();
            Assert.IsNotNull(result);
            Assert.AreEqual(instance, result);
        }


        public static void TestGetLinkedNotCalled<TChild>(PenneoConnector con, Func<Task<TChild>> getter)
            where TChild: Entity
        {
            var mockedResult = new QueryResult<TChild>() { Objects = new List<TChild>() , StatusCode = HttpStatusCode.OK};
            A.CallTo(() => con.ApiConnector.GetLinkedEntitiesAsync<TChild>(null, null)).WithAnyArguments().Returns(mockedResult);
            getter();
            A.CallTo(() => con.ApiConnector.GetLinkedEntitiesAsync<TChild>(null, null)).WithAnyArguments().MustNotHaveHappened();
        }

        public static async void TestFindLinked<TChild>(PenneoConnector con, Func<Task<TChild>> getter)
            where TChild : Entity
        {
            var instance = (TChild)Activator.CreateInstance(typeof(TChild));
            A.CallTo(() => con.ApiConnector.FindLinkedEntityAsync<TChild>(null, 0)).WithAnyArguments().Returns(instance);

            var task = getter();
            var result = await task;

            A.CallTo(() => con.ApiConnector.FindLinkedEntityAsync<TChild>(null, 0)).WithAnyArguments().MustHaveHappened();
            Assert.IsNotNull(result);
            Assert.AreEqual(instance, result);
        }


        public static async Task TestPerformActionSuccess(PenneoConnector con, Func<Task>func)
        {
            A.CallTo(() => con.ApiConnector.PerformAction(null, null)).WithAnyArguments().Returns(new ServerResult());

            await func();

            A.CallTo(() => con.ApiConnector.PerformAction(null, null)).WithAnyArguments().MustHaveHappened();
        }

        public static void TestLink(PenneoConnector con, Action action, Entity parent, Entity child)
        {
            A.CallTo(() => con.ApiConnector.LinkEntityAsync(parent, child)).WithAnyArguments().Returns(true);

            action();

            A.CallTo(() => con.ApiConnector.LinkEntityAsync(parent, child)).WithAnyArguments().MustHaveHappened();
        }

        public static void TestUnlink(PenneoConnector con, Action action, Entity parent, Entity child)
        {
            A.CallTo(() => con.ApiConnector.UnlinkEntityAsync(parent, child)).WithAnyArguments().Returns(true);

            action();

            A.CallTo(() => con.ApiConnector.UnlinkEntityAsync(parent, child)).WithAnyArguments().MustHaveHappened();
        }

        public static void TestGetFileAsset(PenneoConnector con, Action action)
        {
            var data = new byte[] {1, 2, 3};
            A.CallTo(() => con.ApiConnector.GetFileAssetsAsync(null, null)).WithAnyArguments().Returns(data);

            action();

            A.CallTo(() => con.ApiConnector.GetFileAssetsAsync(null, null)).WithAnyArguments().MustHaveHappened();
        }

        public static void TestGetTextAsset(PenneoConnector con, Action action)
        {
            const string data = "123";
            A.CallTo(() => con.ApiConnector.GetTextAssetsAsync(null, null)).WithAnyArguments().Returns(data);

            action();

            A.CallTo(() => con.ApiConnector.GetTextAssetsAsync(null, null)).WithAnyArguments().MustHaveHappened();
        }
    }
}
