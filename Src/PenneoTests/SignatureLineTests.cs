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
        public async Task PersistSuccessTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPersist(con, CreateSignatureLine);
        }

        [Test]
        public async Task PersistFailTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPersistFail(con, CreateSignatureLine);
        }

        [Test]
        public async Task DeleteTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestDelete(con, CreateSignatureLine);
        }

        [Test]
        public async Task GetTest()
        {
            await TestUtil.TestGet<SignatureLine>();
        }

        [Test]
        public async Task SetSignerSuccessTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var sl = CreateSignatureLine();
            var s = new Signer(sl.Document.CaseFile);

            A.CallTo(() => con.ApiConnector.LinkEntityAsync(sl, s)).Returns(true);

            await sl.SetSignerAsync(con, s);

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