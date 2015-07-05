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
        
        [JsonProperty("Options")]
        [Obsolete("Obsolete. Use Opts")]
        public string OptionsJson { get; set; }

        [JsonIgnore]
        [Obsolete("Obsolete. Use Opts")]
        public IEnumerable<DocumentTypeOption> Options
        {
            get 
            {
                if (OptionsJson == null)
                {
                    return null;
                }
                return JsonConvert.DeserializeObject<IEnumerable<DocumentTypeOption>>(OptionsJson); 
            }
        }

        public IEnumerable<DocumentTypeOption> Opts { get; set; }

        public IEnumerable<SignerType> SignerTypes { get; set; }
    }
}