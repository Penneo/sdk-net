using System.IO;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Penneo;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using CollectionAssert = Microsoft.VisualStudio.TestTools.UnitTesting.CollectionAssert;

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
            TestUtil.TestPersist(() => new Validation());
        }

        [Test]
        public void PersistFailTest()
        {
            TestUtil.TestPersistFail(() => new Validation());
        }

        [Test]
        public void DeleteTest()
        {
            TestUtil.TestDelete(() => new Validation());
        }

        [Test]
        public void GetTest()
        {
            TestUtil.TestGet<Validation>();
        }

        [Test]
        public void GetLinkTest()
        {
            TestUtil.TestGetTextAsset(() => new Validation().GetLink());
        }

        [Test]
        public void GetPdfTest()
        {
            TestUtil.TestGetFileAsset(() => new Validation().GetPdf());
        }

        [Test]
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

        [Test]
        public void SendTest()
        {
            TestUtil.TestPerformActionSuccess(() => new Validation().Send());
        }
    }
}