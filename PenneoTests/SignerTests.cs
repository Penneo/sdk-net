using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penneo;

namespace PenneoTests
{
    [TestClass]
    public class SignerTests
    {
        private static Signer CreateSigner()
        {
            var cf = new CaseFile();
            var s = new Signer(cf, "john", "1111111111");
            return s;
        }

        [TestMethod]
        public void ConstructorTest()
        {
            var s = CreateSigner();
            Assert.IsNotNull(s.CaseFile);
            Assert.AreEqual("john", s.Name);
            Assert.AreEqual("1111111111", s.SocialSecurityNumberPlain);
            Assert.AreEqual(s.CaseFile, s.Parent);
        }
        
        [TestMethod]
        public void PersistSuccessTest()
        {
            TestUtil.TestPersist(CreateSigner);
        }

        [TestMethod]
        [ExpectedException(typeof (Exception))]
        public void PersistFailTest()
        {
            TestUtil.TestPersistFail(CreateSigner);
        }

        [TestMethod]
        public void DeleteTest()
        {
            TestUtil.TestDelete(CreateSigner);
        }

        [TestMethod]
        public void GetTest()
        {
            TestUtil.TestGet<Signer>();
        }

        [TestMethod]
        public void GetSigningRequestTest()
        {
            TestUtil.TestGetLinked(() => CreateSigner().GetSigningRequest());
        }        
    }
}