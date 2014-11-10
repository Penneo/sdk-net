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

        [Obsolete("Replaced by Role. Since 1.0.13.0")]
        public string Name
        {
            get { return Role; }
            set { Role = value; }
        }
    }
}