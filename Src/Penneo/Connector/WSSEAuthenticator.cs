using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;

namespace Penneo.Connector
{
    /// <summary>
    /// WSSE authenticator for RestSharp
    /// </summary>
    internal class WSSEAuthenticator : IAuthenticator
    {
        private Random _rnd;

        public WSSEAuthenticator(string userName, string password, Func<string, string, string, string> digester = null, Func<string> noncer = null)
        {
            _rnd = new Random(Guid.NewGuid().GetHashCode());
            UserName = userName;
            Password = password;
            Digester = digester ?? DefaultDigester;
            Noncer = noncer ?? DefaultNoncer;
        }

        /// <summary>
        /// The username for authentication
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The password for authentication
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The password digester. Assign to create custom digest.
        /// </summary>
        public Func<string, string, string, string> Digester { get; set; }

        /// <summary>
        /// The nonce generator. Assign to create custom nonce.
        /// </summary>
        public Func<string> Noncer { get; set; }

        #region IAuthenticator Members

        /// <summary>
        /// Adds WSSE security headers to the request
        /// </summary>        
        public ValueTask Authenticate(RestClient client, RestRequest request)
        {
            var created = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            var nonce = Base64Encode(Noncer());
            var digest = Digester(nonce, created, Password);

            request.AddHeader("Authorization", "WSSE profile=\"UsernameToken\"");
            request.AddHeader("X-WSSE", string.Format("UsernameToken Username=\"{0}\", PasswordDigest=\"{1}\", Nonce=\"{2}\", Created=\"{3}\"",
                UserName,
                digest,
                nonce,
                created
                ));
            return default;
        }

        #endregion

        /// <summary>
        /// The detault password digester
        /// </summary>
        public string DefaultDigester(string nonce, string created, string password)
        {
            return Base64Encode(Sha1(Base64Decode(nonce) + created + password));
        }

        /// <summary>
        /// The default noncer
        /// </summary>
        public string DefaultNoncer()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int size = 16;
            var charResult = new char[size];

            for (var i = 0; i < size; i++)
            {
                charResult[i] = chars[_rnd.Next(0, chars.Length - 1)];
            }
            var result = new string(charResult);
            return result;
        }

        #region Util

        protected byte[] Sha1(string str)
        {
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                return sha1.ComputeHash(Encoding.UTF8.GetBytes(str));
            }
        }

        protected string Base64Decode(string str)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }

        protected string Base64Encode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        protected string Base64Encode(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        #endregion
    }
}