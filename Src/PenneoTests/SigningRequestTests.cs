using NUnit.Framework;
using Penneo;

namespace PenneoTests
{
    [TestFixture]
    public class SigningRequestTests
    {        
        [Test]
        public void PersistSuccessTest()
        {
            TestUtil.TestPersist(() => new SigningRequest());
        }

        [Test]
        public void PersistFailTest()
        {
            TestUtil.TestPersistFail(() => new SigningRequest());
        }

        [Test]
        public void DeleteTest()
        {
            TestUtil.TestDelete(() => new SigningRequest());
        }

        [Test]
        public void GetTest()
        {
            TestUtil.TestGet<SigningRequest>();
        }

        [Test]
        public void GetLinkTest()
        {
            TestUtil.TestGetTextAsset(() => new SigningRequest().GetLink());
        }

        [Test]
        public void SendTest()
        {
            TestUtil.TestPerformActionSuccess(() => new SigningRequest().Send());
        }

        [Test]
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