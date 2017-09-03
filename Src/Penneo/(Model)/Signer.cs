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

        public SigningRequest GetSigningRequest(PenneoConnector con)
        {
            if (SigningRequest == null)
            {
                SigningRequest = GetLinkedEntities<SigningRequest>(con).Objects.FirstOrDefault();
            }
            return SigningRequest;
        }

        public bool AddSignerType(PenneoConnector con, SignerType type)
        {
            return LinkEntity(con, type);
        }

        public bool RemoveSignerType(PenneoConnector con, SignerType type)
        {
            return UnlinkEntity(con, type);
        }

        public IEnumerable<SignerType> GetSignerTypes(PenneoConnector con)
        {
            return GetLinkedEntities<SignerType>(con).Objects;
        }

        public IEnumerable<LogEntry> GetEventLog(PenneoConnector con)
        {
            return GetLinkedEntities<LogEntry>(con).Objects;
        }
    }
}
