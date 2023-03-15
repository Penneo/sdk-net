using System;

namespace Penneo
{
    public class SignerType : Entity
    {
        public string Role { get; set; }
        public int? UpperLimit { get; set; }
        public int? LowerLimit { get; set; }
        public int SignOrder { get; set; }
        public string Conditions { get; set; }
    }
}
