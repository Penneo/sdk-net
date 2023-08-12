using System.IO;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using Penneo;

namespace PenneoTests
{
    [TestFixture]
    public class ValidationTests
    {
        [Test]
        public void ConstructorTest()
        {
            var cf = new Validation("name", "email");
            Assert.AreEqual("name", cf.Name);
            Assert.AreEqual("email", cf.Email);
        }

        [Test]
        public async Task PersistSuccessTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPersist(con, () => new Validation());
        }

        [Test]
        public async Task PersistFailTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPersistFail(con, () => new Validation());
        }

        [Test]
        public async Task DeleteTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestDelete(con, () => new Validation());
        }

        [Test]
        public async Task GetTest()
        {
            await TestUtil.TestGet<Validation>();
        }

        [Test]
        public void GetLinkTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestGetTextAsset(con, () => new Validation().GetLinkAsync(con));
        }

        [Test]
        public void GetPdfTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestGetFileAsset(con, () => new Validation().GetPdfAsync(con));
        }

        [Test]
        public async Task SavePdfTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var data = new byte[] { 1, 2, 3 };
            A.CallTo(() => con.ApiConnector.GetFileAssetsAsync(null, null)).WithAnyArguments().Returns(data);

            var doc = new Validation();
            var savePath = Path.GetTempFileName();
            try
            {
                await doc.SavePdfAsync(con, savePath);
                var readBytes = File.ReadAllBytes(savePath);
                CollectionAssert.AreEqual(data, readBytes);
            }
            finally
            {
                File.Delete(savePath);
            }

            A.CallTo(() => con.ApiConnector.GetFileAssetsAsync(null, null)).WithAnyArguments().MustHaveHappened();
        }

        [Test]
        public async Task SendTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPerformActionSuccess(con, () => new Validation().SendAsync(con));
        }
    }
}
