using System;
using System.IO;

namespace Penneo.Util
{
    /// <summary>
    /// File utilities
    /// </summary>
    internal static class FileUtil
    {
        /// <summary>
        /// Reads the contents from a file path, and convert it to a base64 encoded string
        /// </summary>
        public static string GetBase64(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(path);
            }
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path + " not found");
            }

            var fileContents = File.ReadAllBytes(path);
            var encoded = Convert.ToBase64String(fileContents);
            return encoded;
        }
    }
}