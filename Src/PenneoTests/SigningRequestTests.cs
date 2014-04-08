using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penneo;

namespace PenneoTests
{
    [TestClass]
    public class SigningRequestTests
    {        
        [TestMethod]
        public void PersistSuccessTest()
        {
            TestUtil.TestPersist(() => new SigningRequest());
        }

        [TestMethod]
        [ExpectedException(typeof (Exception))]
        public void PersistFailTest()
        {
            TestUtil.TestPersistFail(() => new SigningRequest());
        }

        [TestMethod]
        public void DeleteTest()
        {
            TestUtil.TestDelete(() => new SigningRequest());
        }

        [TestMethod]
        public void GetTest()
        {
            TestUtil.TestGet<SigningRequest>();
        }

        [TestMethod]
        public void GetLinkTest()
        {
            TestUtil.TestGetTextAsset(() => new SigningRequest().GetLink());
        }

        [TestMethod]
        public void SendTest()
        {
            TestUtil.TestPerformActionSuccess(() => new SigningRequest().Send());
        }
    }
}