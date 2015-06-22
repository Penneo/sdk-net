using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Penneo
{
    public class Folder : Entity
    {
        public Folder()
        {
            ChildFolders = new List<Folder>();
        }

        public Folder(string title)
            : this()
        {
            Title = title;
        }

        public String Title { get; set; }

        [JsonProperty("Parent", IsReference = true)]
        public Folder ParentFolder { get; set; }

        public ICollection<Folder> ChildFolders { get; set; } 

        public IEnumerable<CaseFile> GetCaseFiles()
        {
            var caseFiles = GetLinkedEntities<CaseFile>(@"Penneo\SDK\CaseFile");
            return caseFiles.Objects;
        }

        public bool AddCaseFile(CaseFile caseFile)
        {
            return LinkEntity(caseFile);
        }

        public bool RemoveCaseFile(CaseFile caseFile)
        {
            return UnlinkEntity(caseFile);
        }

        public override string ToString()
        {
            return Title + "(" + Id + (ParentFolder != null ? "," + ParentFolder.Id : null) + ")";
        }

        public static int GetDepth(List<Folder> list, Folder folder)
        {
            if (folder.ParentFolder == null)
            {
                return 0;
            }
            return GetDepth(list, list.First(p => p.Title == folder.ParentFolder.Title)) + 1;
        }

        internal static object QueryPostProcessor(object input)
        {
            var list = (IEnumerable<Folder>)input;
            var root = list.ToDictionary(x => x.Id);
            var listToRemoveFromRoot = RemoveSubFoldersFromRoot(root, list.ToList());
            return root.Values.Except(listToRemoveFromRoot).ToList();
        }

        internal static HashSet<Folder> RemoveSubFoldersFromRoot(Dictionary<int?, Folder> root, List<Folder> folders)
        {
            var toRemove = new HashSet<Folder>();
            for (var i = 0; i < folders.Count; i++)
            {
                var f = folders[i];
                if (f.ParentFolder != null)
                {
                    Folder actualParentFolder;
                    if (root.TryGetValue(f.ParentFolder.Id, out actualParentFolder))
                    {
                        actualParentFolder.ChildFolders.Add(f);
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                    toRemove.Add(f);
                }
            }
            return toRemove;
        }
    }
}