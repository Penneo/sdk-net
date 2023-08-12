using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Penneo;

namespace PenneoTests
{
    [TestFixture]
    public class CastFileTests
    {
        [Test]
        public void ConstructorTest()
        {
            var cf = new CaseFile("CF");
            Assert.AreEqual("CF", cf.Title);
        }

        [Test]
        public async Task PersistSuccessTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPersist(con, () => new CaseFile());
        }

        [Test]
        public async Task PersistFailTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPersistFail(con, () => new CaseFile());
        }

        [Test]
        public async Task DeleteTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestDelete(con, () => new CaseFile());
        }

        [Test]
        public async Task GetTest()
        {
            await TestUtil.TestGet<CaseFile>();
        }

        [Test]
        public async Task GetDocumentsTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestGetLinked(con, () => new CaseFile().GetDocumentsAsync(con));
        }

        [Test]
        public async Task GetSignersTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestGetLinked(con, () => new CaseFile().GetSignersAsync(con));
        }

        [Test]
        public async Task FindSignerTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestFindLinked(con, () => new CaseFile().FindSignerAsync(con, 0));
        }

        [Test]
        public async Task SendTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPerformActionSuccess(con, () => new CaseFile().SendAsync(con));
        }
        
        [Test]
        public async Task ActivateTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPerformActionSuccess(con, () => new CaseFile().ActivateAsync(con));
        }

        [Test]
        public void TestJsonDeserialization()
        {
            const string json = "{\"reference\":\"hi\",\"signers\":[{\"sdkClassName\":\"Signer\",\"id\":334,\"name\":\"A signer\",\"signingRequest\":{\"sdkClassName\":\"SigningRequest\",\"id\":334,\"email\":\"test@example.com\",\"emailSubject\":\"Test subject\",\"emailText\":\"Test text\",\"status\":1,\"accessControl\":true}}],\"sdkClassName\":\"CaseFile\",\"id\":245,\"title\":\"CF\",\"status\":1,\"documents\":[{\"sdkClassName\":\"Document\",\"id\":359,\"documentId\":\"CB5VL-GS115-G5KDD-GFHKH-IPGAX-3J0LZ\",\"title\":\"My Doc\",\"status\":0,\"signable\":true,\"signatureLines\":[{\"signerId\":334,\"sdkClassName\":\"SignatureLine\",\"id\":477,\"signOrder\":0}],\"created\":\"1412092744\",\"modified\":\"1412092744\",\"completed\":\"1412092744\"}],\"signIteration\":2,\"visibilityMode\":3,\"created\":\"1412092736\"}";

            var caseFile = JsonConvert.DeserializeObject<CaseFile>(json);

            //Case File
            Assert.IsNotNull(caseFile);
            Assert.AreEqual(245, caseFile.Id);
            Assert.AreEqual("CF", caseFile.Title);
            Assert.AreEqual(1, caseFile.Status);
            Assert.AreEqual(2, caseFile.SignIteration);
            Assert.AreEqual(3, caseFile.VisibilityMode);
            Assert.AreEqual(new DateTime(2014, 9, 30, 15, 58, 56), caseFile.Created);
            Assert.AreEqual("hi", caseFile.Reference);

            //Signers
            Assert.IsNotNull(caseFile.Signers);
            Assert.AreEqual(1, caseFile.Signers.Count());
            var signer = caseFile.Signers.First();
            Assert.AreEqual(334, signer.Id);
            Assert.AreEqual("A signer", signer.Name);

            //Signing Request
            Assert.IsNotNull(signer.SigningRequest);
            var sr = signer.SigningRequest;
            Assert.AreEqual(334, sr.Id);
            Assert.AreEqual("test@example.com", sr.Email);
            Assert.AreEqual("Test subject", sr.EmailSubject);
            Assert.AreEqual("Test text", sr.EmailText);
            Assert.AreEqual(1, sr.Status);
            Assert.AreEqual(true, sr.AccessControl);

            //Document
            Assert.IsNotNull(caseFile.Documents);
            Assert.AreEqual(1, caseFile.Documents.Count());
            var doc = caseFile.Documents.First();
            Assert.AreEqual(359, doc.Id);
            Assert.AreEqual("CB5VL-GS115-G5KDD-GFHKH-IPGAX-3J0LZ", doc.DocumentId);
            Assert.AreEqual("My Doc", doc.Title);
            Assert.AreEqual(0, doc.Status);
            Assert.AreEqual(true, doc.Signable);
            Assert.AreEqual(new DateTime(2014, 9, 30, 15, 59, 04), doc.Created);
            Assert.AreEqual(new DateTime(2014, 9, 30, 15, 59, 04), doc.Modified);
            Assert.AreEqual(new DateTime(2014, 9, 30, 15, 59, 04), doc.Completed);

            //Signature Line
            Assert.IsNotNull(doc.SignatureLines);
            Assert.AreEqual(1, doc.SignatureLines.Count());
            var sl = doc.SignatureLines.First();
            Assert.AreEqual(477, sl.Id);
            Assert.AreEqual(334, sl.SignerId);
        }
    }
}
