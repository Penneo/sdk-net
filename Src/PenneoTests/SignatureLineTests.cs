using System;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Penneo;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace PenneoTests
{
    [TestFixture]
    public class SignatureLineTests
    {
        private static SignatureLine CreateSignatureLine()
        {
            var cf = new CaseFile();
            var doc = new Document(cf);
            var s = new SignatureLine(doc, "role", 1, "conditions");
            return s;
        }

        [Test]
        public void ConstructorTest()
        {
            var s = CreateSignatureLine();
            Assert.IsNotNull(s.Document);
            Assert.AreEqual("role", s.Role);
            Assert.AreEqual(1, s.SignOrder);
            Assert.AreEqual("conditions", s.Conditions);
            Assert.AreEqual(s.Document, s.Parent);
        }

        [Test]
        public void PersistSuccessTest()
        {
            TestUtil.TestPersist(CreateSignatureLine);
        }

        [Test]
        public void PersistFailTest()
        {
            TestUtil.TestPersistFail(CreateSignatureLine);
        }

        [Test]
        public void DeleteTest()
        {
            TestUtil.TestDelete(CreateSignatureLine);
        }

        [Test]
        public void GetTest()
        {
            TestUtil.TestGet<SignatureLine>();
        }

        [Test]
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

        [Test]
        public void SetSignerFailTest()
        {
            var sl = CreateSignatureLine();
            var s = new Signer(sl.Document.CaseFile);

            var connector = TestUtil.CreateFakeConnector();
            A.CallTo(() => connector.LinkEntity(sl, s)).Returns(false);

            try
            {
                var result = sl.SetSigner(s);
                Assert.IsFalse(result);
            }
            finally
            {
                A.CallTo(() => connector.LinkEntity(sl, s)).MustHaveHappened();
            }
        }   
    }
}