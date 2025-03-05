using System;
using Newtonsoft.Json;
using Penneo.Connector;

namespace Penneo
{
    public class SignerTypeMap : GenericEntity<int?>
    {
        public int SignerId { get; private set; }
        public Signer Signer
        {
            set
            {
                if (value.Id == null)
                {
                    throw new ArgumentException("Persist the signer before calling this");
                }

                if (value.CaseFile.Id == null)
                {
                    throw new ArgumentException("Persis the case file before calling this");
                }

                CaseFileId = (int)value.CaseFile.Id;
                SignerId = (int)value.Id;
            }
        }

        public int SignerTypeId { get; set; }
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime? ActiveAt { get; set; }
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime? ExpireAt { get; set; }
        public string Role { get; set; }
        private int CaseFileId;

        internal override string GetRelativeUrl(PenneoConnector con)
        {
            return "/casefiles/" + CaseFileId + "/signers/" + SignerId + "/signertypes/" + SignerTypeId;
        }
    }

}
