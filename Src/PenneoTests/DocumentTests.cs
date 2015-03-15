using System;
using System.IO;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penneo;

namespace PenneoTests
{
    [TestClass]
    public class DocumentTests
    {
        private static Document CreateDocument()
        {
            var cf = new CaseFile();
            var doc = new Document(cf, "doc", "path");
            doc.Id = 1;
            return doc;
        }

        [TestMethod]
        public void ConstructorTest()
        {
            var doc = CreateDocument();
            Assert.IsNotNull(doc.CaseFile);
            Assert.AreEqual("doc", doc.Title);
            Assert.AreEqual("path", doc.PdfFile);
        }

        [TestMethod]
        public void GetCaseFileTest()
        {
            var doc = new Document();
            TestUtil.TestGetLinked(() => doc.CaseFile);
        }

        [TestMethod]
        public void PersistSuccessTest()
        {
            TestUtil.TestPersist(CreateDocument);
        }

        [TestMethod]
        public void PersistFailTest()
        {
            TestUtil.TestPersistFail(CreateDocument);
        }

        [TestMethod]
        public void DeleteTest()
        {
            TestUtil.TestDelete(CreateDocument);
        }
        
        [TestMethod]
        public void GetTest()
        {
            TestUtil.TestGet<Document>();
        }

        [TestMethod]
        public void MakeSignableTest()
        {
            var doc = CreateDocument();
            doc.MakeSignable();
            Assert.AreEqual("signable", doc.Type);
        }

        
        [TestMethod]
        public void GetSignatureLinesTest()
        {
            TestUtil.TestGetLinked(CreateDocument().GetSignatureLines);
        }

        [TestMethod]
        public void FindSignatureLineTest()
        {
            TestUtil.TestFindLinked(() => CreateDocument().FindSignatureLine(0));
        }

        [TestMethod]
        public void GetPdfTest()
        {
            TestUtil.TestGetFileAsset(() => CreateDocument().GetPdf());
        }

        [TestMethod]
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

        [TestMethod]
        public void GetDocumentTypeTest()
        {
            var doc1 = CreateDocument();
            TestUtil.TestGetLinked(doc1.GetDocumentType);

            var doc2 = new Document();
            doc2.DocumentType = new DocumentType();
            TestUtil.TestGetLinkedNotCalled(doc2.GetDocumentType);
            Assert.AreEqual(doc2.GetDocumentType(), doc2.DocumentType);
        }

        [TestMethod]
        public void SetDocumentTypeTest()
        {
            var dt = new DocumentType();
            var doc = new Document();
            doc.SetDocumentType(dt);
            Assert.AreEqual(dt, doc.DocumentType);
        }

        [TestMethod]
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