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
            Assert.That(cf.Title, Is.EqualTo("CF"));
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
            Assert.That(caseFile, Is.Not.Null);
            Assert.That(caseFile.Id, Is.EqualTo(245));
            Assert.That(caseFile.Title, Is.EqualTo("CF"));
            Assert.That(caseFile.Status, Is.EqualTo(1));
            Assert.That(caseFile.SignIteration, Is.EqualTo(2));
            Assert.That(caseFile.VisibilityMode, Is.EqualTo(3));
            Assert.That(caseFile.Created, Is.EqualTo(new DateTime(2014, 9, 30, 15, 58, 56)));
            Assert.That(caseFile.Reference, Is.EqualTo("hi"));

            //Signers
            Assert.That(caseFile.Signers, Is.Not.Null);
            Assert.That(caseFile.Signers.Count(), Is.EqualTo(1));
            var signer = caseFile.Signers.First();
            Assert.That(signer.Id, Is.EqualTo(334));
            Assert.That(signer.Name, Is.EqualTo("A signer"));

            //Signing Request
            Assert.That(signer.SigningRequest, Is.Not.Null);
            var sr = signer.SigningRequest;
            Assert.That(sr.Id, Is.EqualTo(334));
            Assert.That(sr.Email, Is.EqualTo("test@example.com"));
            Assert.That(sr.EmailSubject, Is.EqualTo("Test subject"));
            Assert.That(sr.EmailText, Is.EqualTo("Test text"));
            Assert.That(sr.Status, Is.EqualTo(1));
            Assert.That(sr.AccessControl, Is.True);

            //Document
            Assert.That(caseFile.Documents, Is.Not.Null);
            Assert.That(caseFile.Documents.Count(), Is.EqualTo(1));
            var doc = caseFile.Documents.First();
            Assert.That(doc.Id, Is.EqualTo(359));
            Assert.That(doc.DocumentId, Is.EqualTo("CB5VL-GS115-G5KDD-GFHKH-IPGAX-3J0LZ"));
            Assert.That(doc.Title, Is.EqualTo("My Doc"));
            Assert.That(doc.Status, Is.EqualTo(0));
            Assert.That(doc.Signable, Is.True);
            Assert.That(doc.Created, Is.EqualTo(new DateTime(2014, 9, 30, 15, 59, 04)));
            Assert.That(doc.Modified, Is.EqualTo(new DateTime(2014, 9, 30, 15, 59, 04)));
            Assert.That(doc.Completed, Is.EqualTo(new DateTime(2014, 9, 30, 15, 59, 04)));

            //Signature Line
            Assert.That(doc.SignatureLines, Is.Not.Null);
            Assert.That(doc.SignatureLines.Count(), Is.EqualTo(1));
            var sl = doc.SignatureLines.First();
            Assert.That(sl.Id, Is.EqualTo(477));
            Assert.That(sl.SignerId, Is.EqualTo(334));
        }
    }
}
