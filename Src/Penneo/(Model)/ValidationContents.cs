using System;
using Newtonsoft.Json;
using Penneo.Connector;

namespace Penneo
{
    public class ValidationContents
    {
        public string SSN { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime BirthDate { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string MailDistrict { get; set; }
        public bool Status { get; set; }

    }
}
