using System.Collections.Generic;
using NUnit.Framework;
using Penneo;
using Penneo.Mapping;


namespace PenneoTests
{
    [TestFixture]
    public class MappingTests
    {
        [Test]
        public void StringMappingTest()
        {
            const string title = "My Title";

            var mapping = new MappingBuilder<CaseFile>()
                .ForCreate()
                .Map(x => x.Title)
                .ForUpdate()
                .Map(x => x.Title)
                .GetMapping();

            var obj = new CaseFile() { Title = title };
            var createValues = mapping.GetCreateValues(obj);
            var updateValues = mapping.GetUpdateValues(obj);

            Assert.That(createValues, Is.Not.Null);
            Assert.That(createValues.Count, Is.EqualTo(1));
            Assert.That((string)createValues["Title"], Is.EqualTo(title));

            Assert.That(updateValues, Is.Not.Null);
            Assert.That(updateValues.Count, Is.EqualTo(1));
            Assert.That((string)updateValues["Title"], Is.EqualTo(title));
        }

        [Test]
        public void IntegerMappingTest()
        {
            const int status = 3;

            var mapping = new MappingBuilder<CaseFile>()
                .ForCreate()
                .Map(x => x.Status)
                .ForUpdate()
                .Map(x => x.Status)
                .GetMapping();

            var obj = new CaseFile() { Status = 3 };
            var createValues = mapping.GetCreateValues(obj);
            var updateValues = mapping.GetUpdateValues(obj);

            Assert.That(createValues, Is.Not.Null);
            Assert.That(createValues.Count, Is.EqualTo(1));
            Assert.That((int)createValues["Status"], Is.EqualTo(status));

            Assert.That(updateValues, Is.Not.Null);
            Assert.That(updateValues.Count, Is.EqualTo(1));
            Assert.That((int)updateValues["Status"], Is.EqualTo(status));
        }

        [Test]
        public void CreateMappingTest()
        {
            var mappings = new Mappings();

            Assert.That(mappings.GetMapping(typeof(CaseFile)), Is.Null);

            mappings.AddMapping(
            new MappingBuilder<CaseFile>()
                .ForCreate()
                .Map(x => x.Status)
                .Create());

            Assert.That(mappings.GetMapping(typeof(CaseFile)), Is.Not.Null);
        }

        [Test]
        public void KeyNotFoundTest()
        {
            var mapping = new MappingBuilder<CaseFile>()
                .ForCreate()
                .Map(x => x.Title)
                .GetMapping();

            var obj = new CaseFile() { Status = 3 };

            //only create mapping created, so this should fail
            Assert.That(() => mapping.GetUpdateValues(obj), Throws.TypeOf<KeyNotFoundException>());
        }
    }
}