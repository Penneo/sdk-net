using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penneo;

namespace PenneoTests
{
    [TestClass]
    public class CastFileTests
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var cf = new CaseFile("CF");
            Assert.AreEqual("CF", cf.Title);
        }

        [TestMethod]
        public void PersistSuccessTest()
        {
            TestUtil.TestPersist(() => new CaseFile());
        }

        [TestMethod]
        [ExpectedException(typeof (Exception))]
        public void PersistFailTest()
        {
            TestUtil.TestPersistFail(() => new CaseFile());
        }

        [TestMethod]
        public void DeleteTest()
        {
            TestUtil.TestDelete(() => new CaseFile());
        }

        [TestMethod]
        public void GetTest()
        {
            TestUtil.TestGet<CaseFile>();
        }

        [TestMethod]
        public void GetDocumentsTest()
        {
            TestUtil.TestGetLinked(new CaseFile().GetDocuments);
        }

        [TestMethod]
        public void GetSignersTest()
        {
            TestUtil.TestGetLinked(new CaseFile().GetSigners);
        }

        [TestMethod]
        public void FindSignerTest()
        {
            TestUtil.TestFindLinked(() => new CaseFile().FindSigner(0));
        }

        [TestMethod]
        public void SendTest()
        {
            TestUtil.TestPerformActionSuccess(() => new CaseFile().Send());
        }

        [TestMethod]
        public void ActivateTest()
        {
            TestUtil.TestPerformActionSuccess(() => new CaseFile().Activate());
        }  

    }
}