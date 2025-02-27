using System.Runtime.Serialization;

namespace Penneo
{
    public class MessageTemplate : EntityWithIntId
    {
        #region MessageTemplateType enum
        /// <summary>
        /// Type of message templates
        /// </summary>
        public enum MessageTemplateType
        {
            [EnumMember(Value = "signing_request")]
            SigningRequest,
            [EnumMember(Value = "completed")]
            SigningCompleted,
            [EnumMember(Value = "reminder")]
            SigningReminder,
            [EnumMember(Value = "carbon_copy_recipient")]
            SigningCcRecipient
        }
        #endregion
        /// <summary>
        /// The Id of the entity
        /// </summary>
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
