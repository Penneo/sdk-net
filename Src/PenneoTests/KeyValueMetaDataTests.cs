using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using Penneo;
using Penneo.Util;

namespace PenneoTests
{
    [TestFixture]
    public class KeyValueMetaDataTests
    {
        [Test]
        public void AddKeyValueTest()
        {
            const string key = "myKey";
            const string val = "myValue";
            var helper = new KeyValueMetaDataHelper();
            helper.Add(key, val);
            Assert.AreEqual(1, helper.Count);
            Assert.AreEqual(val, helper.GetMetaDataValue(key));

            var json = helper.ToJson();
            Assert.IsTrue(!string.IsNullOrEmpty(json));
            Assert.IsTrue(json.Contains(key));
            Assert.IsTrue(json.Contains(val));
        }

        [Test]
        public void AddBytesTest()
        {
            const string key = "myKey";
            var val = new byte[] { 1, 2, 3 };
            var helper = new KeyValueMetaDataHelper();
            helper.AddBytes(key, val);
            Assert.AreEqual(1, helper.Count);
            CollectionAssert.AreEqual(val, (byte[])helper.GetMetaDataValue(key));
        }

        [Test]
        public void AddFileTest()
        {
            var helper = new KeyValueMetaDataHelper();
            const string key = "myKey";
            const string content = "myContent";
            var filePath = Path.GetTempFileName();

            File.WriteAllText(filePath, content, Encoding.UTF8);
            helper.AddFile(key, filePath);
            var bytes = (byte[])helper.GetMetaDataValue(key);
            var text = Encoding.UTF8.GetString(bytes);
            Assert.IsTrue(content.Equals(text, StringComparison.InvariantCulture));
        }

        [Test]
        public void ExistingMetaDataTest()
        {
            const string key = "myKey";
            const string val = "myValue";
            var helper = new KeyValueMetaDataHelper("{'" + key + "':'" + val + "'}");
            var result = (string)helper.GetMetaDataValue(key);
            Assert.IsTrue(val.Equals(result, StringComparison.InvariantCulture));
        }
    }
}