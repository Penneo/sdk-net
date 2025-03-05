﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Penneo
{
    public class SigningRequest : EntityWithIntId
    {
        private const string ASSET_LINK = "link";
        private const string ACTION_SEND = "send";

        public string Email { get; set; }
        public string EmailSubject { get; set; }
        public string EmailText { get; set; }
        public string ReminderEmailSubject { get; set; }
        public string ReminderEmailText { get; set; }
        public string CompletedEmailSubject { get; set; }
        public string CompletedEmailText { get; set; }
        public string EmailFormat { get; set; }
        public string RejectReason { get; set; }
        public string SuccessUrl { get; set; }
        public string FailUrl { get; set; }
        public int? Status { get; set; }
        public int? ReminderInterval { get; set; }
        public bool AccessControl { get; set; }
        public bool EnableInsecureSigning { get; set; }
        public IEnumerable<string> InsecureSigningMethods { get; set; }

        public SigningRequestStatus GetStatus()
        {
            if (!Status.HasValue)
            {
                return SigningRequestStatus.New;
            }
            return (SigningRequestStatus)Status;
        }

        /// <summary>
        /// Get the signing request link
        /// </summary>
        public Task<string> GetLinkAsync(PenneoConnector con)
        {
            return GetTextAssetsAsync(con, ASSET_LINK);
        }

        /// <summary>
        /// Send the signing request for signing
        /// </summary>
        public async Task<bool> SendAsync(PenneoConnector con)
        {
            return (await PerformActionAsync(con, ACTION_SEND).ConfigureAwait(false)).Success;
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
    
    public static class InsecureSigningMethod
    {
        public const string Draw = "draw";
        public const string Image = "image";
        public const string Text = "text";
    }
}
