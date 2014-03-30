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

        public Document Document { get; internal set; }
        public Signer Signer { get; private set; }
        public string Role { get; set; }
        public string Conditions { get; set; }
        public int SignOrder { get; set; }

        internal override Entity Parent
        {
            get { return Document; }
        }


        public void SetSigner(Signer signer)
        {
            Signer = signer;
            LinkEntity(Signer);
        }
    }
}