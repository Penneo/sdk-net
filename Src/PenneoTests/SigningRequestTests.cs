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

        [TestMethod]
        public void GetStatusTest()
        {
            var s = new SigningRequest();
            s.Status = null;
            Assert.AreEqual(SigningRequestStatus.New, s.GetStatus());

            s.Status = (int?)SigningRequestStatus.Signed;
            Assert.AreEqual(SigningRequestStatus.Signed, s.GetStatus());
        }
    }
}