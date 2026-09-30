using System.Collections.Generic;

namespace Penneo
{
    [global::System.Obsolete(global::Penneo.SdkDeprecation.Message)]
    public class DocumentType : GenericEntity<int?>
    {
        public string Name { get; set; }
        public int? UpperLimit { get; set; }
        public int? LowerLimit { get; set; }

        public IEnumerable<DocumentTypeOption> Opts { get; set; }

        public IEnumerable<SignerType> SignerTypes { get; set; }
    }
}
