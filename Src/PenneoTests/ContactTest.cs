using System;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using Penneo;

namespace PenneoTests
{
    [TestFixture]
    public class ContactTests
    {
        [Test]
        public void ConstructorTest()
        {
            var c = new Contact("C");
            Assert.AreEqual("C", c.Name);
        }

        [Test]
        public void PersistSuccessTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestPersist(con, () => new Contact());
        }

        [Test]
        public void PersistFailTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestPersistFail(con, () => new Contact());
        }

        [Test]
        public void DeleteTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            TestUtil.TestDelete(con, () => new Contact());
        }

        [Test]
        public void GetTest()
        {
            TestUtil.TestGet<Contact>();
        }

        [Test]
        public void TestJsonDeserialization()
        {
            const string json = "{\"name\":\"cname\",\"email\":\"mail@mail.dk\",\"id\":1234}";

            var Contact = JsonConvert.DeserializeObject<Contact>(json);

            //Case File
            Assert.IsNotNull(Contact);
            Assert.AreEqual(1234, Contact.Id);
            Assert.AreEqual("cname", Contact.Name);
            Assert.AreEqual("mail@mail.dk", Contact.Email);
        }

    }
}