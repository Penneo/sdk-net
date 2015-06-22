using System.IO;
using FakeItEasy;
using NUnit.Framework;
using Penneo;

namespace PenneoTests
{
    [TestFixture]
    public class DocumentTests
    {
        private static Document CreateDocument()
        {
            var cf = new CaseFile();
            var doc = new Document(cf, "doc", "path");
            doc.Id = 1;
            return doc;
        }

        [Test]
        public void ConstructorTest()
        {
            var doc = CreateDocument();
            Assert.IsNotNull(doc.CaseFile);
            Assert.AreEqual("doc", doc.Title);
            Assert.AreEqual("path", doc.PdfFile);
        }

        [Test]
        public void GetCaseFileTest()
        {
            var doc = new Document();
            TestUtil.TestGetLinked(() => doc.CaseFile);
        }

        [Test]
        public void PersistSuccessTest()
        {
            TestUtil.TestPersist(CreateDocument);
        }

        [Test]
        public void PersistFailTest()
        {
            TestUtil.TestPersistFail(CreateDocument);
        }

        [Test]
        public void DeleteTest()
        {
            TestUtil.TestDelete(CreateDocument);
        }

        [Test]
        public void GetTest()
        {
            TestUtil.TestGet<Document>();
        }

        [Test]
        public void MakeSignableTest()
        {
            var doc = CreateDocument();
            doc.MakeSignable();
            Assert.AreEqual("signable", doc.Type);
        }


        [Test]
        public void GetSignatureLinesTest()
        {
            TestUtil.TestGetLinked(CreateDocument().GetSignatureLines);
        }

        [Test]
        public void FindSignatureLineTest()
        {
            TestUtil.TestFindLinked(() => CreateDocument().FindSignatureLine(0));
        }

        [Test]
        public void GetPdfTest()
        {
            TestUtil.TestGetFileAsset(() => CreateDocument().GetPdf());
        }

        [Test]
        public void SavePdfTest()
        {
            var connector = TestUtil.CreateFakeConnector();
            var data = new byte[] { 1, 2, 3 };
            A.CallTo(() => connector.GetFileAssets(null, null)).WithAnyArguments().Returns(data);

            var doc = CreateDocument();            
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
        public void GetDocumentTypeTest()
        {
            var doc1 = CreateDocument();
            TestUtil.TestGetLinked(doc1.GetDocumentType);

            var doc2 = new Document();
            doc2.DocumentType = new DocumentType();
            TestUtil.TestGetLinkedNotCalled(doc2.GetDocumentType);
            Assert.AreEqual(doc2.GetDocumentType(), doc2.DocumentType);
        }

        [Test]
        public void SetDocumentTypeTest()
        {
            var dt = new DocumentType();
            var doc = new Document();
            doc.SetDocumentType(dt);
            Assert.AreEqual(dt, doc.DocumentType);
        }

        [Test]
        public void GetStatusTest()
        {
            var doc = CreateDocument();
            doc.Status = null;
            Assert.AreEqual(DocumentStatus.New, doc.GetStatus());

            doc.Status = (int?)DocumentStatus.Completed;
            Assert.AreEqual(DocumentStatus.Completed, doc.GetStatus());
        }
    }
}