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
            var s = CreateSignatureLine();
            Assert.That(s.Document, Is.Not.Null);
            Assert.That(s.Role, Is.EqualTo("role"));
            Assert.That(s.SignOrder, Is.EqualTo(1));
            Assert.That(s.Conditions, Is.EqualTo("conditions"));
            Assert.That(s.ActiveAt, Is.Not.Null);
            Assert.That(s.ExpireAt, Is.Not.Null);
            Assert.That(s.Parent, Is.SameAs(s.Document));
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

            Assert.That(sl.Signer, Is.SameAs(s));
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
                Assert.That(result, Is.False);
            }
            finally
            {
                A.CallTo(() => con.ApiConnector.LinkEntityAsync(sl, s)).MustHaveHappened();
            }
        }
    }
}