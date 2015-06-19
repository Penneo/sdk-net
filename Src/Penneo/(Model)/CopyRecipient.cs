using System;
using System.Collections.Generic;

namespace Penneo
{
    public class CopyRecipient : Entity
    {
        public CopyRecipient()
        {
        }

        public CopyRecipient(CaseFile caseFile)
        {
            CaseFile = caseFile;
        }

        public CopyRecipient(CaseFile caseFile, string name, string email)
            : this(caseFile)
        {
            Name = name;
            Email = email;
        }

        public CaseFile CaseFile { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        internal override Entity Parent
        {
            get { return CaseFile; }
        }
    }
}