using System.Collections.Generic;

namespace Penneo
{
    public class DocumentType : Entity
    {
        public string Name { get; set; }
        public int? UpperLimit { get; set; }
        public int? LowerLimit { get; set; }
        public string Options { get; set; }
        public IEnumerable<SignerType> SignerTypes { get; set; }
    }
}