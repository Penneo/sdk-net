using System.Runtime.Serialization;

namespace Penneo
{
    /// <summary>
    /// You can configure Penneo to send a request to your servers when certain events occur.
    /// See https://github.com/Penneo/sdk-net/blob/master/docs/webhooks.md for more details.
    /// </summary>
    public class WebhookSubscription : EntityWithStringId
    {
        public int CustomerId { get; set; }

        public string Secret { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// List of event types that should trigger the webhook
        /// </summary>
        public EventType[] EventTypes;

        /// <summary>
        /// Target URL for the webhook
        /// </summary>
        public string Endpoint { get; set; }

    }

    public enum EventType
    {
        [EnumMember(Value = "sign.casefile.completed")]
        CaseFileCompleted,

        [EnumMember(Value = "sign.casefile.expired")]
        CaseFileExpired,

        [EnumMember(Value = "sign.casefile.failed")]
        CaseFileFailed,

        [EnumMember(Value = "sign.casefile.rejected")]
        CaseFileRejected,

        [EnumMember(Value = "sign.signer.requestSent")]
        SignerRequestSent,

        [EnumMember(Value = "sign.signer.requestOpened")]
        SignerRequestOpened,

        [EnumMember(Value = "sign.signer.signed")]
        SignerSigned,

        [EnumMember(Value = "sign.signer.rejected")]
        SignerRejected,

        [EnumMember(Value = "sign.signer.reminderSent")]
        SignerReminderSent,

        [EnumMember(Value = "sign.signer.undeliverable")]
        SignerUndeliverable,

        [EnumMember(Value = "sign.signer.requestActivated")]
        SignerRequestActivated,

        [EnumMember(Value = "sign.signer.finalized")]
        SignerFinalized,

        [EnumMember(Value = "sign.signer.deleted")]
        SignerDeleted,

        [EnumMember(Value = "sign.signer.signedWithImageUploadAndNAP")]
        SignerSignedWithImageUploadAndNAP,

        [EnumMember(Value = "sign.signer.transientBounce")]
        SignerTransientBounce,

        [EnumMember(Value = "webhook.subscription.test")]
        WebhookSubscriptionTest
    }

}
