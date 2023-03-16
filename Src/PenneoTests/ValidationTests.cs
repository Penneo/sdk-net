using System.IO;
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
        public void PersistSuccessTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestPersist(con, () => new Validation());
        }

        [Test]
        public void PersistFailTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestPersistFail(con, () => new Validation());
        }

        [Test]
        public void DeleteTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestDelete(con, () => new Validation());
        }

        [Test]
        public void GetTest()
        {
            TestUtil.TestGet<Validation>();
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
        public void SavePdfTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var data = new byte[] { 1, 2, 3 };
            A.CallTo(() => con.ApiConnector.GetFileAssetsAsync(null, null)).WithAnyArguments().Returns(data);

            var doc = new Validation();
            var savePath = Path.GetTempFileName();
            try
            {
                doc.SavePdfAsync(con, savePath);
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
        public void SendTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestPerformActionSuccess(con, () => new Validation().SendAsync(con));
        }
    }
}