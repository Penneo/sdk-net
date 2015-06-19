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
            TestUtil.TestPersist(() => new Folder());
        }

        [Test]
        public void PersistFailTest()
        {
            TestUtil.TestPersistFail(() => new Folder());
        }

        [Test]
        public void DeleteTest()
        {
            TestUtil.TestDelete(() => new Folder());
        }

        [Test]
        public void GetTest()
        {
            TestUtil.TestGet<Folder>();
        }

        [Test]
        public void AddCaseFileTest()
        {
            var folder = new Folder();
            var cf = new CaseFile();
            TestUtil.TestLink(() => folder.AddCaseFile(cf), folder, cf);
        }

        [Test]
        public void RemoveCaseFileTest()
        {
            var folder = new Folder();
            var cf = new CaseFile();
            TestUtil.TestUnlink(() => folder.RemoveCaseFile(cf), folder, cf);
        }
    }
}