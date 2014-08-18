using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Penneo
{
    public class Folder : Entity
    {
        public String Title { get; set; }

        public Folder()
        {            
        }

        public Folder(string title)
        {
            Title = title;
        }

        public IEnumerable<CaseFile> GetCaseFiles()
	    {
            var caseFiles = GetLinkedEntities<CaseFile>(@"Penneo\SDK\CaseFile");
            return caseFiles;
	    }

        public bool AddCaseFile(CaseFile caseFile)
        {
            return LinkEntity(caseFile);
    	}

	    public bool RemoveCaseFile(CaseFile caseFile)
	    {
		    return UnlinkEntity(caseFile);
	    }
    }
}
