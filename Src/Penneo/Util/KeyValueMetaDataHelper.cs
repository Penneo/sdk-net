using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Penneo.Util
{
    public class KeyValueMetaDataHelper
    {
        private readonly Dictionary<string, object> _keyValues;

        public int Count { get { return _keyValues.Count; }}

        public KeyValueMetaDataHelper(string metaData = null)
        {
            _keyValues = ToKeyValueMetaData(metaData);
        }

        public void Add(string key, object value)
        {
            _keyValues[key] = value;
        }

        public void AddBytes(string key, byte[] rawBytes)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            if (rawBytes == null)
            {
                throw new ArgumentNullException("rawBytes");
            }
            var base64 = Convert.ToBase64String(rawBytes, Base64FormattingOptions.None);
            _keyValues[key] = "#base64#" + base64;
        }

        public void AddFile(string key, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(filePath);
            }
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }
            var bytes = File.ReadAllBytes(filePath);
            _keyValues.AddBytes(key, bytes);
        }

        public object GetMetaDataValue(string key)
        {
            object val;
            if (_keyValues.TryGetValue(key, out val))
            {
                if (!IsBase64(val))
                {
                    return val;
                }
                var valWithoutTypeDef = ((string)val).Substring(8);
                var bytes = Convert.FromBase64String(valWithoutTypeDef);
                return bytes;
            }
            throw new KeyNotFoundException(key);
        }

        public void WriteFile(string key, string filePath)
        {
            var val = _keyValues.GetMetaDataValue(key);
            if (val == null)
            {
                File.WriteAllText("", filePath);
                return;
            }
            var s = val as string;
            if (!string.IsNullOrEmpty(s))
            {
                File.WriteAllText(filePath, s);
                return;
            }
            var bytes = val as byte[];
            if (bytes != null)
            {
                File.WriteAllBytes(filePath, bytes);
                return;
            }
            throw new NotSupportedException("WriteFile does not support the value type " + val.GetType().Name);
        }

        public string ToJson()
        {
            var json = JsonConvert.SerializeObject(_keyValues);
            return json;
        }

        private static Dictionary<string, object> ToKeyValueMetaData(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return new Dictionary<string, object>();
            }
            var o = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            return o;
        }

        public void ResolveFileTags()
        {
            foreach (var entry in _keyValues)
            {
                var s = entry.Value as string;

                if (string.IsNullOrEmpty(s))
                {
                    continue;
                }
                if (s.StartsWith("#file#"))
                {
                    var filePath = s.Substring(6);
                    var bytes = File.ReadAllBytes(filePath);
                    var base64 = Convert.ToBase64String(bytes);
                    s = "#base64#" + base64;
                    _keyValues[entry.Key] = s;
                }
            }
        }

        private static bool IsBase64(object val)
        {
            if (val == null)
            {
                return false;
            }
            var s = val as string;
            return s != null && s.StartsWith("#base64#");
        }
    }
}
