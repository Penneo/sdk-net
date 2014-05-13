using System;
using System.IO;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penneo;

namespace PenneoTests
{
    [TestClass]
    public class ValidationTests
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var cf = new Validation("name", "email");
            Assert.AreEqual("name", cf.Name);
            Assert.AreEqual("email", cf.Email);
        }

        [TestMethod]
        public void PersistSuccessTest()
        {
            TestUtil.TestPersist(() => new Validation());
        }

        [TestMethod]
        [ExpectedException(typeof (Exception))]
        public void PersistFailTest()
        {
            TestUtil.TestPersistFail(() => new Validation());
        }

        [TestMethod]
        public void DeleteTest()
        {
            TestUtil.TestDelete(() => new Validation());
        }

        [TestMethod]
        public void GetTest()
        {
            TestUtil.TestGet<Validation>();
        }

        [TestMethod]
        public void GetLinkTest()
        {
            TestUtil.TestGetTextAsset(() => new Validation().GetLink());
        }

        [TestMethod]
        public void GetPdfTest()
        {
            TestUtil.TestGetFileAsset(() => new Validation().GetPdf());
        }

        [TestMethod]
        public void SavePdfTest()
        {
            var connector = TestUtil.CreateFakeConnector();
            var data = new byte[] { 1, 2, 3 };
            A.CallTo(() => connector.GetFileAssets(null, null)).WithAnyArguments().Returns(data);

            var doc = new Validation();
            var savePath = Path.GetTempFileName();
            try
            {
                doc.SavePdf(savePath);
                var readBytes = File.ReadAllBytes(savePath);
                CollectionAssert.AreEqual(data, readBytes);
            }
            finally
            {
                File.Delete(savePath);
            }
            A.CallTo(() => connector.GetFileAssets(null, null)).WithAnyArguments().MustHaveHappened();
        }

        [TestMethod]
        public void SendTest()
        {
            TestUtil.TestPerformActionSuccess(() => new Validation().Send());
        }
    }
}