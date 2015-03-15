using System;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penneo;

namespace PenneoTests
{
    [TestClass]
    public class SignatureLineTests
    {
        private static SignatureLine CreateSignatureLine()
        {
            var cf = new CaseFile();
            var doc = new Document(cf);
            var s = new SignatureLine(doc, "role", 1, "conditions");
            return s;
        }

        [TestMethod]
        public void ConstructorTest()
        {
            var s = CreateSignatureLine();
            Assert.IsNotNull(s.Document);
            Assert.AreEqual("role", s.Role);
            Assert.AreEqual(1, s.SignOrder);
            Assert.AreEqual("conditions", s.Conditions);
            Assert.AreEqual(s.Document, s.Parent);
        }
        
        [TestMethod]
        public void PersistSuccessTest()
        {
            TestUtil.TestPersist(CreateSignatureLine);
        }

        [TestMethod]
        public void PersistFailTest()
        {
            TestUtil.TestPersistFail(CreateSignatureLine);
        }

        [TestMethod]
        public void DeleteTest()
        {
            TestUtil.TestDelete(CreateSignatureLine);
        }

        [TestMethod]
        public void GetTest()
        {
            TestUtil.TestGet<SignatureLine>();
        }

        [TestMethod]
        public void SetSignerSuccessTest()
        {
            var sl = CreateSignatureLine();
            var s = new Signer(sl.Document.CaseFile);

            var connector = TestUtil.CreateFakeConnector();
            A.CallTo(() => connector.LinkEntity(sl, s)).Returns(true);

            sl.SetSigner(s);

            Assert.AreEqual(s, sl.Signer);
            A.CallTo(() => connector.LinkEntity(sl, s)).MustHaveHappened();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SetSignerFailTest()
        {
            var sl = CreateSignatureLine();
            var s = new Signer(sl.Document.CaseFile);

            var connector = TestUtil.CreateFakeConnector();
            A.CallTo(() => connector.LinkEntity(sl, s)).Returns(false);

            try
            {
                sl.SetSigner(s);
            }
            finally
            {
                A.CallTo(() => connector.LinkEntity(sl, s)).MustHaveHappened();
            }
        }   
    }
}