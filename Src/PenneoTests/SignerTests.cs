using NUnit.Framework;
using Penneo;

namespace PenneoTests
{
    [TestFixture]
    public class SignerTests
    {
        private static Signer CreateSigner()
        {
            var cf = new CaseFile();
            var s = new Signer(cf, "john", "1111111111");
            return s;
        }

        [Test]
        public void ConstructorTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var s = CreateSigner();
            Assert.IsNotNull(s.CaseFile);
            Assert.AreEqual("john", s.Name);
            Assert.AreEqual("1111111111", s.SocialSecurityNumber);
            Assert.AreEqual(s.CaseFile, s.Parent);
        }

        [Test]
        public void PersistSuccessTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestPersist(con, CreateSigner);
        }

        [Test]
        public void PersistFailTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestPersistFail(con, CreateSigner);
        }

        [Test]
        public void DeleteTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestDelete(con, CreateSigner);
        }

        [Test]
        public void GetTest()
        {
            TestUtil.TestGet<Signer>();
        }

        [Test]
        public void GetSigningRequestTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestGetLinked(con, () => CreateSigner().GetSigningRequest(con));
        }        
    }
}