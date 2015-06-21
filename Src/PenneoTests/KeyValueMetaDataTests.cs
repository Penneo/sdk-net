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
            var metaData = new Dictionary<string, object>();
            const string key = "myKey";
            const string val = "myValue";
            metaData.Add(key, val);
            Assert.AreEqual(1, metaData.Count);
            Assert.AreEqual(val, metaData[key]);
            Assert.AreEqual(val, metaData.GetMetaDataValue(key));
        }

        [Test]
        public void AddBytesTest()
        {
            var metaData = new Dictionary<string, object>();
            const string key = "myKey";
            var val = new byte[] { 1, 2, 3 };
            metaData.AddBytes(key, val);
            Assert.AreEqual(1, metaData.Count);
            CollectionAssert.AreEqual(val, (byte[])metaData.GetMetaDataValue(key));
        }

        [Test]
        public void AddFileTest()
        {
            var metaData = new Dictionary<string, object>();
            const string key = "myKey";
            var filePath = Path.GetTempFileName();
            const string content = "myContent";
            File.WriteAllText(filePath, content, Encoding.UTF8);
            metaData.AddFile(key, filePath);
            var bytes = (byte[])metaData.GetMetaDataValue(key);
            var text = Encoding.UTF8.GetString(bytes);
            Assert.IsTrue(content.Equals(text, StringComparison.InvariantCulture));
        }
    }
}