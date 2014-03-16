using System;

namespace Penneo.Util
{
    /// <summary>
    /// String utilities
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// Sets the first char to lowercase in a string
        /// </summary>
        public static string FirstCharacterToLower(string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
                return str;

            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
    }
}