using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Penneo.Connector;
using Penneo.Util;

namespace Penneo
{
    public class Document : Entity
    {
        private const string TYPE_ATTACHMENT = "attachment";
        private const string TYPE_SIGNABLE = "signable";
        private const string ASSET_PDF = "pdf";
        private CaseFile _caseFile;

        private IEnumerable<SignatureLine> _signatureLines;
        private byte[] _pdfRaw;
        private DocumentType _documentType;

        public Document()
        {
            MetaData = null;
            SignType = TYPE_ATTACHMENT;
        }

        public Document(CaseFile cf)
            : this()
        {
            CaseFile = cf;
        }

        public Document(CaseFile cf, string title, string pdfFileContentOrPath)
            : this(cf)
        {
            CaseFile = cf;
            Title = title;
            
            byte[] buffer = new byte[pdfFile.Length * 3 / 4];
            if (Convert.TryFromBase64String(pdfFile, buffer, out _))
            {
                PdfRaw = buffer;
            }
            else if (!string.IsNullOrEmpty(pdfFile) && File.Exists(pdfFile))
            {
                PdfFile = pdfFile;
            }
        }

        /// <summary>
        /// The document identifier
        /// </summary>
        public string DocumentId { get; set; }
        /// <summary>
        /// The title of the document
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The document status. See <see cref="DocumentStatus"/>.
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Reference to the pdf file on disk or as a base64 string, which will be uploaded to the document
        /// </summary>
        public string PdfFile { get; set; }

        /// <summary>
        /// The raw byte array of the pdf
        /// </summary>
        public byte[] PdfRaw
        {
            get
            {
                if (_pdfRaw == null && !string.IsNullOrEmpty(PdfFile) && File.Exists(PdfFile))
                {
                    _pdfRaw = File.ReadAllBytes(PdfFile);
                }
                return _pdfRaw;
            }
            set => _pdfRaw = value;
        }

        /// <summary>
        /// The document type. See <see cref="DocumentType"/>.
        /// </summary>
        public string SignType { get; internal set; }

        /// <summary>
        /// The document type
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public DocumentType DocumentType
        {
            get { return _documentType; }
            set { _documentType = value; }
        }

        /// <summary>
        /// The document type id
        /// </summary>
        public int? DocumentTypeId => DocumentType?.Id;

        /// <summary>
        /// Get the document type
        /// </summary>
        public DocumentType GetDocumentType(PenneoConnector con)
        {
            if (Id.HasValue && DocumentType == null)
            {
                var documentTypes = GetLinkedEntities<DocumentType>(con);
                DocumentType = documentTypes.Objects.FirstOrDefault();
            }
            return DocumentType;
        }

        /// <summary>
        /// Set the document type
        /// </summary>
        public void SetDocumentType(DocumentType type)
        {
            DocumentType = type;
        }

        /// <summary>
        /// Options as json
        /// </summary>
        [JsonProperty("Options")]
        [Obsolete("Obsolete. Use Opts")]
        public string OptionsJson { get; set; }

        /// <summary>
        /// Options for the document
        /// </summary>
        [JsonIgnore]
        [Obsolete("Obsolete. Use Opts")]
        public Dictionary<string, object> Options
        {
            get
            {
                if (OptionsJson == null)
                {
                    return null;
                }
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(OptionsJson);
            }
        }

        /// <summary>
        /// Options for the document
        /// </summary>
        public Dictionary<string, object> Opts { get; set; }

        /// <summary>
        /// Custom meta data for the document
        /// </summary>
        public string MetaData { get; set; }

        /// <summary>
        /// The data the document was created
        /// </summary>
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime Created { get; set; }

        /// <summary>
        /// The data the document was last modified
        /// </summary>
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime Modified { get; set; }

        /// <summary>
        /// The date the document was completed
        /// </summary>
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime Completed { get; set; }

        /// <summary>
        /// Denotes if the document is signable or not
        /// </summary>
        public bool Signable
        {
            get { return SignType == TYPE_SIGNABLE; }
            set { SignType = value ? TYPE_SIGNABLE : TYPE_ATTACHMENT; }
        }

        /// <summary>
        /// Reference to the case file
        /// </summary>
        public CaseFile CaseFile
        {
            get { return _caseFile; }
            internal set { _caseFile = value; }
        }

        /// <summary>
        /// Get the case file (load if not loaded)
        /// </summary>
        public CaseFile GetCaseFile(PenneoConnector con)
        {
            if (CaseFile == null)
            {
                CaseFile = GetLinkedEntities<CaseFile>(con).Objects.FirstOrDefault();
            }
            return CaseFile;
        }

        /// <summary>
        /// Make the document signable
        /// </summary>
        public void MakeSignable()
        {
            SignType = TYPE_SIGNABLE;
        }

        /// <summary>
        /// Get the status of the document
        /// </summary>
        public DocumentStatus GetStatus()
        {
            if (!Status.HasValue)
            {
                return DocumentStatus.New;
            }
            return (DocumentStatus)Status;
        }

        /// <summary>
        /// Get the PDF document as byte array
        /// </summary>
        /// <returns></returns>
        public byte[] GetPdf(PenneoConnector con)
        {
            if (PdfRaw == null)
            {
                PdfRaw = GetFileAssets(con, ASSET_PDF);
            }
            return PdfRaw;
        }

        /// <summary>
        /// Set the PDF document as byte array
        /// </summary>
        public void SetPdf(byte[] data)
        {
            PdfRaw = data;
        }

        /// <summary>
        /// Save the PDF document to a file
        /// </summary>
        public void SavePdf(PenneoConnector con, string path)
        {
            var data = GetPdf(con);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllBytes(path, data);
        }

        /// <summary>
        /// Signature lines on the document
        /// NOTE: Only returns already loaded signature lines.
        /// </summary>
        public IEnumerable<SignatureLine> SignatureLines
        {
            get { return _signatureLines; }
            set { _signatureLines = value; }
        }

        /// <summary>
        /// Get signature lines on the document. If no signature lines are loaded, the method will fetch the lines from the back-end.
        /// </summary>
        public IEnumerable<SignatureLine> GetSignatureLines(PenneoConnector con)
        {
            if (_signatureLines == null)
            {
                _signatureLines = GetLinkedEntities<SignatureLine>(con).Objects.ToList();
            }
            foreach (var sl in _signatureLines)
            {
                if (sl.Document == null)
                {
                    sl.Document = this;
                }
            }
            return _signatureLines;
        }

        /// <summary>
        /// Find a specific signature line.
        /// </summary>
        public SignatureLine FindSignatureLine(PenneoConnector con, int id)
        {
            if (_signatureLines != null)
            {
                return _signatureLines.FirstOrDefault(x => x.Id == id);
            }
            var sl = FindLinkedEntity<SignatureLine>(con, id);
            sl.Document = this;
            return sl;
        }
    }

    /// <summary>
    /// Document status values
    /// </summary>
    public enum DocumentStatus
    {
        New = 0,
        Pending = 1,
        Rejected = 2,
        Deleted = 3,
        Signed = 4,
        Completed = 5,
        Quarantined = 6,
        Failed = 7
    }
}
