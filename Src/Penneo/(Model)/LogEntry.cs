using System;
using Newtonsoft.Json;
using Penneo.Connector;

namespace Penneo
{
    public class LogEntry : Entity
    {
        public int EventType { get; set; }

        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime? EventTime { get; set; }
    }
}