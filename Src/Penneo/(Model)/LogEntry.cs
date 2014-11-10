using System;

namespace Penneo
{
    public class LogEntry : Entity
    {
        public int EventType { get; set; }
        public DateTime? EventTime { get; set; }
    }
}