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
        public string SocialSecurityNumber { get; set; }
        public string OnBehalfOf { get; set; }
        public string VATIdentificationNumber { get; set; }
        public CaseFile CaseFile { get; internal set; }

        internal override Entity Parent
        {
            get { return CaseFile; }
        }

        public SigningRequest GetSigningRequest()
        {
            return GetLinkedEntities<SigningRequest>().FirstOrDefault();
        }
    }
}