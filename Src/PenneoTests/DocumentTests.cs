﻿using System;
using System.IO;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using NUnit.Framework.Legacy;
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
            doc.DocumentOrder = 1;
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
            var doc = CreateDocument();
            Assert.That(doc.CaseFile, Is.Not.Null);
            Assert.That(doc.Title, Is.EqualTo("doc"));
            Assert.That(doc.DocumentOrder, Is.EqualTo(1));
            Assert.That(doc.PdfFile, Is.EqualTo(TestPdfPath));
        }

        [Test]
        public async Task GetCaseFileTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var doc = new Document();
            await TestUtil.TestGetLinked(con, async () => await doc.GetCaseFileAsync(con));
        }

        [Test]
        public async Task PersistSuccessTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPersist(con, CreateDocument);
        }

        [Test]
        public async Task PersistFailTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPersistFail(con, CreateDocument);
        }

        [Test]
        public async Task DeleteTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestDelete(con, CreateDocument);
        }

        [Test]
        public async Task GetTest()
        {
            await TestUtil.TestGet<Document>();
        }

        [Test]
        public void MakeSignableTest()
        {
            var doc = CreateDocument();
            doc.MakeSignable();
            Assert.That(doc.SignType, Is.EqualTo("signable"));
        }


        [Test]
        public async Task GetSignatureLinesTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestGetLinked(con, async () => await CreateDocument().GetSignatureLinesAsync(con));
        }

        [Test]
        public async Task FindSignatureLineTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestFindLinked(con, async () => await CreateDocument().FindSignatureLineAsync(con, 0));
        }

        [Test]
        public async Task GetPdfTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestGetFileAsset(con, () => CreateEmptyDocument().GetPdfAsync(con));
        }

        [Test]
        public async Task SavePdfTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var data = File.ReadAllBytes(TestPdfPath);
            A.CallTo(() => con.ApiConnector.GetFileAssetsAsync(null, null)).WithAnyArguments().Returns(data);

            var doc = CreateDocument();
            var savePath = Path.GetTempFileName();
            try
            {
                await doc.SavePdfAsync(con, savePath);
                var readBytes = await File.ReadAllBytesAsync(savePath);
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
                await doc2.SavePdfAsync(con, savePath);
                var readBytes = await File.ReadAllBytesAsync(savePath);
                CollectionAssert.AreEqual(data, readBytes);
            }
            finally
            {
                File.Delete(savePath2);
            }

            A.CallTo(() => con.ApiConnector.GetFileAssetsAsync(null, null)).WithAnyArguments().MustHaveHappened();
        }

        [Test]
        public async Task GetDocumentTypeTest()
        {
            var con1 = TestUtil.CreatePenneoConnector();
            var doc1 = CreateDocument();
            await TestUtil.TestGetLinked(con1, () => doc1.GetDocumentTypeAsync(con1));

            var con2 = TestUtil.CreatePenneoConnector();
            var doc2 = new Document();
            doc2.DocumentType = new DocumentType();
            await TestUtil.TestGetLinkedNotCalled(con2, () => doc2.GetDocumentTypeAsync(con2));
            Assert.That(doc2.DocumentType, Is.EqualTo(await doc2.GetDocumentTypeAsync(con2)));
        }

        [Test]
        public void SetDocumentTypeTest()
        {
            var dt = new DocumentType();
            var doc = new Document();
            doc.SetDocumentType(dt);
            Assert.That(doc.DocumentType, Is.EqualTo(dt));
        }

        [Test]
        public void GetStatusTest()
        {
            var doc = CreateDocument();
            doc.Status = null;
            Assert.That(doc.GetStatus(), Is.EqualTo(DocumentStatus.New));

            doc.Status = (int?)DocumentStatus.Completed;
            Assert.That(doc.GetStatus(), Is.EqualTo(DocumentStatus.Completed));
        }

        [Test]
        public void UseBase64FileContentTest()
        {
            var data = Convert.FromBase64String(Base64String);
            var doc = CreateBase64Document();
            Assert.That(doc.PdfRaw, Is.EqualTo(data));
        }
    }
}