using System.Collections.Generic;

namespace Penneo
{
    public class CaseFileTemplate : Entity
    {
        public string Name { get; set; }
        public IEnumerable<DocumentType> DocumentTypes { get; set; }
    }
}