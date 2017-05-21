using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Penneo
{
    public class Folder : Entity
    {
        private int? _parentId;
        private Folder _parentFolder;

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
        public Folder ParentFolder
        {
            get { return _parentFolder; }
            set
            {
                _parentFolder = value;

                //Synchronize ParentId with the new ParentFolder
                if (value != null && value.Id.HasValue)
                {
                    _parentId = value.Id.Value;
                }
                else
                {
                    _parentId = null;
                }
            }
        }

        public int? ParentId
        {
            get { return _parentId; }
            set
            {
                _parentId = value;

                //Synchronize ParentFolder with the new ParentId
                if (_parentFolder != null && _parentFolder.Id != _parentId)
                {
                    _parentFolder = null;
                }
            }
        }

        public ICollection<Folder> ChildFolders { get; set; } 

        public IEnumerable<CaseFile> GetCaseFiles()
        {
            if (!Id.HasValue)
            {
                return new List<CaseFile>();
            }
            return GetLinkedEntities<CaseFile>("folders/" + Id + "/casefiles").Objects;
        }

        public bool AddCaseFile(CaseFile caseFile)
        {
            return LinkEntity(caseFile);
        }

        public bool RemoveCaseFile(CaseFile caseFile)
        {
            return UnlinkEntity(caseFile);
        }

        public IEnumerable<Validation> GetValidations()
        {
            if (!Id.HasValue)
            {
                return new List<Validation>();
            }
            return GetLinkedEntities<Validation>("folders/" + Id + "/validations").Objects;
        }

        public bool AddValidation(Validation validation)
        {
            return LinkEntity(validation);
        }

        public bool RemoveValidation(Validation validation)
        {
            return UnlinkEntity(validation);
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
