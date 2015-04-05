using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Penneo.Connector;

namespace Penneo
{
    public class CaseFile : Entity
    {
        #region CaseFileStatus enum

        public enum CaseFileStatus
        {
            New = 0,
            Pending = 1,
            Rejected = 2,
            Deleted = 3,
            Signed = 4,
            Completed = 5
        }

        #endregion

        private const string ACTION_SEND = "send";
        private const string ACTION_ACTIVATE = "activate";

        private IEnumerable<Document> _documents;
        private IEnumerable<Signer> _signers;

        public CaseFile()
        {
            MetaData = null;
            VisibilityMode = 0;
        }

        public CaseFile(string title)
            : this()
        {
            Title = title;
        }

        public string Title { get; set; }
        public string MetaData { get; set; }
        public int? Status { get; set; }
        public int SignIteration { get; set; }
        public int? VisibilityMode { get; set; }
        public CaseFileTemplate CaseFileTemplate { get; set; }

        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime Created { get; set; }
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime? SendAt { get; set; }
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime? ExpireAt { get; set; }

        public IEnumerable<Document> Documents
        {
            get { return _documents; }
            set { _documents = value; }
        }

        public IEnumerable<Document> GetDocuments()
        {
            if (_documents == null)
            {
                _documents = GetLinkedEntities<Document>().ToList();
                foreach (var doc in _documents)
                {
                    doc.CaseFile = this;
                }
            }
            return _documents;
        }

        public IEnumerable<Signer> Signers
        {
            get { return _signers; }
            set { _signers = value; }
        }

        public IEnumerable<Signer> GetSigners()
        {
            if (_signers == null)
            {
                _signers = GetLinkedEntities<Signer>().ToList();
                foreach (var s in _signers)
                {
                    s.CaseFile = this;
                }
            }
            return _signers;
        }

        public Signer FindSigner(int id)
        {
            if (_signers != null)
            {
                return _signers.FirstOrDefault(x => x.Id == id);
            }

            var linked = FindLinkedEntity<Signer>(id);
            linked.CaseFile = this;
            return linked;
        }

        public CaseFileStatus GetStatus()
        {
            if (!Status.HasValue)
            {
                return CaseFileStatus.New;
            }
            return (CaseFileStatus) Status;
        }

        public IEnumerable<CaseFileTemplate> GetCaseFileTemplates()
        {
            return GetLinkedEntities<CaseFileTemplate>("casefile/casefiletypes");
        }

        public IEnumerable<DocumentType> GetDocumentTypes()
        {
            return GetLinkedEntities<DocumentType>("casefiles/" + Id + "/documenttypes");
        }

        public IEnumerable<SignerType> GetSignerTypes()
        {
            if (!Id.HasValue)
            {
                return new List<SignerType>();
            }
            return GetLinkedEntities<SignerType>("casefiles/" + Id + "/signertypes");
        }

        public CaseFileTemplate GetCaseFileTemplate()
        {
            if (Id.HasValue && CaseFileTemplate == null)
            {
                var caseFileTypes = GetLinkedEntities<CaseFileTemplate>();
                CaseFileTemplate = caseFileTypes.FirstOrDefault();
            }
            return CaseFileTemplate;
        }

        public void SetCaseFileTemplate(CaseFileTemplate template)
        {
            CaseFileTemplate = template;
        }

        public IEnumerable<string> GetErrors()
        {
            return GetStringListAsset("errors");
        }

        public bool Send()
        {
            return PerformAction(ACTION_SEND).Success;
        }

        public bool Activate()
        {
            return PerformAction(ACTION_ACTIVATE).Success;
        }
    }
}