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
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestPersist(con, () => new SigningRequest());
        }

        [Test]
        public void PersistFailTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestPersistFail(con, () => new SigningRequest());
        }

        [Test]
        public void DeleteTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestDelete(con, () => new SigningRequest());
        }

        [Test]
        public void GetTest()
        {
            TestUtil.TestGet<SigningRequest>();
        }

        [Test]
        public void GetLinkTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestGetTextAsset(con, () => new SigningRequest().GetLink(con));
        }

        [Test]
        public void SendTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestPerformActionSuccess(con, () => new SigningRequest().Send(con));
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