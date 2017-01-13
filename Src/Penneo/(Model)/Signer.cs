using System.Collections.Generic;
using System.Linq;

namespace Penneo
{
    public class Signer : Entity
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
        public string SocialSecurityNumber { get; set; }
        public string OnBehalfOf { get; set; }
        public string VATIdentificationNumber { get; set; }
        public CaseFile CaseFile { get; set; }
        public SigningRequest SigningRequest { get; set; }

        internal override Entity Parent
        {
            get { return CaseFile; }
        }

        public SigningRequest GetSigningRequest()
        {
            if (SigningRequest == null)
            {
                SigningRequest = GetLinkedEntities<SigningRequest>().Objects.FirstOrDefault();
            }
            return SigningRequest;
        }

        public bool AddSignerType(SignerType type)
        {
            return LinkEntity(type);
        }

        public bool RemoveSignerType(SignerType type)
        {
            return UnlinkEntity(type);
        }

        public IEnumerable<SignerType> GetSignerTypes()
        {
            return GetLinkedEntities<SignerType>().Objects;
        }

        public IEnumerable<LogEntry> GetEventLog()
        {
            return GetLinkedEntities<LogEntry>().Objects;
        }
    }
}
