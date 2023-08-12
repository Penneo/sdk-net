using System.Threading.Tasks;
using NUnit.Framework;
using Penneo;

namespace PenneoTests
{
    [TestFixture]
    public class SigningRequestTests
    {        
        [Test]
        public async Task PersistSuccessTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPersist(con, () => new SigningRequest());
        }

        [Test]
        public async Task PersistFailTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPersistFail(con, () => new SigningRequest());
        }

        [Test]
        public async Task DeleteTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestDelete(con, () => new SigningRequest());
        }

        [Test]
        public async Task GetTest()
        {
            await TestUtil.TestGet<SigningRequest>();
        }

        [Test]
        public void GetLinkTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestGetTextAsset(con, () => new SigningRequest().GetLinkAsync(con));
        }

        [Test]
        public async Task SendTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPerformActionSuccess(con, () => new SigningRequest().SendAsync(con));
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