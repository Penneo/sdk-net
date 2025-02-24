using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Penneo
{
    public class Signer : EntityWithIntId
    {
        public Signer()
        {
        }

        public Signer(CaseFile cf)
        {
            CaseFile = cf;
        }

        public Signer(CaseFile cf, string name)
            : this(cf)
        {
            Name = name;
        }

        public Signer(CaseFile cf, string name, string socialSecurityNumber)
            : this(cf, name)
        {
            SocialSecurityNumber = socialSecurityNumber;
        }

        public string Name { get; set; }

        public string ValidatedName { get; set; }

        /// <summary>
        /// The signer's social security number or mobile number (including the country code with or without the leading '+').
        /// Social security numbers are validated at signing and can be also be validated to get access to the documents when the casefile is set to hold sensitive data or when access control is enabled on the signing request.
        /// When the ssnType 'sms' is used, the value is only tested when accessing the document.
        /// </summary>
        [JsonProperty(PropertyName = "socialSecurityNumberPlain")]
        public string SocialSecurityNumber { get; set; }

        /// <summary>
        /// The type of data in 'socialSecurityNumberPlain'. For example 'dk:cpr', or 'se:pin' or 'sms'.
        /// See https://app.penneo.com/api/v3/signers/ssn-types for available types. Defaults to 'legacy'.
        /// When 'sms' is used, enabling access control on the signing request is mandatory.
        /// </summary>
        public string SsnType = "legacy";

        public string OnBehalfOf { get; set; }

        public bool StoreAsContact { get; set; }

        [JsonProperty(PropertyName = "vatin")]
        public string VATIdentificationNumber { get; set; }
        
        /// <summary>
        /// Will default to the language of the casefile if not set.
        /// </summary>
        public string Language { get; set; }
        public CaseFile CaseFile { get; set; }
        public SigningRequest SigningRequest { get; set; }

        internal override Entity Parent
        {
            get { return CaseFile; }
        }

        public async Task<SigningRequest> GetSigningRequest(PenneoConnector con)
        {
            if (SigningRequest == null)
            {
                SigningRequest = (await GetLinkedEntitiesAsync<SigningRequest>(con).ConfigureAwait(false)).Objects.FirstOrDefault();
            }
            return SigningRequest;
        }

        /// <summary>
        /// Associate a signer type, if you need more control over this, create a SignerTypeMap directly instead of using
        /// this method.
        /// </summary>
        /// <param name="con"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<bool> AddSignerType(PenneoConnector con, SignerType type)
        {
            return LinkEntityAsync(con, type);
        }

        public Task<bool> RemoveSignerType(PenneoConnector con, SignerType type)
        {
            return UnlinkEntity(con, type);
        }

        public async Task<IEnumerable<SignerType>> GetSignerTypes(PenneoConnector con)
        {
            return (await GetLinkedEntitiesAsync<SignerType>(con).ConfigureAwait(false)).Objects;
        }

        public async Task<IEnumerable<LogEntry>> GetEventLog(PenneoConnector con)
        {
            return (await GetLinkedEntitiesAsync<LogEntry>(con).ConfigureAwait(false)).Objects;
        }
    }
}
