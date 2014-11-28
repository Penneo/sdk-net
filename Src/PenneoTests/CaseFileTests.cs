using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Penneo;

namespace PenneoTests
{
    [TestClass]
    public class CastFileTests
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var cf = new CaseFile("CF");
            Assert.AreEqual("CF", cf.Title);
        }

        [TestMethod]
        public void PersistSuccessTest()
        {
            TestUtil.TestPersist(() => new CaseFile());
        }

        [TestMethod]
        [ExpectedException(typeof (Exception))]
        public void PersistFailTest()
        {
            TestUtil.TestPersistFail(() => new CaseFile());
        }

        [TestMethod]
        public void DeleteTest()
        {
            TestUtil.TestDelete(() => new CaseFile());
        }

        [TestMethod]
        public void GetTest()
        {
            TestUtil.TestGet<CaseFile>();
        }

        [TestMethod]
        public void GetDocumentsTest()
        {
            TestUtil.TestGetLinked(new CaseFile().GetDocuments);
        }

        [TestMethod]
        public void GetSignersTest()
        {
            TestUtil.TestGetLinked(new CaseFile().GetSigners);
        }

        [TestMethod]
        public void FindSignerTest()
        {
            TestUtil.TestFindLinked(() => new CaseFile().FindSigner(0));
        }

        [TestMethod]
        public void SendTest()
        {
            TestUtil.TestPerformActionSuccess(() => new CaseFile().Send());
        }

        [TestMethod]
        public void ActivateTest()
        {
            TestUtil.TestPerformActionSuccess(() => new CaseFile().Activate());
        }

        [TestMethod]
        public void TestJsonDeserialization()
        {
            const string json = "{\"signers\":[{\"sdkClassName\":\"Signer\",\"id\":334,\"name\":\"A signer\",\"signingRequest\":{\"sdkClassName\":\"SigningRequest\",\"id\":334,\"email\":\"test@example.com\",\"emailSubject\":\"Test subject\",\"emailText\":\"Test text\",\"status\":1,\"accessControl\":true}}],\"sdkClassName\":\"CaseFile\",\"id\":245,\"title\":\"CF\",\"status\":1,\"documents\":[{\"sdkClassName\":\"Document\",\"id\":359,\"documentId\":\"CB5VL-GS115-G5KDD-GFHKH-IPGAX-3J0LZ\",\"title\":\"My Doc\",\"status\":0,\"signable\":true,\"signatureLines\":[{\"signerId\":334,\"sdkClassName\":\"SignatureLine\",\"id\":477,\"signOrder\":0}],\"created\":\"1412092744\",\"modified\":\"1412092744\",\"completed\":\"1412092744\"}],\"signIteration\":2,\"visibilityMode\":3,\"created\":\"1412092736\"}";

            var caseFile = JsonConvert.DeserializeObject<CaseFile>(json);

            //Case File
            Assert.IsNotNull(caseFile);
            Assert.AreEqual(245, caseFile.Id);
            Assert.AreEqual("CF", caseFile.Title);
            Assert.AreEqual(1, caseFile.Status);
            Assert.AreEqual(2, caseFile.SignIteration);
            Assert.AreEqual(3, caseFile.VisibilityMode);
            Assert.AreEqual(new DateTime(2014, 9, 30, 15, 58, 56), caseFile.Created);

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