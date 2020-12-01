using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Penneo.Util
{
    public static class MetaDataExtensions
    {
        public static void AddBytes(this Dictionary<string, object> d, string key, byte[] rawBytes)
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
            d[key] = "#base64#" + base64;
        }

        public static void AddFile(this Dictionary<string, object> d, string key, string filePath)
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
            d.AddBytes(key, bytes);
        }

        public static object GetMetaDataValue(this Dictionary<string, object> d, string key)
        {
            object val;
            if (d.TryGetValue(key, out val))
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

        public static void WriteFile(this Dictionary<string, object> d, string key, string filePath)
        {
            var val = d.GetMetaDataValue(key);
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

        public static string ToJson(this Dictionary<string, object> d)
        {
            var json = JsonConvert.SerializeObject(d);
            return json;
        }

        public static Dictionary<string, object> ToKeyValueMetaData(this string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return new Dictionary<string, object>();
            }
            var o = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            return o;
        }

        public static void ResolveFileTags(this Dictionary<string, object> d)
        {
            foreach (var entry in d)
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
                    d[entry.Key] = s;
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
