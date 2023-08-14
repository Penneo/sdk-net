using System.Threading.Tasks;
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
            var s = CreateSigner();
            Assert.IsNotNull(s.CaseFile);
            Assert.AreEqual("john", s.Name);
            Assert.AreEqual("1111111111", s.SocialSecurityNumber);
            Assert.AreEqual(s.CaseFile, s.Parent);
        }

        [Test]
        public async Task PersistSuccessTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPersist(con, CreateSigner);
        }

        [Test]
        public async Task PersistFailTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPersistFail(con, CreateSigner);
        }

        [Test]
        public async Task DeleteTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestDelete(con, CreateSigner);
        }

        [Test]
        public async Task GetTest()
        {
            await TestUtil.TestGet<Signer>();
        }

        [Test]
        public async Task GetSigningRequestTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestGetLinked(con, () => CreateSigner().GetSigningRequest(con));
        }        
    }
}