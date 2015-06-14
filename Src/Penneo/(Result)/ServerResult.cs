using System.Net;

namespace Penneo
{
    public class ServerResult
    {
        public bool Success { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string JsonContent { get; set; }

        public bool ServerNotFound { get { return (int)StatusCode == 0 || StatusCode == HttpStatusCode.NotFound; } }
        public bool ServerMaintenance { get { return (int)StatusCode == 503; } }
        public bool Unauthorized { get { return StatusCode == HttpStatusCode.Unauthorized || StatusCode == HttpStatusCode.Forbidden; }}
        public string ErrorMessage { get; set; }
    }
}
