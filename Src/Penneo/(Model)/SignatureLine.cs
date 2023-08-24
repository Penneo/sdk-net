using System;
using System.Linq;
using System.Threading.Tasks;
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

        /// <summary>
        /// Parameter to specify that the signer signs the document as a certain role (owner, secretary, etc).
        /// This will be visible on the signature page.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// A boolean expression that determines, when the signers signature is needed. Contact us for more info.
        /// </summary>
        public string Conditions { get; set; }

        /// <summary>
        /// The date when the signature line should be activated
        /// </summary>
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime? ActiveAt { get; set; }

        /// <summary>
        /// The date when the signature line was activated
        /// </summary>
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime? ActivatedAt { get; set; }

        /// <summary>
        /// The date when the signature line should expire
        /// </summary>
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime? ExpireAt { get; set; }

        /// <summary>
        /// Determines the signing order starting at zero.
        /// Every signature line having signOrder 0 must be signed before signature lines having signOrder 1 are available for signing and so forth.
        /// </summary>
        public int SignOrder { get; set; }

        /// <summary>
        /// The ID of the signer linked to the signature line
        /// </summary>
        public int? SignerId { get; set; }

        /// <summary>
        /// The date when the signature line was signed
        /// </summary>
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime SignedAt { get; set; }

        internal override Entity Parent
        {
            get { return Document; }
        }

        public Signer Signer { get; private set; }

        public async Task<Signer> GetSignerAsync(PenneoConnector con)
        {
            if (Signer == null)
            {
                if (SignerId.HasValue)
                {
                    Signer = await Document.CaseFile.FindSignerAsync(con, SignerId.Value).ConfigureAwait(false);
                }
                else
                {
                    Signer = (await GetLinkedEntitiesAsync<Signer>(con).ConfigureAwait(false)).Objects.FirstOrDefault();
                    if (Signer != null)
                    {
                        Signer.CaseFile = Document.CaseFile;
                    }
                }
            }

            return Signer;
        }

        public Task<bool> SetSignerAsync(PenneoConnector con, Signer signer)
        {
            Signer = signer;
            return LinkEntityAsync(con, Signer);
        }
    }
}
