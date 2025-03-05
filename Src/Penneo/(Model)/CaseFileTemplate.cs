using System.Collections.Generic;

namespace Penneo
{
    public class CaseFileTemplate : GenericEntity<int?>
    {
        public string Name { get; set; }
        public IEnumerable<DocumentType> DocumentTypes { get; set; }
    }
}
