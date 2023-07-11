using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Penneo.Connector;

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
        private CaseFileTemplate _caseFileTemplate;

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

        public string Reference { get; set; }

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
        /// Indicates if notifications are disabled for the owner of the case file
        /// </summary>
        public bool DisableNotificationsOwner { get; set; }

        /// <summary>
        /// Indicates if sign on meeting feature is enabled
        /// </summary>
        public bool SignOnMeeting { get; set; }

        /// <summary>
        /// The case file template
        /// </summary>
        public CaseFileTemplate CaseFileTemplate
        {
            get { return _caseFileTemplate; }
            set
            {
                _caseFileTemplate = value;
                if (_caseFileTemplate != null)
                {
                    CaseFileTemplateId = _caseFileTemplate.Id;
                }
            }
        }

        /// <summary>
        /// The case file template id
        /// </summary>
        [JsonProperty(PropertyName = "caseFileTypeId")]
        public int? CaseFileTemplateId { get; set; }

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
        /// The date when the case file was activated
        /// </summary>
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime? Activated { get; set; }

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
        /// The user of the case file
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// The customer of the case file
        /// </summary>
        public int? CustomerId { get; set; }

        /// <summary>
        /// If set, documents will not be sent as email attachments to signers and the case file owner when the signing
        /// process is finalized.
        /// </summary>
        public bool DisableEmailAttachments = false;

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
        public async Task<IEnumerable<Document>> GetDocumentsAsync(PenneoConnector con)
        {
            if (_documents == null)
            {
                _documents = (await GetLinkedEntitiesAsync<Document>(con)).Objects.ToList();
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
        public async Task<IEnumerable<Signer>> GetSignersAsync(PenneoConnector con)
        {
            if (Signers == null)
            {
                Signers = (await GetLinkedEntitiesAsync<Signer>(con)).Objects.ToList();
            }
            return Signers;
        }

        /// <summary>
        /// Find a given signer on the case file. If the signer is not found in the list loaded signers, an attempt will be made to find the signer on the backend.
        /// </summary>
        public async Task<Signer> FindSignerAsync(PenneoConnector con, int id)
        {
            Signer signer = null;
            if (_signers != null)
            {
                signer = _signers.FirstOrDefault(x => x.Id == id);
            }
            if (signer == null)
            {
                signer = await FindLinkedEntityAsync<Signer>(con, id);
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
        public async Task<IEnumerable<CopyRecipient>> GetCopyRecipientsAsync(PenneoConnector con)
        {
            if (_copyRecipients == null)
            {
                _copyRecipients = (await GetLinkedEntitiesAsync<CopyRecipient>(con)).Objects.ToList();
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

            return (CaseFileStatus)Status;
        }

        /// <summary>
        /// Get all available case file templates
        /// </summary>
        public async Task<QueryResult<CaseFileTemplate>> GetTemplatesAsync(PenneoConnector con)
        {
            return await GetLinkedEntitiesAsync<CaseFileTemplate>(con, "casefile/casefiletypes");
        }

        /// <summary>
        /// Get all available documents types for this case file
        /// </summary>
        public async Task<IEnumerable<DocumentType>> GetDocumentTypesAsync(PenneoConnector con)
        {
            return (await GetLinkedEntitiesAsync<DocumentType>(con, "casefiles/" + Id + "/documenttypes")).Objects;
        }

        /// <summary>
        /// Get all available signer types for this case file
        /// </summary>
        public async Task<IEnumerable<SignerType>> GetSignerTypesAsync(PenneoConnector con)
        {
            if (!Id.HasValue)
            {
                return new List<SignerType>();
            }
            return (await GetLinkedEntitiesAsync<SignerType>(con, "casefiles/" + Id + "/signertypes")).Objects;
        }

        /// <summary>
        /// Gets the user instance (CustomerId and UserId must be set)
        /// </summary>
        public async Task<User> GetUserAsync(PenneoConnector con)
        {
            if (!CustomerId.HasValue || !UserId.HasValue)
            {
                return null;
            }
            var r = await GetLinkedEntityAsync<User>(con, "customers/" + CustomerId + "/users/" + UserId);
            return r != null ? r.Object : null;
        }

        /// <summary>
        /// Gets the customer instance (CustomerId must be set)
        /// </summary>
        public async Task<Customer> GetCustomerAsync(PenneoConnector con)
        {
            if (!CustomerId.HasValue)
            {
                return null;
            }
            var r = await GetLinkedEntityAsync<Customer>(con, "customers/" + CustomerId);
            return r != null ? r.Object : null;
        }

        /// <summary>
        /// Get the case file template for this case file
        /// </summary>
        public async Task<CaseFileTemplate> GetCaseFileTemplateAsync(PenneoConnector con)
        {
            if (Id.HasValue && CaseFileTemplate == null)
            {
                var caseFileTypes = await GetLinkedEntitiesAsync<CaseFileTemplate>(con);
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
        public async Task<IEnumerable<string>> GetErrorsAsync(PenneoConnector con)
        {
            return await GetStringListAssetAsync(con, "errors");
        }

        /// <summary>
        /// Send the case file for signing
        /// </summary>
        public async Task<bool> SendAsync(PenneoConnector con)
        {
            return (await PerformActionAsync(con, ACTION_SEND)).Success;
        }

        /// <summary>
        /// Activate the case file
        /// </summary>
        public async Task<bool> ActivateAsync(PenneoConnector con)
        {
            return (await PerformActionAsync(con, ACTION_ACTIVATE)).Success;
        }
    }
}
