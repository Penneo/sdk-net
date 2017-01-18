using System;
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

        public Document()
        {
            MetaData = null;
            Type = TYPE_ATTACHMENT;
        }

        public Document(CaseFile cf)
            : this()
        {
            CaseFile = cf;
        }

        public Document(CaseFile cf, string title, string pdfFilePath)
            : this(cf)
        {
            CaseFile = cf;
            Title = title;
            PdfFile = pdfFilePath;
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
        /// Reference to the pdf file on disk, which will be uploaded to the document
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
            set
            {
                _pdfRaw = value;
            }
        }

        /// <summary>
        /// The document type. See <see cref="DocumentType"/>.
        /// </summary>
        public string Type { get; internal set; }

        /// <summary>
        /// The document type
        /// </summary>
        [JsonIgnore]
        public DocumentType DocumentType { get; set; }

        /// <summary>
        /// Get the document type
        /// </summary>
        public DocumentType GetDocumentType()
        {
            if (Id.HasValue && DocumentType == null)
            {
                var documentTypes = GetLinkedEntities<DocumentType>();
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
            get { return Type == TYPE_SIGNABLE; }
            set { Type = value ? TYPE_SIGNABLE : TYPE_ATTACHMENT; }
        }

        /// <summary>
        /// Reference to the case file
        /// </summary>
        public CaseFile CaseFile
        {
            get
            {
                if (_caseFile == null)
                {
                    _caseFile = GetLinkedEntities<CaseFile>().Objects.FirstOrDefault();
                }
                return _caseFile;
            }
            internal set { _caseFile = value; }
        }

        /// <summary>
        /// Make the document signable
        /// </summary>
        public void MakeSignable()
        {
            Type = TYPE_SIGNABLE;
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
        public byte[] GetPdf()
        {
            if (PdfRaw == null)
            {
                PdfRaw = GetFileAssets(ASSET_PDF);
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
        public void SavePdf(string path)
        {
            var data = GetPdf();
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
        public IEnumerable<SignatureLine> GetSignatureLines()
        {
            if (_signatureLines == null)
            {
                _signatureLines = GetLinkedEntities<SignatureLine>().Objects.ToList();
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
        public SignatureLine FindSignatureLine(int id)
        {
            if (_signatureLines != null)
            {
                return _signatureLines.FirstOrDefault(x => x.Id == id);
            }
            var sl = FindLinkedEntity<SignatureLine>(id);
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
