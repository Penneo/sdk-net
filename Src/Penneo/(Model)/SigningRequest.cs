namespace Penneo
{
    public class SigningRequest : Entity
    {
        private const string ASSET_LINK = "link";
        private const string ACTION_SEND = "send";

        public string Email { get; set; }
        public string EmailSubject { get; set; }
        public string EmailText { get; set; }
        public string ReminderEmailSubect { get; set; }
        public string ReminderEmailText { get; set; }
        public string CompletedEmailSubect { get; set; }
        public string CompletedEmailText { get; set; }
        public string EmailFormat { get; set; }
        public string RejectReason { get; set; }
        public string SuccessUrl { get; set; }
        public string FailUrl { get; set; }
        public int? Status { get; set; }
        public int? ReminderInterval { get; set; }
        public bool AccessControl { get; set; }
        public bool EnableInsecureSigning { get; set; }

        public SigningRequestStatus GetStatus()
        {
            if (!Status.HasValue)
            {
                return SigningRequestStatus.New;
            }
            return (SigningRequestStatus) Status;
        }

        public string GetLink()
        {
            return GetTextAssets(ASSET_LINK);
        }

        public bool Send()
        {
            return PerformAction(ACTION_SEND).Success;
        }
    }

    public enum SigningRequestStatus
    {
        New = 0,
        Pending = 1,
        Rejected = 2,
        Deleted = 3,
        Signed = 4,
        Undeliverable = 5
    }
}