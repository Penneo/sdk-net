using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Penneo.Connector;

namespace Penneo
{
    public class Document : Entity
    {
        private const string TYPE_ATTACHMENT = "attachment";
        private const string TYPE_SIGNABLE = "signable";
        private const string ASSET_PDF = "pdf";
        private CaseFile _caseFile;
        private byte[] _pdfRaw;

        private IEnumerable<SignatureLine> _signatureLines;

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

        public string DocumentId { get; set; }
        public string Title { get; set; }
        public string MetaData { get; set; }
        public int? Status { get; set; }
        public string PdfFile { get; set; }
        public string Type { get; internal set; }
        public string Options { get; set; }
        public DocumentType DocumentType { get; set; }

        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime Created { get; set; }
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime Modified { get; set; }
        [JsonConverter(typeof(PenneoDateConverter))]
        public DateTime Completed { get; set; }

        public bool Signable
        {
            get { return Type == TYPE_SIGNABLE; }
            set { Type = value ? TYPE_SIGNABLE : TYPE_ATTACHMENT; }
        }

        public CaseFile CaseFile
        {
            get
            {
                if (_caseFile == null)
                {
                    _caseFile = GetLinkedEntities<CaseFile>().FirstOrDefault();
                }
                return _caseFile;
            }
            internal set { _caseFile = value; }
        }

        public void MakeSignable()
        {
            Type = TYPE_SIGNABLE;
        }

        public DocumentStatus GetStatus()
        {
            if (!Status.HasValue)
            {
                return DocumentStatus.New;
            }
            return (DocumentStatus) Status;
        }

        public byte[] GetPdf()
        {
            if (_pdfRaw == null)
            {
                _pdfRaw = GetFileAssets(ASSET_PDF);
            }
            return _pdfRaw;
        }

        public void SetPdf(byte[] data)
        {
            _pdfRaw = data;
        }

        public void SavePdf(string path)
        {
            var data = GetPdf();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllBytes(path, data);
        }

        public IEnumerable<SignatureLine> SignatureLines
        {
            get { return _signatureLines; }
            set { _signatureLines = value; }
        }

        public IEnumerable<SignatureLine> GetSignatureLines()
        {
            if (_signatureLines == null)
            {
                _signatureLines = GetLinkedEntities<SignatureLine>().ToList();
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

        public DocumentType GetDocumentType()
        {
            if (Id.HasValue && DocumentType == null)
            {
                var documentTypes = GetLinkedEntities<DocumentType>();
                DocumentType = documentTypes.FirstOrDefault();
            }
            return DocumentType;
        }

        public void SetDocumentType(DocumentType type)
        {
            DocumentType = type;
        }
    }

    public enum DocumentStatus
    {
        New = 0,
        Pending = 1,
        Rejected = 2,
        Deleted = 3,
        Signed = 4,
        Completed = 5
    }
}