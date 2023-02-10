﻿using System;
using System.IO;
using FakeItEasy;
using NUnit.Framework;
using Penneo;

namespace PenneoTests
{
    [TestFixture]
    public class DocumentTests
    {
        private static readonly string TestPdfPath = $"{Directory.GetCurrentDirectory()}/Resources/test.pdf";
        private static readonly string Base64String = "TZLu0SfTr3wxiuAxzdFBvA==";

        private static Document CreateDocument()
        {
            var cf = new CaseFile();
            var doc = new Document(cf, "doc", TestPdfPath);
            doc.Id = 1;
            return doc;
        }
        
        private static Document CreateEmptyDocument()
        {
            var cf = new CaseFile();
            var doc = new Document(cf);
            doc.Id = 2;
            return doc;
        }
        
        private static Document CreateBase64Document()
        {
            var cf = new CaseFile();
            var doc = new Document(cf, "doc", Base64String);
            doc.Id = 3;
            return doc;
        }

        [Test]
        public void ConstructorTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var doc = CreateDocument();
            Assert.IsNotNull(doc.CaseFile);
            Assert.AreEqual("doc", doc.Title);
            Assert.AreEqual(TestPdfPath, doc.PdfFile);
        }

        [Test]
        public void GetCaseFileTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var doc = new Document();
            TestUtil.TestGetLinked(con, () => doc.GetCaseFile(con));
        }

        [Test]
        public void PersistSuccessTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestPersist(con, CreateDocument);
        }

        [Test]
        public void PersistFailTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestPersistFail(con, CreateDocument);
        }

        [Test]
        public void DeleteTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestDelete(con, CreateDocument);
        }

        [Test]
        public void GetTest()
        {
            TestUtil.TestGet<Document>();
        }

        [Test]
        public void MakeSignableTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var doc = CreateDocument();
            doc.MakeSignable();
            Assert.AreEqual("signable", doc.SignType);
        }


        [Test]
        public void GetSignatureLinesTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestGetLinked(con, () => CreateDocument().GetSignatureLines(con));
        }

        [Test]
        public void FindSignatureLineTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestFindLinked(con, () => CreateDocument().FindSignatureLine(con, 0));
        }

        [Test]
        public void GetPdfTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestGetFileAsset(con, () => CreateEmptyDocument().GetPdf(con));
        }

        [Test]
        public void SavePdfTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var data = File.ReadAllBytes(TestPdfPath);
            A.CallTo(() => con.ApiConnector.GetFileAssets(null, null)).WithAnyArguments().Returns(data);

            var doc = CreateDocument();
            var savePath = Path.GetTempFileName();
            try
            {
                doc.SavePdf(con, savePath);
                var readBytes = File.ReadAllBytes(savePath);
                CollectionAssert.AreEqual(data, readBytes);
            }
            finally
            {
                File.Delete(savePath);
            }

            var doc2 = CreateEmptyDocument();
            var savePath2 = Path.GetTempFileName();
            try
            {
                doc2.SavePdf(con, savePath);
                var readBytes = File.ReadAllBytes(savePath);
                CollectionAssert.AreEqual(data, readBytes);
            }
            finally
            {
                File.Delete(savePath2);
            }

            A.CallTo(() => con.ApiConnector.GetFileAssets(null, null)).WithAnyArguments().MustHaveHappened();
        }

        [Test]
        public void GetDocumentTypeTest()
        {
            var con1 = TestUtil.CreatePenneoConnector();
            var doc1 = CreateDocument();
            TestUtil.TestGetLinked(con1, () => doc1.GetDocumentType(con1));

            var con2 = TestUtil.CreatePenneoConnector();
            var doc2 = new Document();
            doc2.DocumentType = new DocumentType();
            TestUtil.TestGetLinkedNotCalled(con2, () => doc2.GetDocumentType(con2));
            Assert.AreEqual(doc2.GetDocumentType(con2), doc2.DocumentType);
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
            var con = TestUtil.CreatePenneoConnector();
            var doc = CreateDocument();
            doc.Status = null;
            Assert.AreEqual(DocumentStatus.New, doc.GetStatus());

            doc.Status = (int?)DocumentStatus.Completed;
            Assert.AreEqual(DocumentStatus.Completed, doc.GetStatus());
        }

        [Test]
        public void UseBase64FileContentTest()
        {
            var data = Convert.FromBase64String(Base64String);
            var doc = CreateBase64Document();
            Assert.AreEqual(data, doc.PdfRaw);
        }
    }
}