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

        public Signer(CaseFile cf, string name, string socialSecurityNumber)
            : this(cf)
        {
            Name = name;
            SocialSecurityNumberPlain = socialSecurityNumber;
        }

        public string Name { get; set; }
        public string SocialSecurityNumberPlain { get; set; }
        public string OnBehalfOf { get; set; }
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