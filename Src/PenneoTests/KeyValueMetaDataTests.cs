﻿using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Legacy;
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
            Assert.That(helper.Count, Is.EqualTo(1));
            Assert.That(helper.GetMetaDataValue(key), Is.EqualTo(val));

            var json = helper.ToJson();
            Assert.That(json, Is.Not.Null.And.Not.Empty);
            Assert.That(json, Does.Contain(key));
            Assert.That(json, Does.Contain(val));
        }

        [Test]
        public void AddBytesTest()
        {
            const string key = "myKey";
            var val = new byte[] { 1, 2, 3 };
            var helper = new KeyValueMetaDataHelper();
            helper.AddBytes(key, val);
            Assert.That(helper.Count, Is.EqualTo(1));
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
            Assert.That(content.Equals(text, StringComparison.InvariantCulture), Is.True);
        }

        [Test]
        public void ExistingMetaDataTest()
        {
            const string key = "myKey";
            const string val = "myValue";
            var helper = new KeyValueMetaDataHelper("{'" + key + "':'" + val + "'}");
            var result = (string)helper.GetMetaDataValue(key);
            Assert.That(val.Equals(result, StringComparison.InvariantCulture), Is.True);
        }
    }
}