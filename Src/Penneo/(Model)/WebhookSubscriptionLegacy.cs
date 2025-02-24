using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Penneo.Connector;
using RestSharp;

namespace Penneo
{
    /// <summary>
    /// *** DEPRECATED ***
    /// You can configure Penneo to send a request to your servers whenever certain entities change.
    /// See https://github.com/Penneo/sdk-net/blob/master/docs/webhooks.md for more details.
    /// </summary>
    [Obsolete("This entity is read-only for legacy data. Creation of WebhookSubscriptionLegacy is not supported. Use " 
              + nameof(WebhookSubscription) + " instead.")]
    public class WebhookSubscriptionLegacy: EntityWithIntId
    {
        public readonly int? UserId;

        public readonly int? CustomerId;

        public readonly bool Confirmed = false;

        public readonly string Topic;

        public readonly string Endpoint;

        [JsonConverter(typeof(PenneoDateConverter))]
        public readonly DateTime? Created;
        
        [Obsolete("Creation/updating of WebhookSubscriptionLegacy is not supported. " +
                  "This entity is read-only for legacy data.", true)]
        public override async Task<bool> PersistAsync(PenneoConnector con)
        {
            throw new NotSupportedException(
                "WebhookSubscriptionLegacy is deprecated. Use the new WebhookSubscription class."
            );
        }

        [Obsolete("Creation/updating of WebhookSubscriptionLegacy is not supported. " +
                  "This entity is read-only for legacy data.", true)]
        public async Task<bool> ConfirmAsync(PenneoConnector con, string token)
        {
            throw new NotSupportedException(
                "WebhookSubscriptionLegacy is deprecated. Use the new WebhookSubscription class."
            );
        }
    }
}