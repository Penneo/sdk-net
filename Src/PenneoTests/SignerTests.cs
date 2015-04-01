using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Penneo;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

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
        public void PersistSuccessTest()
        {
            TestUtil.TestPersist(CreateSigner);
        }

        [Test]
        public void PersistFailTest()
        {
            TestUtil.TestPersistFail(CreateSigner);
        }

        [Test]
        public void DeleteTest()
        {
            TestUtil.TestDelete(CreateSigner);
        }

        [Test]
        public void GetTest()
        {
            TestUtil.TestGet<Signer>();
        }

        [Test]
        public void GetSigningRequestTest()
        {
            TestUtil.TestGetLinked(() => CreateSigner().GetSigningRequest());
        }        
    }
}