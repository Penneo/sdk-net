using System;
using System.IO;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penneo;

namespace PenneoTests
{
    [TestClass]
    public class FolderTests
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var f = new Folder("title");
            Assert.AreEqual("title", f.Title);
        }

        [TestMethod]
        public void PersistSuccessTest()
        {
            TestUtil.TestPersist(() => new Folder());
        }

        [TestMethod]
        [ExpectedException(typeof (Exception))]
        public void PersistFailTest()
        {
            TestUtil.TestPersistFail(() => new Folder());
        }

        [TestMethod]
        public void DeleteTest()
        {
            TestUtil.TestDelete(() => new Folder());
        }

        [TestMethod]
        public void GetTest()
        {
            TestUtil.TestGet<Folder>();
        }
        
        [TestMethod]
        public void AddCaseFileTest()
        {
            var folder = new Folder();
            var cf = new CaseFile();
            TestUtil.TestLink(() => folder.AddCaseFile(cf), folder, cf);
        }

        [TestMethod]
        public void RemoveCaseFileTest()
        {
            var folder = new Folder();
            var cf = new CaseFile();
            TestUtil.TestUnlink(() => folder.RemoveCaseFile(cf), folder, cf);
        }
    }
}