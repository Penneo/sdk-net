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
            Assert.AreEqual("title", f.Title);
        }

        [Test]
        public void PersistSuccessTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestPersist(con, () => new Folder());
        }

        [Test]
        public void PersistFailTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestPersistFail(con, () => new Folder());
        }

        [Test]
        public void DeleteTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestDelete(con, () => new Folder());
        }

        [Test]
        public void GetTest()
        {
            TestUtil.TestGet<Folder>();
        }

        [Test]
        public void AddCaseFileTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var folder = new Folder();
            var cf = new CaseFile();
            TestUtil.TestLink(con, () => folder.AddCaseFile(con, cf), folder, cf);
        }

        [Test]
        public void RemoveCaseFileTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var folder = new Folder();
            var cf = new CaseFile();
            TestUtil.TestUnlink(con, () => folder.RemoveCaseFile(con, cf), folder, cf);
        }

        [Test]
        public void AddValidationTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var folder = new Folder();
            var validation = new Validation();
            TestUtil.TestLink(con, () => folder.AddValidation(con, validation), folder, validation);
        }

        [Test]
        public void RemoveValidationTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            var folder = new Folder();
            var validation = new Validation();
            TestUtil.TestUnlink(con, () => folder.RemoveValidation(con, validation), folder, validation);
        }
    }
}