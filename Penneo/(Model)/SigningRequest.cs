namespace Penneo
{
    public class SigningRequest : Entity
    {
        private const string ASSET_LINK = "link";
        private const string ACTION_SEND = "send";

        public string Email { get; set; }
        public string EmailText { get; set; }
        public string RejectReason { get; set; }
        public string SuccessUrl { get; set; }
        public string FailUrl { get; set; }
        public int Status { get; internal set; }

        public string GetLink()
        {
            return GetTextAssets(ASSET_LINK);
        }

        public bool Send()
        {
            return PerformAction(ACTION_SEND);
        }
    }
}