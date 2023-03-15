using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Penneo
{
    public class DocumentType : Entity
    {
        public string Name { get; set; }
        public int? UpperLimit { get; set; }
        public int? LowerLimit { get; set; }

        public IEnumerable<DocumentTypeOption> Opts { get; set; }

        public IEnumerable<SignerType> SignerTypes { get; set; }
    }
}
