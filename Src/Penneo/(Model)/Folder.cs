using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Penneo
{
    public class Folder : EntityWithIntId
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

        public async Task<IEnumerable<CaseFile>> GetCaseFilesAsync(PenneoConnector con)
        {
            if (!Id.HasValue)
            {
                return new List<CaseFile>();
            }
            return (await GetLinkedEntitiesAsync<CaseFile>(con, "folders/" + Id + "/casefiles").ConfigureAwait(false)).Objects;
        }

        public Task<bool> AddCaseFileAsync(PenneoConnector con, CaseFile caseFile)
        {
            return LinkEntityAsync(con, caseFile);
        }

        public Task<bool> RemoveCaseFileAsync(PenneoConnector con, CaseFile caseFile)
        {
            return UnlinkEntity(con, caseFile);
        }

        public async Task<IEnumerable<Validation>> GetValidationsAsync(PenneoConnector con)
        {
            if (!Id.HasValue)
            {
                return new List<Validation>();
            }
            return (await GetLinkedEntitiesAsync<Validation>(con, "folders/" + Id + "/validations").ConfigureAwait(false)).Objects;
        }

        public Task<bool> AddValidationAsync(PenneoConnector con, Validation validation)
        {
            return LinkEntityAsync(con, validation);
        }

        public Task<bool> RemoveValidationAsync(PenneoConnector con, Validation validation)
        {
            return UnlinkEntity(con, validation);
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
