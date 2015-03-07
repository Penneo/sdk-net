using System.Net;

namespace Penneo
{
    public class Error
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool ServerNotFound { get { return (int) StatusCode == 0 || StatusCode == HttpStatusCode.NotFound; } }
        public string ErrorMessage { get; set; }
    }
}
