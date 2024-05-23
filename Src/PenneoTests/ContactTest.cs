using System.Threading.Tasks;
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
            Assert.That(c.Name, Is.EqualTo("C"));
        }

        [Test]
        public async Task PersistSuccessTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPersist(con, () => new Contact());
        }

        [Test]
        public async Task PersistFailTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestPersistFail(con, () => new Contact());
        }

        [Test]
        public async Task DeleteTest()
        {
            var con = TestUtil.CreatePenneoConnector();
            await TestUtil.TestDelete(con, () => new Contact());
        }

        [Test]
        public async Task GetTest()
        {
            await TestUtil.TestGet<Contact>();
        }

        [Test]
        public void TestJsonDeserialization()
        {
            const string json = "{\"name\":\"cname\",\"email\":\"mail@mail.dk\",\"id\":1234}";

            var contact = JsonConvert.DeserializeObject<Contact>(json);

            //Case File
            Assert.That(contact, Is.Not.Null);
            Assert.That(contact.Id, Is.EqualTo(1234));
            Assert.That(contact.Name, Is.EqualTo("cname"));
            Assert.That(contact.Email, Is.EqualTo("mail@mail.dk"));
        }
    }
}