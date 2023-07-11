using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Penneo.Connector;
using RestSharp;

namespace Penneo
{
    /// <summary>
    /// You can configure Penneo to send a request to your servers whenever certain entities change.
    /// See https://github.com/Penneo/sdk-net/blob/master/docs/webhooks.md for more details.
    /// </summary>
    public class WebhookSubscription : Entity
    {
        public int? UserId;

        public int? CustomerId;

        public bool Confirmed = false;

        public string Topic;

        public string Endpoint;

        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime? Created;

        public async Task<bool> ConfirmAsync(PenneoConnector con, string token)
        {
            var data = new Dictionary<string, object> {{"token", token}};
            return (await PerformComplexActionAsync(con, Method.Post, "confirm", data)).Success;
        }
    }
}
