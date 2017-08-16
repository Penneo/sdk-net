using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Penneo.Connector;
using Penneo.Util;

namespace Penneo
{
    public class CaseFile : Entity
    {
        #region CaseFileStatus enum

        /// <summary>
        /// Available case file statuses
        /// </summary>
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
        private IEnumerable<CopyRecipient> _copyRecipients;

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

        /// <summary>
        /// Case file title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The language of the case file
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Case file status
        /// <see cref="CaseFileStatus"/>
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Break the sign process into iterations
        /// </summary>
        public int SignIteration { get; set; }

        /// <summary>
        /// The visibility mode of the case file
        /// </summary>
        public int? VisibilityMode { get; set; }

        /// <summary>
        /// Indicates if the case file contains sensitive data
        /// </summary>
        public bool SensitiveData { get; set; }

        /// <summary>
        /// The case file template
        /// </summary>
        public CaseFileTemplate CaseFileTemplate { get; set; }

        /// <summary>
        /// The data when the case file was created
        /// </summary>
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime Created { get; set; }

        /// <summary>
        /// The date where the case file was sent
        /// </summary>
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime? SendAt { get; set; }

        /// <summary>
        /// The data where the case file will expire
        /// </summary>
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime? ExpireAt { get; set; }

        /// <summary>
        /// Custom meta data for the case file.
        /// </summary>
        public string MetaData { get; set; }

        /// <summary>
        /// The documents in the case file
        /// Note: This property will only return already loaded documents
        /// </summary>
        public IEnumerable<Document> Documents
        {
            get { return _documents; }
            set { _documents = value; }
        }

        /// <summary>
        /// Get documents for the case file. If the documents are not loaded, they will be fetched
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Document> GetDocuments()
        {
            if (_documents == null)
            {
                _documents = GetLinkedEntities<Document>().Objects.ToList();
                foreach (var doc in _documents)
                {
                    doc.CaseFile = this;
                }
            }
            return _documents;
        }

        /// <summary>
        /// The signers in the case file.
        /// NOTE: This property will only return already loaded signers
        /// </summary>
        public IEnumerable<Signer> Signers
        {
            get
            {
                return _signers;
            }
            set
            {
                _signers = value;
                if (_signers != null)
                {
                    foreach (var signer in _signers)
                    {
                        signer.CaseFile = this;
                    }
                }
            }
        }

        /// <summary>
        /// The signers in the case file. If the signers are not loaded, they will be fetched
        /// </summary>
        public IEnumerable<Signer> GetSigners()
        {
            if (Signers == null)
            {
                Signers = GetLinkedEntities<Signer>().Objects.ToList();
            }
            return Signers;
        }

        /// <summary>
        /// Find a given signer on the case file. If the signer is not found in the list loaded signers, an attempt will be made to find the signer on the backend.
        /// </summary>
        public Signer FindSigner(int id)
        {
            Signer signer = null;
            if (_signers != null)
            {
                signer = _signers.FirstOrDefault(x => x.Id == id);
            }
            if (signer == null)
            {
                signer = FindLinkedEntity<Signer>(id);
            }
            if (signer != null)
            {
                signer.CaseFile = this;
            }
            return signer;
        }

        /// <summary>
        /// Get copy recipients of the case file. If the copy recipients are not already loaded, they will be fetched
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CopyRecipient> GetCopyRecipients()
        {
            if (_copyRecipients == null)
            {
                _copyRecipients = GetLinkedEntities<CopyRecipient>().Objects.ToList();
                foreach (var doc in _copyRecipients)
                {
                    doc.CaseFile = this;
                }
            }
            return _copyRecipients;
        }

        /// <summary>
        /// Get copy recipients of the case file.
        /// NOTE: This property will only return already loaded copy recipients
        /// </summary>
        public IEnumerable<CopyRecipient> CopyRecipients
        {
            get { return _copyRecipients; }
            set { _copyRecipients = value; }
        }

        /// <summary>
        /// Get the status of the case file
        /// </summary>
        public CaseFileStatus GetStatus()
        {
            if (!Status.HasValue)
            {
                return CaseFileStatus.New;
            }
            return (CaseFileStatus) Status;
        }

        [Obsolete("Obsolete since 1.0.18. Use GetTemplates instead.")]
        public IEnumerable<CaseFileTemplate> GetCaseFileTemplates()
        {
            return GetLinkedEntities<CaseFileTemplate>("casefile/casefiletypes").Objects;
        }

        /// <summary>
        /// Get all available case file templates
        /// </summary>
        public QueryResult<CaseFileTemplate> GetTemplates()
        {
            return GetLinkedEntities<CaseFileTemplate>("casefile/casefiletypes");
        }

        /// <summary>
        /// Get all available documents types for this case file
        /// </summary>
        public IEnumerable<DocumentType> GetDocumentTypes()
        {
            return GetLinkedEntities<DocumentType>("casefiles/" + Id + "/documenttypes").Objects;
        }

        /// <summary>
        /// Get all available signer types for this case file
        /// </summary>
        public IEnumerable<SignerType> GetSignerTypes()
        {
            if (!Id.HasValue)
            {
                return new List<SignerType>();
            }
            return GetLinkedEntities<SignerType>("casefiles/" + Id + "/signertypes").Objects;
        }

        /// <summary>
        /// Get the case file template for this case file
        /// </summary>
        public CaseFileTemplate GetCaseFileTemplate()
        {
            if (Id.HasValue && CaseFileTemplate == null)
            {
                var caseFileTypes = GetLinkedEntities<CaseFileTemplate>();
                CaseFileTemplate = caseFileTypes.Objects.FirstOrDefault();
            }
            return CaseFileTemplate;
        }

        /// <summary>
        /// Set the case file template for this case file
        /// </summary>
        public void SetCaseFileTemplate(CaseFileTemplate template)
        {
            CaseFileTemplate = template;
        }

        /// <summary>
        /// Get all errors associated with the case file
        /// </summary>
        public IEnumerable<string> GetErrors()
        {
            return GetStringListAsset("errors");
        }

        /// <summary>
        /// Send the case file for signing
        /// </summary>
        public bool Send()
        {
            return PerformAction(ACTION_SEND).Success;
        }

        /// <summary>
        /// Activate the case file
        /// </summary>
        public bool Activate()
        {
            return PerformAction(ACTION_ACTIVATE).Success;
        }
    }
}