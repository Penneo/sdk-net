using System;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using Penneo;

namespace PenneoTests
{
    [TestFixture]
    public class SignatureLineTests
    {
        private static SignatureLine CreateSignatureLine()
        {
            var cf = new CaseFile();
            var doc = new Document(cf);
            var s = new SignatureLine(doc, "role", 1, "conditions", DateTime.Now, DateTime.Now);
            return s;
        }

        [Test]
        public void ConstructorTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var s = CreateSignatureLine();
            Assert.IsNotNull(s.Document);
            Assert.AreEqual("role", s.Role);
            Assert.AreEqual(1, s.SignOrder);
            Assert.AreEqual("conditions", s.Conditions);
            Assert.IsNotNull(s.ActiveAt);
            Assert.IsNotNull(s.ExpireAt);
            Assert.AreEqual(s.Document, s.Parent);
        }

        [Test]
        public void PersistSuccessTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestPersist(con, CreateSignatureLine);
        }

        [Test]
        public void PersistFailTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestPersistFail(con, CreateSignatureLine);
        }

        [Test]
        public void DeleteTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestDelete(con, CreateSignatureLine);
        }

        [Test]
        public void GetTest()
        {
            TestUtil.TestGet<SignatureLine>();
        }

        [Test]
        public void SetSignerSuccessTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var sl = CreateSignatureLine();
            var s = new Signer(sl.Document.CaseFile);

            A.CallTo(() => con.ApiConnector.LinkEntityAsync(sl, s)).Returns(true);

            sl.SetSignerAsync(con, s);

            Assert.AreEqual(s, sl.Signer);
            A.CallTo(() => con.ApiConnector.LinkEntityAsync(sl, s)).MustHaveHappened();
        }

        [Test]
        public async Task SetSignerFailTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var sl = CreateSignatureLine();
            var s = new Signer(sl.Document.CaseFile);

            A.CallTo(() => con.ApiConnector.LinkEntityAsync(sl, s)).Returns(false);

            try
            {
                var result = await sl.SetSignerAsync(con, s);
                Assert.IsFalse(result);
            }
            finally
            {
                A.CallTo(() => con.ApiConnector.LinkEntityAsync(sl, s)).MustHaveHappened();
            }
        }   
    }
}