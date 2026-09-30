using System.Collections.Generic;

namespace Penneo
{
    [global::System.Obsolete(global::Penneo.SdkDeprecation.Message)]
    public class CaseFileTemplate : GenericEntity<int?>
    {
        public string Name { get; set; }
        public IEnumerable<DocumentType> DocumentTypes { get; set; }
    }
}
