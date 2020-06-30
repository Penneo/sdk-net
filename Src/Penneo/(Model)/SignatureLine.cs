using System;
using System.Linq;
using Newtonsoft.Json;
using Penneo.Connector;

namespace Penneo
{
    public class SignatureLine : Entity
    {
        public SignatureLine()
        {
        }

        public SignatureLine(Document doc)
        {
            Document = doc;
        }

        public SignatureLine(Document doc, string role)
            : this(doc)
        {
            Role = role;
        }

        public SignatureLine(Document doc, string role, int signOrder)
            : this(doc, role)
        {
            SignOrder = signOrder;
        }

        public SignatureLine(Document doc, string role, int signOrder, string conditions)
            : this(doc, role, signOrder)
        {
            Conditions = conditions;
        }

        public SignatureLine(Document doc, string role, int signOrder, string conditions, DateTime? activeAt)
            : this(doc, role, signOrder, conditions)
        {
            ActiveAt = activeAt;
        }

        public SignatureLine(Document doc, string role, int signOrder, string conditions, DateTime? activeAt, DateTime? expireAt)
            : this(doc, role, signOrder, conditions, activeAt)
        {
            ExpireAt = expireAt;
        }

        public Document Document { get; internal set; }
        public string Role { get; set; }
        public string Conditions { get; set; }
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime? ActiveAt { get; set; }
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime? ActivatedAt { get; set; }
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime? ExpireAt { get; set; }
        public int SignOrder { get; set; }
        public int? SignerId { get; set; }

        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime SignedAt { get; set; }

        internal override Entity Parent
        {
            get { return Document; }
        }

        public Signer Signer { get; private set; }

        public Signer GetSigner(PenneoConnector con)
        {
            if (Signer == null)
            {
                if (SignerId.HasValue)
                {
                    Signer = Document.CaseFile.FindSigner(con, SignerId.Value);
                }
                else
                {
                    Signer = GetLinkedEntities<Signer>(con).Objects.FirstOrDefault();
                    if (Signer != null)
                    {
                        Signer.CaseFile = Document.CaseFile;
                    }
                }
            }
            return Signer;
        }

        public bool SetSigner(PenneoConnector con, Signer signer)
        {
            Signer = signer;
            return LinkEntity(con, Signer);
        }
    }
}