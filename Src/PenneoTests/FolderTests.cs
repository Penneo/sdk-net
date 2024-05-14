using System.Threading.Tasks;
using NUnit.Framework;
using Penneo;

namespace PenneoTests
{
    [TestFixture]
    public class FolderTests
    {
        [Test]
        public void ConstructorTest()
        {
            var f = new Folder("title");
            Assert.That(f.Title, Is.EqualTo("title"));
        }

        [Test]
        public async Task PersistSuccessTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPersist(con, () => new Folder());
        }

        [Test]
        public async Task PersistFailTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPersistFail(con, () => new Folder());
        }

        [Test]
        public async Task DeleteTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestDelete(con, () => new Folder());
        }

        [Test]
        public async Task GetTest()
        {
            await TestUtil.TestGet<Folder>();
        }

        [Test]
        public async Task AddCaseFileTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var folder = new Folder();
            var cf = new CaseFile();
            await TestUtil.TestLink(con, () => folder.AddCaseFileAsync(con, cf), folder, cf);
        }

        [Test]
        public async Task RemoveCaseFileTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var folder = new Folder();
            var cf = new CaseFile();
            await TestUtil.TestUnlink(con, () => folder.RemoveCaseFileAsync(con, cf), folder, cf);
        }

        [Test]
        public async Task AddValidationTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var folder = new Folder();
            var validation = new Validation();
            await TestUtil.TestLink(con, () => folder.AddValidationAsync(con, validation), folder, validation);
        }

        [Test]
        public async Task RemoveValidationTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var folder = new Folder();
            var validation = new Validation();
            await TestUtil.TestUnlink(con, () => folder.RemoveValidationAsync(con, validation), folder, validation);
        }
    }
}
